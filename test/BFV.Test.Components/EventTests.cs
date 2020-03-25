using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using PubSub;
using Serilog;
using SimpleInjector;
using System;
using System.Linq;
using Xunit;

namespace BFV.Test.Components {
    public class EventTests {
        [Fact]
        public void CorrectPidAlertedWhenTemperatureChanges() {

            var container = ComponentRegistrator.ComponentRegistry()
                                                .RegisterThermos<RandomFakedThermocouple>()
                                                .RegisterPids<TestPid>();

            var thermo = container.GetInstance<Thermocouple>(Location.HLT);
            TestPid correctPid = (TestPid)container.GetInstance<Pid>(Location.HLT);
            TestPid incorrectPid = (TestPid)container.GetInstance<Pid>(Location.BK);

            thermo.Refresh();

            Assert.True(correctPid.ThermocoupleStateChangeOccured);
            Assert.False(incorrectPid.ThermocoupleStateChangeOccured);
        }


        public class TestPid : Pid {

            public bool ThermocoupleStateChangeOccured { get; set; }

            public TestPid(ILogger logger) : base(logger) { }

            public override void ComponentStateChangeOccurred(ComponentStateChange<ThermocoupleState> stateChange) {
                if (stateChange.Location == Location)
                    ThermocoupleStateChangeOccured = true;
            }

        }

    }
}
