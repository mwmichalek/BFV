using BFV.Common.Events;
using BFV.Components.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.Thermocouples {
    public class SimulationThermocouple : Thermocouple,
                                          IComponentStateChangeSubscriber<SsrState> {

        private const double COOLING_RATIO = 0.1;

        public double TemperatureChange { get; set; } = 0;

        public SimulationThermocouple(ILogger<Thermocouple> logger) : base(logger) {
            CurrentState = new ThermocoupleState { Temperature = Temperature.RoomTemp };
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<SsrState> stateChange) {
            if (stateChange.Location == Location) {
                double percentage = (double)stateChange.CurrentState.Percentage / 100d;
                TemperatureChange = percentage - COOLING_RATIO;  // Anything below 10% decreases water temperature.
            }
        }

        public override void Refresh() {
            var newTemp = CurrentState.Temperature + TemperatureChange;
            if (newTemp < Temperature.RoomTemp) newTemp = Temperature.RoomTemp;
            if (newTemp > Temperature.BoilingTemp) newTemp = Temperature.BoilingTemp;
            
            // Only update if there is a temp change
            if (newTemp != CurrentState.Temperature) {
                PriorState = CurrentState;
                CurrentState = new ThermocoupleState {
                    Temperature = newTemp
                };
                //_logger.LogInformation($"Thermo: {Location} {CurrentState}");
                _publishThermocoupleStateChange(this.CreateComponentStateChange());
            }
        }
    }
}
