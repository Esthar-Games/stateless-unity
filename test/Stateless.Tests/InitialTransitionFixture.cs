#pragma warning disable CS0168
#pragma warning disable CS1998
using System;
using System.Collections.Generic;
#if TASKS
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
#endif
using NUnit.Framework;

namespace Stateless.Tests
{
    public class InitialTransitionFixture
    {
        [Test]
        public void EntersSubState()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .InitialTransition(State.C);

            sm.Configure(State.C)
                .SubstateOf(State.B);

            sm.Fire(Trigger.X);
            Assert.AreEqual(State.C, sm.State);
        }

        [Test]
        public void EntersSubStateofSubstate()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .InitialTransition(State.C);

            sm.Configure(State.C)
                .InitialTransition(State.D)
                .SubstateOf(State.B);

            sm.Configure(State.D)
                .SubstateOf(State.C);

            sm.Fire(Trigger.X);
            Assert.AreEqual(State.D, sm.State);
        }

        [Test]
        public void DoesNotEnterSubStateofSubstate()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B);

            sm.Configure(State.C)
                .InitialTransition(State.D)
                .SubstateOf(State.B);

            sm.Configure(State.D)
                .SubstateOf(State.C);

            sm.Fire(Trigger.X);
            Assert.AreEqual(State.B, sm.State);
        }
#if TASKS
        [Test]
        public async Task EntersSubStateAsync()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .InitialTransition(State.C);

            sm.Configure(State.C)
                .SubstateOf(State.B);

            await sm.FireAsync(Trigger.X);
            Assert.AreEqual(State.C, sm.State);
        }

        [Test]
        public async Task EntersSubStateofSubstateAsync()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .InitialTransition(State.C);

            sm.Configure(State.C)
                .InitialTransition(State.D)
                .SubstateOf(State.B);

            sm.Configure(State.D)
                .SubstateOf(State.C);

            await sm.FireAsync(Trigger.X);
            Assert.AreEqual(State.D, sm.State);
        }

        [Test]
        public async Task DoesNotEnterSubStateofSubstateAsync()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B);

            sm.Configure(State.C)
                .InitialTransition(State.D)
                .SubstateOf(State.B);

            sm.Configure(State.D)
                .SubstateOf(State.C);

            await sm.FireAsync(Trigger.X);
            Assert.AreEqual(State.B, sm.State);
        }
#endif
        [Test]
        public void DoNotAllowTransitionToSelf()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            Assert.Throws(typeof(ArgumentException), () =>
                // This configuration would create an infinite loop
                sm.Configure(State.A)
                    .InitialTransition(State.A));
        }

        [Test]
        public void DoNotAllowTransitionToAnotherSuperstate()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .InitialTransition(State.A); // Invalid configuration, State a is a superstate

            Assert.Throws(typeof(InvalidOperationException), () =>
                sm.Fire(Trigger.X));
        }
#if TASKS
        [Test]
        public async Task DoNotAllowTransitionToAnotherSuperstateAsync()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .InitialTransition(State.A);

            bool caught = false;
            try
            {
                await sm.FireAsync(Trigger.X);
            }
            catch (System.Exception x)
            {
                caught = true;
            }

            Assert.That(caught);
        }
#endif
        [Test]
        public void DoNotAllowMoreThanOneInitialTransition()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .InitialTransition(State.C);

            Assert.Throws(typeof(InvalidOperationException), () =>
                sm.Configure(State.B)
                .InitialTransition(State.A));
        }

        [Test]
        public void Transition_with_reentry_Test()
        {
            //   -------------------------
            //   | A                     |---\
            //   |        ---------      |    X (PermitReentry)
            //   |   o--->| B     |      |<--/         
            //   |        ---------      |          
            //   |                    *  |
            //   -------------------------          
            //                 
            // X: Exit A => Enter A => Enter B

            var sm = new StateMachine<State, Trigger>(State.A); //never triggers any action!

            int order = 0;

            int onEntryStateAfired = 0;
            int onEntryStateBfired = 0;
            int onExitStateAfired = 0;
            int onExitStateBfired = 0;

            sm.Configure(State.A)
                .InitialTransition(State.B)
                .OnEntry(t => onEntryStateAfired = ++order)
                .OnExit(t => onExitStateAfired = ++order)
                .PermitReentry(Trigger.X);

            sm.Configure(State.B)
                .SubstateOf(State.A)
                .OnEntry(t => onEntryStateBfired = ++order)
                .OnExit(t => onExitStateBfired = ++order);

            sm.Fire(Trigger.X);

            Assert.AreEqual(State.B, sm.State);
            Assert.AreEqual(0, onExitStateBfired);
            Assert.AreEqual(1, onExitStateAfired);
            Assert.AreEqual(2, onEntryStateAfired);
            Assert.AreEqual(3, onEntryStateBfired);
        }

        [Test]
        public void VerifyNotEnterSuperstateWhenDoingInitialTransition()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .InitialTransition(State.C)
                .OnEntry(() => sm.Fire(Trigger.Y))
                .Permit(Trigger.Y, State.D);

            sm.Configure(State.C)
                .SubstateOf(State.B)
                .Permit(Trigger.Y, State.D);

            sm.Fire(Trigger.X);

            Assert.AreEqual(State.D, sm.State);
        }

        [Test]
        public void SubStateOfSubstateOnEntryCountAndOrder()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var onEntryCount = "";

            sm.Configure(State.A)
                .OnEntry(() => onEntryCount += "A")
                .Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .OnEntry(() => onEntryCount += "B")
                .InitialTransition(State.C);

            sm.Configure(State.C)
                .OnEntry(() => onEntryCount += "C")
                .InitialTransition(State.D)
                .SubstateOf(State.B);

            sm.Configure(State.D)
                .OnEntry(() => onEntryCount += "D")
                .SubstateOf(State.C);

            sm.Fire(Trigger.X);

            Assert.AreEqual("BCD", onEntryCount);
        }

        [Test]
        public void TransitionEvents_OrderingWithInitialTransition()
        {
            var expectedOrdering = new List<string> { "OnExitA", "OnTransitionedAB", "OnEntryB", "OnTransitionedBC", "OnEntryC", "OnTransitionCompletedAC" };
            var actualOrdering = new List<string>();

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B)
                .OnExit(() => actualOrdering.Add("OnExitA"));

            sm.Configure(State.B)
                .InitialTransition(State.C)
                .OnEntry(() => actualOrdering.Add("OnEntryB"));

            sm.Configure(State.C)
                .SubstateOf(State.B)
                .OnEntry(() => actualOrdering.Add("OnEntryC"));

            sm.OnTransitioned(t => actualOrdering.Add($"OnTransitioned{t.Source}{t.Destination}"));
            sm.OnTransitionCompleted(t => actualOrdering.Add($"OnTransitionCompleted{t.Source}{t.Destination}"));

            sm.Fire(Trigger.X);
            Assert.AreEqual(State.C, sm.State);

            Assert.AreEqual(expectedOrdering.Count, actualOrdering.Count);
            for (int i = 0; i < expectedOrdering.Count; i++)
            {
                Assert.AreEqual(expectedOrdering[i], actualOrdering[i]);
            }
        }
#if TASKS
        [Test]
        public async Task AsyncTransitionEvents_OrderingWithInitialTransition()
        {
            var expectedOrdering = new List<string> { "OnExitA", "OnTransitionedAB", "OnEntryB", "OnTransitionedBC", "OnEntryC", "OnTransitionCompletedAC" };
            var actualOrdering = new List<string>();

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B)
                .OnExit(() => actualOrdering.Add("OnExitA"));

            sm.Configure(State.B)
                .InitialTransition(State.C)
                .OnEntry(() => actualOrdering.Add("OnEntryB"));

            sm.Configure(State.C)
                .SubstateOf(State.B)
                .OnEntry(() => actualOrdering.Add("OnEntryC"));

            sm.OnTransitionedAsync(t => UniTask.Create(async () => actualOrdering.Add($"OnTransitioned{t.Source}{t.Destination}")));
            sm.OnTransitionCompletedAsync(t => UniTask.Create(async () => actualOrdering.Add($"OnTransitionCompleted{t.Source}{t.Destination}")));

            // await so that the async call completes before asserting anything
            await sm.FireAsync(Trigger.X);
            Assert.AreEqual(State.C, sm.State);

            Assert.AreEqual(expectedOrdering.Count, actualOrdering.Count);
            for (int i = 0; i < expectedOrdering.Count; i++)
            {
                Assert.AreEqual(expectedOrdering[i], actualOrdering[i]);
            }
        }
#endif
    }
}
