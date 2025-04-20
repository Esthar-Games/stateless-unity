#if TASKS
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Stateless.Tests
{
    public class InternalTransitionAsyncFixture
    {
        [Test]
        public async Task InternalTransitionAsyncIf_AllowGuardWithParameter()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int>(Trigger.X);
            const int intParam = 5;
            var guardInvoked = false;
            var callbackInvoked = false;

            sm.Configure(State.A)
                .InternalTransitionAsyncIf(trigger, i =>
                {
                    guardInvoked = true;
                    Assert.AreEqual(intParam, i);
                    return true;
                }, (i, transition) =>
                {
                    callbackInvoked = true;
                    Assert.AreEqual(intParam, i);
                    return UniTask.CompletedTask;
                });

            await sm.FireAsync(trigger, intParam);

            Assert.True(guardInvoked);
            Assert.True(callbackInvoked);
        }
    
        [Test]
        public async Task InternalTransitionAsyncIf_AllowGuardWithTwoParameters()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int, string>(Trigger.X);
            const int intParam = 5;
            const string stringParam = "5";
            var guardInvoked = false;
            var callbackInvoked = false;

            sm.Configure(State.A)
                .InternalTransitionAsyncIf(trigger, (i, s) =>
                {
                    guardInvoked = true;
                    Assert.AreEqual(intParam, i);
                    Assert.AreEqual(stringParam, s);
                    return true;
                }, (i, s, transition) =>
                {
                    callbackInvoked = true;
                    Assert.AreEqual(intParam, i);
                    Assert.AreEqual(stringParam, s);
                    return UniTask.CompletedTask;
                });

            await sm.FireAsync(trigger, intParam, stringParam);

            Assert.True(guardInvoked);
            Assert.True(callbackInvoked);
        }
    
        [Test]
        public async Task InternalTransitionAsyncIf_AllowGuardWithThreeParameters()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int, string, bool>(Trigger.X);
            const int intParam = 5;
            const string stringParam = "5";
            const bool boolParam = true;
            var guardInvoked = false;
            var callbackInvoked = false;

            sm.Configure(State.A)
                .InternalTransitionAsyncIf(trigger, (i, s, b) =>
                {
                    guardInvoked = true;
                    Assert.AreEqual(intParam, i);
                    Assert.AreEqual(stringParam, s);
                    Assert.AreEqual(boolParam, b);
                    return true;
                }, (i, s, b, transition) =>
                {
                    callbackInvoked = true;
                    Assert.AreEqual(intParam, i);
                    Assert.AreEqual(stringParam, s);
                    Assert.AreEqual(boolParam, b);
                    return UniTask.CompletedTask;
                });

            await sm.FireAsync(trigger, intParam, stringParam, boolParam);

            Assert.True(guardInvoked);
            Assert.True(callbackInvoked);
        }

        [Test]
        [Obsolete]
        public async Task InternalTransitionAsyncIf_DeprecatedOverload_AllowGuardWithoutParameter()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int>(Trigger.X);
            const int intParam = 5;
            var guardInvoked = false;
            var callbackInvoked = false;

            sm.Configure(State.A)
                .InternalTransitionAsyncIf(trigger, () =>
                {
                    guardInvoked = true;
                    return true;
                }, (i, transition) =>
                {
                    callbackInvoked = true;
                    Assert.AreEqual(intParam, i);
                    return UniTask.CompletedTask;
                });

            await sm.FireAsync(trigger, intParam);

            Assert.True(guardInvoked);
            Assert.True(callbackInvoked);
        }

        [Test]
        [Obsolete]
        public async Task InternalTransitionAsyncIf_DeprecatedOverload_AllowGuardWithParameter()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int>(Trigger.X);
            const int intParam = 5;
            var guardInvoked = false;
            var callbackInvoked = false;

            sm.Configure(State.A)
                .InternalTransitionAsyncIf(trigger, () =>
                {
                    guardInvoked = true;
                    return true;
                }, (i, transition) =>
                {
                    callbackInvoked = true;
                    Assert.AreEqual(intParam, i);
                    return UniTask.CompletedTask;
                });

            await sm.FireAsync(trigger, intParam);

            Assert.True(guardInvoked);
            Assert.True(callbackInvoked);
        }

        [Test]
        [Obsolete]
        public async Task InternalTransitionAsyncIf_DeprecatedOverload_AllowGuardWithTwoParameters()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int, string>(Trigger.X);
            const int intParam = 5;
            const string stringParam = "5";
            var guardInvoked = false;
            var callbackInvoked = false;

            sm.Configure(State.A)
                .InternalTransitionAsyncIf(trigger, () =>
                {
                    guardInvoked = true;
                    return true;
                }, (i, s, transition) =>
                {
                    callbackInvoked = true;
                    Assert.AreEqual(intParam, i);
                    Assert.AreEqual(stringParam, s);
                    return UniTask.CompletedTask;
                });

            await sm.FireAsync(trigger, intParam, stringParam);

            Assert.True(guardInvoked);
            Assert.True(callbackInvoked);
        }

        [Test]
        [Obsolete]
        public async Task InternalTransitionAsyncIf_DeprecatedOverload_AllowGuardWithThreeParameters()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int, string, bool>(Trigger.X);
            const int intParam = 5;
            const string stringParam = "5";
            const bool boolParam = true;
            var guardInvoked = false;
            var callbackInvoked = false;

            sm.Configure(State.A)
                .InternalTransitionAsyncIf(trigger, () =>
                {
                    guardInvoked = true;
                    return true;
                }, (i, s, b, transition) =>
                {
                    callbackInvoked = true;
                    Assert.AreEqual(intParam, i);
                    Assert.AreEqual(stringParam, s);
                    Assert.AreEqual(boolParam, b);
                    return UniTask.CompletedTask;
                });

            await sm.FireAsync(trigger, intParam, stringParam, boolParam);

            Assert.True(guardInvoked);
            Assert.True(callbackInvoked);
        }

        [Test]
        [Obsolete]
        public async Task InternalTransitionAsyncIf_DeprecatedOverload_GuardExecutedOnlyOnce()
        {
            var guardCalls = 0;
            var order = new Order
            {
                Status = OrderStatus.OrderPlaced,
                PaymentStatus = PaymentStatus.Pending,
            };
            var stateMachine = new StateMachine<OrderStatus, OrderStateTrigger>(order.Status);
            stateMachine.Configure(OrderStatus.OrderPlaced)
                 .InternalTransitionAsyncIf<int>(OrderStateTrigger.PaymentCompleted,
                       () => PreCondition(ref guardCalls),
                       _ => ChangePaymentState(order, PaymentStatus.Completed));

            await stateMachine.FireAsync(OrderStateTrigger.PaymentCompleted);

            Assert.AreEqual(1, guardCalls);
        }

        /// <summary>
        /// This unit test demonstrated bug report #417
        /// </summary>
        [Test]
        public async Task InternalTransitionAsyncIf_GuardExecutedOnlyOnce()
        {
            var guardCalls = 0;
            var order = new Order
            {
                Status = OrderStatus.OrderPlaced,
                PaymentStatus = PaymentStatus.Pending,
            };
            var stateMachine = new StateMachine<OrderStatus, OrderStateTrigger>(order.Status);
            stateMachine.Configure(OrderStatus.OrderPlaced)
                 .InternalTransitionAsyncIf(OrderStateTrigger.PaymentCompleted,
                       () => PreCondition(ref guardCalls),
                       () => ChangePaymentState(order, PaymentStatus.Completed));

            await stateMachine.FireAsync(OrderStateTrigger.PaymentCompleted);

            Assert.AreEqual(1, guardCalls);
        }

        private bool PreCondition(ref int calls)
        {
            calls++;
            return true;
        }

        private async UniTask ChangePaymentState(Order order, PaymentStatus paymentStatus)
        {
            await UniTask.FromResult(order.PaymentStatus = paymentStatus);
        }

        private enum OrderStatus { OrderPlaced }
        private enum PaymentStatus { Pending, Completed }
        private enum OrderStateTrigger { PaymentCompleted }
        private class Order
        {
            public OrderStatus Status { get; internal set; }
            public PaymentStatus PaymentStatus { get; internal set; }
        }
    }
}
#endif