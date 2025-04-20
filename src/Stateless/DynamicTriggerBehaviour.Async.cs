#if TASKS

using System;
using Cysharp.Threading.Tasks;

namespace Stateless
{
    public partial class StateMachine<TState, TTrigger>
    {
        internal class DynamicTriggerBehaviourAsync : TriggerBehaviour
        {
            readonly Func<object[], UniTask<TState>> _destination;
            internal Reflection.DynamicTransitionInfo TransitionInfo { get; private set; }

            public DynamicTriggerBehaviourAsync(TTrigger trigger, Func<object[], UniTask<TState>> destination,
                TransitionGuard transitionGuard, Reflection.DynamicTransitionInfo info)
                : base(trigger, transitionGuard)
            {
                _destination = destination ?? throw new ArgumentNullException(nameof(destination));
                TransitionInfo = info ?? throw new ArgumentNullException(nameof(info));
            }

            public async UniTask<TState> GetDestinationState(TState source, object[] args)
            {
               return await _destination(args);
            }
        }
    }
}
#endif