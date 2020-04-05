using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFV.Components {

    public interface IPid : ILocatableComponent,
                            IRefreshableComponent,
                            IComponentStateChangePublisher<PidState>,
                            IComponentStateRequestSubscriber<PidState>,
                            IComponentStateRequestPublisher<PidState>,
                            IComponentStateChangeSubscriber<ThermocoupleState>,
                            IComponentStateRequestPublisher<SsrState> {

    }




    public class Pid : StateComponent<PidState>, 
                       IPid {

        public Location Location { get; set; }

        protected readonly ILogger<Pid> _logger;

        private Action<ComponentStateChange<PidState>> _publishPidStateChanged;

        private Action<ComponentStateRequest<PidState>> _publishPidDisengageRequest;

        private Action<ComponentStateRequest<SsrState>> _publishSsrRequest;

        private DateTime _lastRun;

        private bool _isRunning = false;

        private int _dutyCycleInMillis = 2000;

        private int _percentage = 0;

        public Pid(ILogger<Pid> logger) {
            _logger = logger;
        }

        

        public void Refresh() {
            if (PriorState.IsEngaged && !CurrentState.IsEngaged)
                UpdateSsr(0);

            if (CurrentState.IsEngaged) {
                if (CurrentState.PidMode == PidMode.Temperature && CurrentState.Temperature != double.MinValue) {
                    var currentTime = DateTime.Now;
                    if (_lastRun == null)
                        _lastRun = currentTime;


                    var secondsSinceLastUpdate = (currentTime - _lastRun).Seconds;
                    if (secondsSinceLastUpdate == 0) secondsSinceLastUpdate = 1;

                    double error = CurrentState.SetPoint - CurrentState.Temperature;

                    // integral term calculation
                    _integralTerm += (CurrentState.GainIntegral * error * secondsSinceLastUpdate);
                    _integralTerm = Clamp(_integralTerm);

                    // derivative term calculation
                    double dInput = CurrentState.Temperature - PriorState.Temperature;
                    double derivativeTerm = CurrentState.GainDerivative * (dInput / secondsSinceLastUpdate);

                    // proportional term calcullation
                    double proportionalTerm = CurrentState.GainProportional * error;

                    double output = proportionalTerm + _integralTerm - derivativeTerm;
                    output = Clamp(output);

                    _lastRun = currentTime;

                    UpdateSsr((int)output);

                } else if (CurrentState.PidMode == PidMode.Percentage) {
                    // If PidMode is Percentage, SetPoint is a Percentage
                    UpdateSsr((int)CurrentState.SetPoint);
                }
            }
        }

        public virtual void ComponentStateChangeOccurred(ComponentStateChange<ThermocoupleState> stateChange) {
            if (stateChange.Location == Location) {
                _logger.LogInformation($"Need to recalculate value for Pid: {Location}");
                PriorState = CurrentState;
                CurrentState = CurrentState.Clone();
                CurrentState.Temperature = stateChange.CurrentState.Temperature;
                Refresh();
            }
        }

        public virtual void ComponentStateRequestOccurred(ComponentStateRequest<PidState> stateRequest) {
            if (stateRequest.Location == Location) {
                PriorState = CurrentState;
                CurrentState = stateRequest.UpdateState(CurrentState.Clone());

                if (!PriorState.IsEngaged && CurrentState.IsEngaged && _publishPidDisengageRequest != null) {
                    foreach (var location in LocationHelper.PidLocations.Where(loc => loc != Location)) {
                        _publishPidDisengageRequest(new ComponentStateRequest<PidState> {
                            Location = location,
                            Updates = (disengageState) => {
                                disengageState.IsEngaged = false;
                            }
                        });
                    }
                }

                if (_publishPidStateChanged != null)
                    _publishPidStateChanged(CreateComponentStateChange());

                Refresh();
            }
        }

        private void UpdateSsr(int percentage) {
            //TODO: Come up with eligant way of testing to see if a publisher is set.
            if (_publishSsrRequest != null) {
                _publishSsrRequest(new ComponentStateRequest<SsrState> {
                    Location = Location,
                    Updates = (state) => {
                        state.Percentage = percentage;
                    }
                });
            } 
        }

        public virtual void ComponentStateChangePublisher(Action<ComponentStateChange<PidState>> publishStateChange) {
            _publishPidStateChanged = publishStateChange;
        }

        public virtual void ComponentStateRequestPublisher(Action<ComponentStateRequest<PidState>> publishStateRequest) {
            _publishPidDisengageRequest = publishStateRequest;
        }

        public void ComponentStateRequestPublisher(Action<ComponentStateRequest<SsrState>> publishStateRequest) {
            _publishSsrRequest = publishStateRequest;
        }


        /// <summary>
        /// The max output value the control device can accept.
        /// </summary>
        private double _outputMax = 100;

        /// <summary>
        /// The minimum ouput value the control device can accept.
        /// </summary>
        private double _outputMin = 0;

        /// <summary>
        /// Adjustment made by considering the accumulated error over time
        /// </summary>
        /// <remarks>
        /// An alternative formulation of the integral action, is the
        /// proportional-summation-difference used in discrete-time systems
        /// </remarks>
        private double _integralTerm = 0;

        /// <summary>
        /// Limit a variable to the set OutputMax and OutputMin properties
        /// </summary>
        /// <returns>
        /// A value that is between the OutputMax and OutputMin properties
        /// </returns>
        /// <remarks>
        /// Inspiration from http://stackoverflow.com/questions/3176602/how-to-force-a-number-to-be-in-a-range-in-c
        /// </remarks>
        private double Clamp(double variableToClamp) {
            if (variableToClamp <= _outputMin) { return _outputMin; }
            if (variableToClamp >= _outputMax) { return _outputMax; }
            return variableToClamp;
        }
    }

    public enum PidMode {
        Temperature,
        Percentage,
        Unknown
    }

}
