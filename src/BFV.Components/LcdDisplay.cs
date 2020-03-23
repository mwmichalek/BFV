using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components {
    public class LcdDisplay : IComponent, IComponentStateChangeSubscriber<ComponentStateChange<PidState>> {

        private readonly ILogger _logger;

        public LcdDisplay(ILogger logger) {
            _logger = logger;
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<PidState> stateChange) {
            _logger.Debug($"PidStateChangeOccured: {stateChange.Location}");
        }
    }
}
