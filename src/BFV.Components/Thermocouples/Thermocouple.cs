﻿using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using System;
using Microsoft.Extensions.Logging;

namespace BFV.Components.Thermocouples {

    public interface IThermocouple : IComponentStateChangePublisher<ThermocoupleState>,
                                     ILocatableComponent,
                                     IRefreshableComponent {

    }



    public class Thermocouple : StateComponent<ThermocoupleState>,
                                IThermocouple {

        protected readonly ILogger<Thermocouple> _logger;

        public Location Location { get; set; }

        protected Action<ComponentStateChange<ThermocoupleState>> _publishThermocoupleStateChange;

        public Thermocouple(ILogger<Thermocouple> logger) {
            _logger = logger;
        }

        public void ComponentStateChangePublisher(Action<ComponentStateChange<ThermocoupleState>> publishStateChange) {
            _publishThermocoupleStateChange = publishStateChange;
        }

        public virtual void Refresh() {
            // Read hardware
        }

       
    }
}
