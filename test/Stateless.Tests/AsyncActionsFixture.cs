#if TASKS
#pragma warning disable CS1998
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;


namespace Stateless.Tests
{
    public class AsyncActionsFixture
    {
        [Test]
        public void StateMutatorShouldBeCalledOnlyOnce()
        {
            var state = State.B;
            var count = 0;
            var sm = new StateMachine<State, Trigger>(() => state, (s) => { state = s; count++; });
            sm.Configure(State.B).Permit(Trigger.X, State.C);
            sm.FireAsync(Trigger.X);
            Assert.AreEqual(1, count);
        }
        
        [Test]
        public async Task SuperStateShouldNotExitOnSubStateTransition_WhenUsingAsyncTriggers()
        {
            // Arrange.
            var sm = new StateMachine<State, Trigger>(State.A);
            var record = new List<string>();

            sm.Configure(State.A)
                .OnEntryAsync(() => UniTask.Create(async () => record.Add("Entered state A")))
                .OnExitAsync(() => UniTask.Create(async () => record.Add("Exited state A")))
                .Permit(Trigger.X, State.B);
            
            sm.Configure(State.B) // Our super state.
                .InitialTransition(State.C)
                .OnEntryAsync(() => UniTask.Create(async () => record.Add("Entered super state B")))
                .OnExitAsync(() => UniTask.Create(async () => record.Add("Exited super state B")));

            sm.Configure(State.C) // Our first sub state.
                .OnEntryAsync(() => UniTask.Create(async () => record.Add("Entered sub state C")))
                .OnExitAsync(() => UniTask.Create(async () => record.Add("Exited sub state C")))
                .Permit(Trigger.Y, State.D)
                .SubstateOf(State.B);
            sm.Configure(State.D) // Our second sub state.
                .OnEntryAsync(() => UniTask.Create(async () => record.Add("Entered sub state D")))
                .OnExitAsync(() => UniTask.Create(async () => record.Add("Exited sub state D")))
                .SubstateOf(State.B);

            
            // Act.
            await sm.FireAsync(Trigger.X);
            await sm.FireAsync(Trigger.Y);
            
            // Assert.
            Assert.AreEqual("Exited state A", record[0]);
            Assert.AreEqual("Entered super state B", record[1]);
            Assert.AreEqual("Entered sub state C", record[2]);
            Assert.AreEqual("Exited sub state C", record[3]);
            Assert.AreEqual("Entered sub state D", record[4]); // Before the patch the actual result was "Exited super state B"
        }

        [Test]
        public void SuperStateShouldNotExitOnSubStateTransition_WhenUsingSyncTriggers()
        {
            // Arrange.
            var sm = new StateMachine<State, Trigger>(State.A);
            var record = new List<string>();

            sm.Configure(State.A)
                .OnEntry(() => record.Add("Entered state A"))
                .OnExit(() => record.Add("Exited state A"))
                .Permit(Trigger.X, State.B);
            
            sm.Configure(State.B) // Our super state.
                .InitialTransition(State.C)
                .OnEntry(() => record.Add("Entered super state B"))
                .OnExit(() => record.Add("Exited super state B"));

            sm.Configure(State.C) // Our first sub state.
                .OnEntry(() => record.Add("Entered sub state C"))
                .OnExit(() => record.Add("Exited sub state C"))
                .Permit(Trigger.Y, State.D)
                .SubstateOf(State.B);
            sm.Configure(State.D) // Our second sub state.
                .OnEntry(() => record.Add("Entered sub state D"))
                .OnExit(() => record.Add("Exited sub state D"))
                .SubstateOf(State.B);

            
            // Act.
            sm.Fire(Trigger.X);
            sm.Fire(Trigger.Y);
            
            // Assert.
            Assert.AreEqual("Exited state A", record[0]);
            Assert.AreEqual("Entered super state B", record[1]);
            Assert.AreEqual("Entered sub state C", record[2]);
            Assert.AreEqual("Exited sub state C", record[3]);
            Assert.AreEqual("Entered sub state D", record[4]);
        }
        
        [Test]
        public async Task CanFireAsyncEntryAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            var test = "";
            sm.Configure(State.B)
              .OnEntryAsync(() => UniTask.Create(async () => test = "foo"));

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual("foo", test); // Should await action
            Assert.AreEqual(State.B, sm.State); // Should transition to destination state
        }

        [Test]
        public void WhenSyncFireAsyncEntryAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            sm.Configure(State.B)
              .OnEntryAsync(() => TaskResult.Done);

            Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
        }

        [Test]
        public async Task CanFireAsyncExitAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            var test = "";
            sm.Configure(State.A)
              .OnExitAsync(() => UniTask.Create(async () => test = "foo"))
              .Permit(Trigger.X, State.B);

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual("foo", test); // Should await action
            Assert.AreEqual(State.B, sm.State); // Should transition to destination state
        }

        [Test]
        public void WhenSyncFireAsyncExitAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .OnExitAsync(() => TaskResult.Done)
              .Permit(Trigger.X, State.B);

            Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
        }

        [Test]
        public async Task CanFireInternalAsyncAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            var test = "";
            sm.Configure(State.A)
              .InternalTransitionAsync(Trigger.X, () => UniTask.Create(async () => test = "foo"));

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual("foo", test); // Should await action
        }

        [Test]
        public void WhenSyncFireInternalAsyncAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .InternalTransitionAsync(Trigger.X, () => TaskResult.Done);

            Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
        }

        [Test]
        public async Task CanInvokeOnTransitionedAsyncAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            var test = "";
            sm.OnTransitionedAsync(_ => UniTask.Create(async () => test = "foo"));

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual("foo", test); // Should await action
        }

        [Test]
        public async Task CanInvokeOnTransitionCompletedAsyncAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            var test = "";
            sm.OnTransitionCompletedAsync(_ => UniTask.Create(async () => test = "foo"));

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual("foo", test); // Should await action
        }

        [Test]
        public async Task WillInvokeSyncOnTransitionedIfRegisteredAlongWithAsyncAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            var test1 = "";
            var test2 = "";
            sm.OnTransitioned(_ => test1 = "foo1");
            sm.OnTransitionedAsync(_ => UniTask.Create(async () => test2 = "foo2"));

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual("foo1", test1);
            Assert.AreEqual("foo2", test2);
        }

        [Test]
        public async Task WillInvokeSyncOnTransitionCompletedIfRegisteredAlongWithAsyncAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            var test1 = "";
            var test2 = "";
            sm.OnTransitionCompleted(_ => test1 = "foo1");
            sm.OnTransitionCompletedAsync(_ => UniTask.Create(async () => test2 = "foo2"));

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual("foo1", test1);
            Assert.AreEqual("foo2", test2);
        }

        [Test]
        public void WhenSyncFireAsyncOnTransitionedAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            sm.OnTransitionedAsync(_ => TaskResult.Done);

            Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
        }

        [Test]
        public void WhenSyncFireAsyncOnTransitionCompletedAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            sm.OnTransitionCompletedAsync(_ => TaskResult.Done);

            Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
        }

        [Test]
        public async Task CanInvokeOnUnhandledTriggerAsyncAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            var test = "";
            sm.OnUnhandledTriggerAsync((s, t, u) => UniTask.Create(async () => test = "foo"));

            await sm.FireAsync(Trigger.Z);

            Assert.AreEqual("foo", test); // Should await action
        }
        [Test]
        public void WhenSyncFireOnUnhandledTriggerAsyncTask()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B);

            sm.OnUnhandledTriggerAsync((s, t) => TaskResult.Done);

            Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.Z));
        }
        [Test]
        public void WhenSyncFireOnUnhandledTriggerAsyncAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .Permit(Trigger.X, State.B);

            sm.OnUnhandledTriggerAsync((s, t, u) => TaskResult.Done);

            Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.Z));
        }

        [Test]
        public async Task WhenActivateAsync()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            var activated = false;
            sm.Configure(State.A)
              .OnActivateAsync(() => UniTask.Create(async () => activated = true));

            await sm.ActivateAsync();

            Assert.AreEqual(true, activated); // Should await action
        }

        [Test]
        public async Task WhenDeactivateAsync()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            var deactivated = false;
            sm.Configure(State.A)
              .OnDeactivateAsync(() => UniTask.Create(async () => deactivated = true));

            await sm.ActivateAsync();
            await sm.DeactivateAsync();

            Assert.AreEqual(true, deactivated); // Should await action
        }

        [Test]
        public void WhenSyncActivateAsyncOnActivateAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .OnActivateAsync(() => TaskResult.Done);

            Assert.Throws<InvalidOperationException>(() => sm.Activate());
        }

        [Test]
        public void WhenSyncDeactivateAsyncOnDeactivateAction()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
              .OnDeactivateAsync(() => TaskResult.Done);

            sm.Activate();

            Assert.Throws<InvalidOperationException>(() => sm.Deactivate());
        }
        [Test]
        public async Task IfSelfTransitionPermited_ActionsFire_InSubstate_async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            bool onEntryStateBfired = false;
            bool onExitStateBfired = false;
            bool onExitStateAfired = false;

            sm.Configure(State.B)
                .OnEntryAsync(t => UniTask.Create(async () => onEntryStateBfired = true))
                .PermitReentry(Trigger.X)
                .OnExitAsync(t => UniTask.Create(async () => onExitStateBfired = true));

            sm.Configure(State.A)
                .SubstateOf(State.B)
                .OnExitAsync(t => UniTask.Create(async () => onExitStateAfired = true));

            await sm.FireAsync(Trigger.X);

            Assert.AreEqual(State.B, sm.State);
            Assert.True(onExitStateAfired);
            Assert.True(onExitStateBfired);
            Assert.True(onEntryStateBfired);
        }

        [Test]
        public async Task TransitionToSuperstateDoesNotExitSuperstate()
        {
            StateMachine<State, Trigger> sm = new StateMachine<State, Trigger>(State.B);

            bool superExit = false;
            bool superEntry = false;
            bool subExit = false;

            sm.Configure(State.A)
                .OnEntryAsync(t => UniTask.Create(async () => superEntry = true))
                .OnExitAsync(t => UniTask.Create(async () => superExit = true));

            sm.Configure(State.B)
                .SubstateOf(State.A)
                .Permit(Trigger.Y, State.A)
                .OnExitAsync(t => UniTask.Create(async () => subExit = true));

            await sm.FireAsync(Trigger.Y);

            Assert.True(subExit);
            Assert.False(superEntry);
            Assert.False(superExit);
        }

        [Test]
        public async Task IgnoredTriggerMustBeIgnoredAsync()
        {
            bool nullRefExcThrown = false;
            var stateMachine = new StateMachine<State, Trigger>(State.B);
            stateMachine.Configure(State.A)
                .Permit(Trigger.X, State.C);

            stateMachine.Configure(State.B)
                .SubstateOf(State.A)
                .Ignore(Trigger.X);

            try
            {
                // >>> The following statement should not throw a NullReferenceException
                await stateMachine.FireAsync(Trigger.X);
            }
            catch (NullReferenceException )
            {
                nullRefExcThrown = true;
            }

            Assert.False(nullRefExcThrown);
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

            sm.FireAsync(Trigger.X);

            Assert.AreEqual(State.D, sm.State);
        }

        [Test]
        public void OnEntryFromAsync_WhenTriggeredSynchronously_Throws()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .OnEntryFromAsync(Trigger.X, async () => await UniTask.Create(async () => { }));

            Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
        }

        [Test]
        public async Task OnEntryFromAsync_WhenTriggered_InvokesAction()
        {
            bool wasInvoked = false;

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A).Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .OnEntryFromAsync(Trigger.X, async () => await UniTask.Create(async () => { wasInvoked = true; }));

            await sm.FireAsync(Trigger.X);

            Assert.True(wasInvoked);
        }

        [Test]
        public void OnEntryFromAsync_WhenEnteringByAnotherTriggerSynchronously_DoesNotThrow()
        {
            bool wasInvoked = false;

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B)
                .Permit(Trigger.Y, State.B);

            sm.Configure(State.B)
                .OnEntryFromAsync(Trigger.X, async () => await UniTask.Create(async () => { wasInvoked = true; }));

            sm.Fire(Trigger.Y);

            Assert.False(wasInvoked);
        }

        [Test]
        public async Task OnEntryFromAsync_WhenEnteringByAnotherTrigger_InvokesAction()
        {
            bool wasInvoked = false;

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B)
                .Permit(Trigger.Y, State.B);

            sm.Configure(State.B)
                .OnEntryFromAsync(Trigger.X, async () => await UniTask.Create(async () => { wasInvoked = true; }));

            await sm.FireAsync(Trigger.Y);

            Assert.False(wasInvoked);
        }

        [Test]
        public async Task FireAsyncTriggerWithParametersArray()
        {
            const string expectedParam = "42-Stateless-True-123.45-Y";
            string actualParam = null;

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .OnEntryAsync(t =>
                {
                    actualParam = string.Join("-", t.Parameters.Select(x => string.Format(CultureInfo.InvariantCulture, "{0}", x)));
                    return UniTask.CompletedTask;
                });

            await sm.FireAsync(Trigger.X, 42, "Stateless", true, 123.45, Trigger.Y);

            Assert.AreEqual(expectedParam, actualParam);
        }

        [Test]
        public async Task FireAsync_TriggerWithMoreThanThreeParameters()
        {
            const string expectedParam = "42-Stateless-True-123.45-Y";
            string actualParam = null;

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B);

            sm.Configure(State.B)
                .OnEntryAsync(t =>
                {
                    actualParam = string.Join("-", t.Parameters.Select(x => string.Format(CultureInfo.InvariantCulture, "{0}", x)));
                    return UniTask.CompletedTask;
                });

            var parameterizedX = sm.SetTriggerParameters(Trigger.X, typeof(int), typeof(string), typeof(bool), typeof(double), typeof(Trigger));

            await sm.FireAsync(parameterizedX, 42, "Stateless", true, 123.45, Trigger.Y);

            Assert.AreEqual(expectedParam, actualParam);
        }

        [Test]
        public async Task WhenInSubstate_TriggerSuperStateTwiceToSameSubstate_DoesNotReenterSubstate_Async()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var eCount = 0;

            sm.Configure(State.B)
                .OnEntry(() => { eCount++; })
                .SubstateOf(State.C);

            sm.Configure(State.A)
                .SubstateOf(State.C);

            sm.Configure(State.C)
                .Permit(Trigger.X, State.B);

            await sm.FireAsync(Trigger.X);
            await sm.FireAsync(Trigger.X);

            Assert.AreEqual(1, eCount);
        }
    }
}

#endif
