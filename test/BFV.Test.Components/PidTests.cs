using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BFV.Components;
using BFV.Components.Thermocouples;
using BFV.Test.Components.TestComponents;
using BFV.Common;

namespace BFV.Test.Components {
    public class PidTests {

        [Fact]
        public void CorrectPidAlertedWhenTemperatureChanges() {
            var container = ComponentRegistrator.ComponentRegistry()
                                                .RegisterLogging()
                                                .RegisterThermos<RandomFakedThermocouple>()
                                                .RegisterPids<TestPid>();

            var thermo = container.GetInstance<Thermocouple>(Location.HLT);
            TestPid correctPid = (TestPid)container.GetInstance<Pid>(Location.HLT);
            TestPid incorrectPid = (TestPid)container.GetInstance<Pid>(Location.BK);

            thermo.Refresh();

            Assert.True(correctPid.ThermocoupleStateChangeOccured);
            Assert.False(incorrectPid.ThermocoupleStateChangeOccured);

            container.DeregisterAllComponents();
        }

    }
}
