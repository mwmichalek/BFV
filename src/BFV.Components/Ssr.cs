using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components {
    public class Ssr : StateComponent<SsrState>,
                       ILocatableComponent,
                       IComponentStateChangePublisher<SsrState>,
                       IComponentStateChangeSubscriber<PidState> {

        private readonly ILogger _logger;

        private Action<ComponentStateChange<PidState>> _publishPidStateChange;

        public Ssr(ILogger logger) {
            _logger = logger;
        }

        public Location Location { get; set; }

        public void ComponentStateChangeOccurred(ComponentStateChange<PidState> stateChange) {
            throw new NotImplementedException();
        }

        public void ComponentStateChangePublisher(Action<ComponentStateChange<SsrState>> publishStateChange) {
            throw new NotImplementedException();
        }
    }
}
