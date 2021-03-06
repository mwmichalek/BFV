﻿using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.Displays {
    public abstract class Display : IComponent, 
                                    IComponentStateChangeSubscriber<PidState>,
                                    IComponentStateChangeSubscriber<ThermocoupleState> {

        private readonly ILogger<Display> _logger;

        public Display(ILogger<Display> logger) {
            _logger = logger;
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<PidState> stateChange) {
            //_logger.Debug($"PidStateChangeOccured: {stateChange.Location}");
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<ThermocoupleState> stateChange) {
            //_logger.Debug($"ThermometerStateChangeOccured: {stateChange.Location}");
        }
    }
}
