using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using BFV.Test.Components.TestComponents;
using PubSub;
using Serilog;
using SimpleInjector;
using System;
using System.Linq;
using Xunit;

namespace BFV.Test.Components {
    public class RandomFakedThermocoupleTests {

        [Fact]
        public void TemperatureChangeOccursOnRefresh() {

            using (var container = ComponentRegistrator.ComponentRegistry()
                                                       .RegisterThermos<RandomFakedThermocouple>()
                                                       .RegisterPids<TestPid>()) {

                var thermo = container.GetInstance<Thermocouple>(Location.HLT);
                TestPid pid = (TestPid)container.GetInstance<Pid>(Location.HLT);

                thermo.Refresh();

                Assert.NotNull(pid.LastThermocoupleStateChange);

                // Temperature changed
                Assert.NotEqual(pid.LastThermocoupleStateChange.PriorState.Temperature,
                                pid.LastThermocoupleStateChange.CurrentState.Temperature);

                // Timestamp changed
                Assert.NotEqual(pid.LastThermocoupleStateChange.PriorState.Timestamp,
                                pid.LastThermocoupleStateChange.CurrentState.Timestamp);

            }
        }

    }
}
