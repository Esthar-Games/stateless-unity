using System;

#if TASKS
using Cysharp.Threading.Tasks;
#endif

namespace Stateless
{
    public partial class StateMachine<TState, TTrigger>
    {
        internal abstract class InternalActionBehaviour
        {
            public abstract void Execute(Transition transition, object[] args);
#if TASKS
            public abstract UniTask ExecuteAsync(Transition transition, object[] args);
#endif
            public class Sync : InternalActionBehaviour
            {
                readonly Action<Transition, object[]> _action;

                public Sync(Action<Transition, object[]> action)
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
#if TASKS
            public class Async : InternalActionBehaviour
            {
                readonly Func<Transition, object[], UniTask> _action;

                public Async(Func<Transition, object[], UniTask> action)
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
#endif
        }
    }
}
