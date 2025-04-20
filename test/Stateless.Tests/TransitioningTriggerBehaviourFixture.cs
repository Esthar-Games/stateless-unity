using NUnit.Framework;

namespace Stateless.Tests
{
    public class TransitioningTriggerBehaviourFixture
    {
        [Test]
        public void TransitionsToDestinationState()
        {
            var transitioning = new StateMachine<State, Trigger>.TransitioningTriggerBehaviour(Trigger.X, State.C, null);
            Assert.AreEqual(State.C, transitioning.Destination);
        }
    }
}
