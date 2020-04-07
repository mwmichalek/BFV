using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BFV.Components;
using BFV.Components.Thermocouples;
using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using PubSub;
using Moq;
using System.Linq;
using Microsoft.Extensions.Logging;
using BFV.Services.Hub;

namespace BFV.Test.Components {
    public class SimulationThermocoupleTests : SimulatedApplianceBaseTests {

        [Fact]
        public void SsrChangeTriggersThermocoupleChangeAfterRefresh() {

            var Utils = CreateUtils(Location.HLT, new ThermocoupleState {
                Temperature = Temperature.RoomTemp
            });

            ComponentStateChange<ThermocoupleState> thermocoupleChange = null;

            Utils.MockHub.Setup(hb => hb.Publish<ComponentStateChange<ThermocoupleState>>(It.IsAny<ComponentStateChange<ThermocoupleState>>()))
                         .Callback<ComponentStateChange<ThermocoupleState>>((chg) => thermocoupleChange = chg);

            Utils.Thermocouple.ComponentStateChangePublisher(Utils.Hub.Publish<ComponentStateChange<ThermocoupleState>>);

            Utils.Thermocouple.ComponentStateChangeOccurred(new ComponentStateChange<SsrState> {
                Location = Location.HLT,
                CurrentState = new SsrState {
                    Percentage = 50
                }
            });
            Utils.Thermocouple.Refresh();

            Assert.True(thermocoupleChange != null, "ComponentStateChange<ThermocoupleState> was never published");
            Assert.True(thermocoupleChange.Location == Location.HLT, "Ssr change Location is not HLT");
            Assert.True(thermocoupleChange.CurrentState.Temperature > Temperature.RoomTemp, "Thermocouple temperature didn't raise above RoomTemp");
        }

        private (Mock<IHub> MockHub, IHub Hub, SimulationThermocouple Thermocouple) CreateUtils(Location location, ThermocoupleState state) {
            var logger = new Mock<ILogger<Thermocouple>>();
            var mockHub = new Mock<IHub>();
            var hub = mockHub.Object;

            var thermo = new SimulationThermocouple(logger.Object);
            thermo.CurrentState = state;
            thermo.Location = location;
            return (mockHub, hub, thermo);
        }
    }
}
