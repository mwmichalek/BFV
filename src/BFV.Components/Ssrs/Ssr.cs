using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BFV.Components.Ssrs {

    public interface ISsr : ILocatableComponent,
                            IComponentStateChangePublisher<SsrState>,
                            IComponentStateRequestSubscriber<SsrState> {
    }

    public class SimulationSsr : Ssr {

        public SimulationSsr(ILogger<Ssr> logger) : base(logger) { }

    }

    public abstract class Ssr : StateComponent<SsrState>,
                                ISsr {

        protected readonly ILogger<Ssr> _logger;

        private int _dutyCycleInMillis = 2000;
        private int _millisOn = 0;
        private int _millisOff = 2000;

        private Action<ComponentStateChange<SsrState>> _publishSsrStateChanged;

        public Ssr(ILogger<Ssr> logger) {
            _logger = logger;
        }

        public Location Location { get; set; }

        public virtual void ComponentStateRequestOccurred(ComponentStateRequest<SsrState> stateRequest) {
            if (stateRequest.Location == Location) {
                PriorState = CurrentState;
                CurrentState = stateRequest.UpdateState(CurrentState.Clone());

                var publishChanges = false;
                if (PriorState.Percentage != CurrentState.Percentage) {
                    CalculateDurations();
                    publishChanges = true;
                }

                if (!PriorState.IsEngaged && CurrentState.IsEngaged) {
                    Start();
                    publishChanges = true;
                }

                if (publishChanges)
                    _publishSsrStateChanged(CreateComponentStateChange());
            }
        }

        public virtual void ComponentStateChangePublisher(Action<ComponentStateChange<SsrState>> publishStateChange) {
            _publishSsrStateChanged = publishStateChange;
        }

        private void CalculateDurations() {
            // Calculate On and Off durations

            decimal fraction = ((decimal)CurrentState.Percentage / 100.0m);
            _millisOn = (int)(fraction * (decimal)_dutyCycleInMillis);
            _millisOff = _dutyCycleInMillis - _millisOn;

            //Logger.LogInformation($"SSR: {Id} - CALC PERC {CurrentState.Percentage}, FRACTION {fraction}, MILLISON {_millisOn}, MILLISOFF {_millisOff}");
        }

        private void Start() {
            Task.Run(() => Run());
        }

        private void Run() {
            while (CurrentState.IsEngaged) {
                // Something is causing a random blip.
                if (CurrentState.Percentage != 0 && _millisOn > 0) {
                    Toggle(true);
                    Thread.Sleep(_millisOn);
                }
                if (CurrentState.Percentage != 100 && _millisOff > 0) {
                    Toggle(false);
                    Thread.Sleep(_millisOff);
                }
            }
        }

        protected virtual void Toggle(bool isOn) {
            PriorState = CurrentState;
            CurrentState = CurrentState.Clone();
            CurrentState.IsFiring = isOn;
            _publishSsrStateChanged(CreateComponentStateChange());
        }
    }

    public enum SsrPin {
        HLT = 23, //4,
        BK = 24 //5
    }

}












