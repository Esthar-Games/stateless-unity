#pragma warning disable CS0168
#if TASKS
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace Stateless.Tests
{
    public class DynamicTriggerBehaviourAsyncFixture
    {
        [Test]
        public async Task PermitDynamic_Selects_Expected_State_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.A)
                .PermitDynamic(Trigger.X, () => State.B);

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual(State.B, sm.State);
        }

        [Test]
        public async Task PermitDynamic_With_TriggerParameter_Selects_Expected_State_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int>(Trigger.X);
            sm.Configure(State.A)
                .PermitDynamic(trigger, i => i == 1 ? State.B : State.C);

            await sm.FireAsync(trigger, 1);

            Assert.AreEqual(State.B, sm.State);
        }

        [Test]
        public async Task PermitDynamic_Permits_Reentry_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var onExitInvoked = false;
            var onEntryInvoked = false;
            var onEntryFromInvoked = false;
            sm.Configure(State.A)
                .PermitDynamic(Trigger.X, () => State.A)
                .OnEntry(() => onEntryInvoked = true)
                .OnEntryFrom(Trigger.X, () => onEntryFromInvoked = true)
                .OnExit(() => onExitInvoked = true);

            await sm.FireAsync(Trigger.X);

            Assert.True(onExitInvoked, "Expected OnExit to be invoked");
            Assert.True(onEntryInvoked, "Expected OnEntry to be invoked");
            Assert.True(onEntryFromInvoked, "Expected OnEntryFrom to be invoked");
            Assert.AreEqual(State.A, sm.State);
        }

        [Test]
        public async Task PermitDynamic_Selects_Expected_State_Based_On_DestinationStateSelector_Function_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var value = 'C';
            sm.Configure(State.A)
                .PermitDynamic(Trigger.X, () => value == 'B' ? State.B : State.C);

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual(State.C, sm.State);
        }

        [Test]
        public async Task PermitDynamicIf_With_TriggerParameter_Permits_Transition_When_GuardCondition_Met_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int>(Trigger.X);
            sm.Configure(State.A)
                .PermitDynamicIf(trigger, (i) => i == 1 ? State.C : State.B, (i) => i == 1);

            await sm.FireAsync(trigger, 1);

            Assert.AreEqual(State.C, sm.State);
        }

        [Test]
        public async Task PermitDynamicIf_With_2_TriggerParameters_Permits_Transition_When_GuardCondition_Met_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int, int>(Trigger.X);
            sm.Configure(State.A).PermitDynamicIf(
                trigger, 
                (i, j) => i == 1 && j == 2 ? State.C : State.B, 
                (i, j) => i == 1 && j == 2);

            await sm.FireAsync(trigger, 1, 2);

            Assert.AreEqual(State.C, sm.State);
        }

        [Test]
        public async Task PermitDynamicIf_With_3_TriggerParameters_Permits_Transition_When_GuardCondition_Met_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int, int, int>(Trigger.X);
            sm.Configure(State.A).PermitDynamicIf(
                trigger,
                (i, j, k) => i == 1 && j == 2 && k == 3 ? State.C : State.B,
                (i, j, k) => i == 1 && j == 2 && k == 3);

            await sm.FireAsync(trigger, 1, 2, 3);

            Assert.AreEqual(State.C, sm.State);
        }

        [Test]
        public async Task PermitDynamicIf_With_TriggerParameter_Throws_When_GuardCondition_Not_Met_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int>(Trigger.X);
            sm.Configure(State.A)
                .PermitDynamicIf(trigger, (i) => i > 0 ? State.C : State.B, (i) => i == 2 ? true : false);

            bool caught = false;
            try
            {
                await sm.FireAsync(trigger, 1);
            }
            catch (System.Exception x)
            {
                caught = true;
            }

            Assert.That(caught);
        }

        [Test]
        public async Task PermitDynamicIf_With_2_TriggerParameters_Throws_When_GuardCondition_Not_Met_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int, int>(Trigger.X);
            sm.Configure(State.A).PermitDynamicIf(
                trigger,
                (i, j) => i > 0 ? State.C : State.B, 
                (i, j) => i == 2 && j == 3);


            bool caught = false;
            try
            {
                await sm.FireAsync(trigger, 1, 2);
            }
            catch (System.Exception x)
            {
                caught = true;
            }

            Assert.That(caught);
        }

        [Test]
        public async Task PermitDynamicIf_With_3_TriggerParameters_Throws_When_GuardCondition_Not_Met_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int, int, int>(Trigger.X);
            sm.Configure(State.A).PermitDynamicIf(trigger, 
                (i, j, k) => i > 0 ? State.C : State.B, 
                (i, j, k) => i == 2 && j == 3 && k == 4);

            bool caught = false;
            try
            {
                await sm.FireAsync(trigger, 1, 2, 3);
            }
            catch (System.Exception x)
            {
                caught = true;
            }

            Assert.That(caught);         
        }

        [Test]
        public async Task PermitDynamicIf_Permits_Reentry_When_GuardCondition_Met_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var onExitInvoked = false;
            var onEntryInvoked = false;
            var onEntryFromInvoked = false;
            sm.Configure(State.A)
                .PermitDynamicIf(Trigger.X, () => State.A, () => true)
                .OnEntry(() => onEntryInvoked = true)
                .OnEntryFrom(Trigger.X, () => onEntryFromInvoked = true)
                .OnExit(() => onExitInvoked = true);

            await sm.FireAsync(Trigger.X);

            Assert.True(onExitInvoked, "Expected OnExit to be invoked");
            Assert.True(onEntryInvoked, "Expected OnEntry to be invoked");
            Assert.True(onEntryFromInvoked, "Expected OnEntryFrom to be invoked");
            Assert.AreEqual(State.A, sm.State);
        }
    }
}
#endif