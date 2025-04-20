using Stateless.Reflection;
using NUnit.Framework;

namespace Stateless.Tests
{
    public class StateInfoTests
    {
        /// <summary>
        /// For StateInfo, Substates, FixedTransitions and DynamicTransitions are only initialised by a call to AddRelationships.
        /// However, for StateMachineInfo.InitialState, this never happens. Therefore StateMachineInfo.InitialState.Transitions
        /// throws a System.ArgumentNullException. 
        /// </summary>
        [Test]
        public void StateInfo_transitions_should_default_to_empty()
        {
            // ARRANGE
            var stateInfo = StateInfo.CreateStateInfo(new StateMachine<State, Trigger>.StateRepresentation(State.A));
        
            // ACT
            var stateInfoTransitions = stateInfo.Transitions;
        
            // ASSERT
            Assert.Null(stateInfoTransitions);
        } 
    }
}