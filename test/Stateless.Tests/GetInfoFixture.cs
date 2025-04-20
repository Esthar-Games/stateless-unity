using System.Linq;
#if TASKS
using Cysharp.Threading.Tasks;
#endif
using NUnit.Framework;

namespace Stateless.Tests
{
    public class GetInfoFixture
    {
        [Test]
        public void GetInfo_should_return_Entry_action_with_trigger_name()
        {
            // ARRANGE
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.B)
                .OnEntryFrom(Trigger.X, () => { });
        
            // ACT
            var stateMachineInfo = sm.GetInfo();

            // ASSERT

            var states = stateMachineInfo.States;
            Assert.That(states, Has.Count.EqualTo(1));
            var entryActions = states.First();
            Assert.That(entryActions.EntryActions, Has.Count.EqualTo(1));
            var entryActionInfo = entryActions.EntryActions.First();
            Assert.AreEqual(Trigger.X.ToString(), entryActionInfo.FromTrigger);
        }
#if TASKS
        [Test]
        public void GetInfo_should_return_async_Entry_action_with_trigger_name()
        {
            // ARRANGE
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.B)
                .OnEntryFromAsync(Trigger.X, () => UniTask.CompletedTask);
        
            // ACT
            var stateMachineInfo = sm.GetInfo();

            // ASSERT
            var states = stateMachineInfo.States;
            Assert.That(states, Has.Count.EqualTo(1));
            var entryActions = states.First();
            Assert.That(entryActions.EntryActions, Has.Count.EqualTo(1));
            var entryActionInfo = entryActions.EntryActions.First();
            Assert.AreEqual(Trigger.X.ToString(), entryActionInfo.FromTrigger);
        }
#endif
    }
}