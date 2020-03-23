using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components {
    public class Pid : Component<PidState>, IComponentStateChangePublisher<ComponentStateChange<PidState>> {

        private readonly ILogger _logger;

        private Action<ComponentStateChange<PidState>> _publishPidStateChange;

        public Pid(ILogger logger) {
            _logger = logger;
        }
        
        public void ComponentStateChangePublisher(Action<ComponentStateChange<PidState>> publishStateChange) {
            _publishPidStateChange = publishStateChange;
        }

        public void Test() {
            _publishPidStateChange(new ComponentStateChange<PidState> {
                PriorState = new PidState(),
                CurrentState = new PidState()
            });
        }
    }

    public enum PidMode {
        Temperature,
        Percentage,
        Unknown
    }

}
