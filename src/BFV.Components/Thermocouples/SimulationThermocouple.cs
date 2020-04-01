using BFV.Common.Events;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.Thermocouples {
    public class SimulationThermocouple : Thermocouple,
                                          IComponentStateChangeSubscriber<SsrState> {

        public double TemperatureChange { get; set; } = 0;

        public SimulationThermocouple(ILogger logger) : base(logger) {
            CurrentState = new ThermocoupleState { Temperature = 70 };

            // Spawn thread
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<SsrState> stateChange) {
            var percentage = stateChange.CurrentState.Percentage / 100;
            TemperatureChange = percentage - 0.1;  // Anything below 10% decreases water temperature.
        }

        public override void Refresh() {
            PriorState = CurrentState;
            CurrentState = new ThermocoupleState {
                Temperature = PriorState.Temperature + TemperatureChange
            };

            _publishThermocoupleStateChange(this.CreateComponentStateChange());
        }
    }
}
