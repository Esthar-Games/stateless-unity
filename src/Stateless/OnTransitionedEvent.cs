using System;
using System.Collections.Generic;

#if TASKS
using Cysharp.Threading.Tasks;
#endif

namespace Stateless
{
    public partial class StateMachine<TState, TTrigger>
    {
        class OnTransitionedEvent
        {
            event Action<Transition> _onTransitioned;
#if TASKS
            readonly List<Func<Transition, UniTask>> _onTransitionedAsync = new List<Func<Transition, UniTask>>();
#endif

            public void Invoke(Transition transition)
            {
#if TASKS
                if (_onTransitionedAsync.Count != 0)
                    throw new InvalidOperationException(
                        "Cannot execute asynchronous action specified as OnTransitioned callback. " +
                        "Use asynchronous version of Fire [FireAsync]");
#endif
                _onTransitioned?.Invoke(transition);
            }

#if TASKS
            public async UniTask InvokeAsync(Transition transition)
            {
                _onTransitioned?.Invoke(transition);

                foreach (var callback in _onTransitionedAsync)
                    await callback(transition);
            }
#endif

            public void Register(Action<Transition> action)
            {
                _onTransitioned += action;
            }
#if TASKS
            public void Register(Func<Transition, UniTask> action)
            {
                _onTransitionedAsync.Add(action);
            }
#endif
        }
    }
}
