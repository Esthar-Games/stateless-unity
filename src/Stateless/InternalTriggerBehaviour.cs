using System;

#if TASKS
using Cysharp.Threading.Tasks;
#endif

namespace Stateless
{
    public partial class StateMachine<TState, TTrigger>
    {
        internal abstract class InternalTriggerBehaviour : TriggerBehaviour
        {
            protected InternalTriggerBehaviour(TTrigger trigger, TransitionGuard guard) : base(trigger, guard)
            {
            }

            public abstract void Execute(Transition transition, object[] args);

#if TASKS
            public abstract UniTask ExecuteAsync(Transition transition, object[] args);
#endif

            public class Sync : InternalTriggerBehaviour
            {
                public Action<Transition, object[]> InternalAction { get; }

                public Sync(TTrigger trigger, Func<object[], bool> guard, Action<Transition, object[]> internalAction, string guardDescription = null) : base(trigger, new TransitionGuard(guard, guardDescription))
                {
                    InternalAction = internalAction;
                }

                public override void Execute(Transition transition, object[] args)
                {
                    InternalAction(transition, args);
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
            public class Async : InternalTriggerBehaviour
            {
                readonly Func<Transition, object[], UniTask> InternalAction;

                public Async(TTrigger trigger, Func<object[], bool> guard, Func<Transition, object[], UniTask> internalAction, string guardDescription = null) : base(trigger, new TransitionGuard(guard, guardDescription))
                {
                    InternalAction = internalAction;
                }

                [Obsolete]
                public Async(TTrigger trigger, Func<bool> guard, Func<Transition, object[], UniTask> internalAction, string guardDescription = null) : base(trigger, new TransitionGuard(guard, guardDescription))
                {
                    InternalAction = internalAction;
                }

                public override void Execute(Transition transition, object[] args)
                {
                    throw new InvalidOperationException(
                        $"Cannot execute asynchronous action specified in OnEntry event for '{transition.Destination}' state. " +
                         "Use asynchronous version of Fire [FireAsync]");
                }

                public override UniTask ExecuteAsync(Transition transition, object[] args)
                {
                    return InternalAction(transition, args);
                }
            }
#endif
        }
    }
}
