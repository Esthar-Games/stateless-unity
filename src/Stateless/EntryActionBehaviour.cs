using System;

#if TASKS
using Cysharp.Threading.Tasks;
#endif

namespace Stateless
{
    public partial class StateMachine<TState, TTrigger>
    {
        internal abstract class EntryActionBehavior
        {
            protected EntryActionBehavior(Reflection.InvocationInfo description)
            {
                Description = description;
            }

            public Reflection.InvocationInfo Description { get; }

            public abstract void Execute(Transition transition, object[] args);
#if TASKS
            public abstract UniTask ExecuteAsync(Transition transition, object[] args);
#endif

            public class Sync : EntryActionBehavior
            {
                readonly Action<Transition, object[]> _action;

                public Sync(Action<Transition, object[]> action, Reflection.InvocationInfo description) : base(description)
                {
                    _action = action;
                }

                public override void Execute(Transition transition, object[] args)
                {
                    _action(transition, args);
                }
#if TASKS
                public override UniTask ExecuteAsync(Transition transition, object[] args)
                {
                    Execute(transition, args);
                    return TaskResult.Done;
                }
#endif
            }

            public class SyncFrom<TTriggerType> : Sync
            {
                internal TTriggerType Trigger { get; }

                public SyncFrom(TTriggerType trigger, Action<Transition, object[]> action, Reflection.InvocationInfo description)
                    : base(action, description)
                {
                    Trigger = trigger;
                }

                public override void Execute(Transition transition, object[] args)
                {
                    if (transition.Trigger.Equals(Trigger))
                        base.Execute(transition, args);
                }

#if TASKS
                public override UniTask ExecuteAsync(Transition transition, object[] args)
                {
                    Execute(transition, args);
                    return TaskResult.Done;
                }
#endif
            }
#if TASKS
            public class Async : EntryActionBehavior
            {
                readonly Func<Transition, object[], UniTask> _action;

                public Async(Func<Transition, object[], UniTask> action, Reflection.InvocationInfo description) : base(description)
                {
                    _action = action;
                }

                public override void Execute(Transition transition, object[] args)
                {
                    throw new InvalidOperationException(
                        $"Cannot execute asynchronous action specified in OnEntry event for '{transition.Destination}' state. " +
                         "Use asynchronous version of Fire [FireAsync]");
                }

                public override UniTask ExecuteAsync(Transition transition, object[] args)
                {
                    return _action(transition, args);
                }
            }
            
            public class AsyncFrom<TTriggerType> : Async
            {
                internal TTriggerType Trigger { get; }

                public AsyncFrom(TTriggerType trigger, Func<Transition, object[], UniTask> action, Reflection.InvocationInfo description)
                    : base(action, description)
                {
                    Trigger = trigger;
                }

                public override void Execute(Transition transition, object[] args)
                {
                    if (transition.Trigger.Equals(Trigger))
                    {
                        base.Execute(transition, args);
                    }
                }

                public override UniTask ExecuteAsync(Transition transition, object[] args)
                {
                    if (transition.Trigger.Equals(Trigger))
                    {
                        return base.ExecuteAsync(transition, args);
                    }

                    return TaskResult.Done;
                }
            }
#endif
        }
    }
}
