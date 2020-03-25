using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components {
    public class Pid : StateComponent<PidState>, 
                       ILocatableComponent, 
                       IComponentStateChangePublisher<PidState>,
                       IComponentStateChangeSubscriber<ThermocoupleState> {

        private readonly ILogger _logger;

        private Action<ComponentStateChange<PidState>> _publishPidStateChange;

        public Pid(ILogger logger) {
            _logger = logger;
        }

        public Location Location { get; set; }

        public virtual void ComponentStateChangeOccurred(ComponentStateChange<ThermocoupleState> stateChange) {
            if (stateChange.Location == Location) {
                _logger.Information($"Need to recalculate value for Pid: {Location}");
            }
        }

        public virtual void ComponentStateChangePublisher(Action<ComponentStateChange<PidState>> publishStateChange) {
            _publishPidStateChange = publishStateChange;
        }

        //public void Test() {
        //    _publishPidStateChange(new ComponentStateChange<PidState> {
        //        Location = Location,
        //        PriorState = new PidState(),
        //        CurrentState = new PidState()
        //    });
        //}
    }

    public enum PidMode {
        Temperature,
        Percentage,
        Unknown
    }

}
