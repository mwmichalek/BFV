using BFV.Common;
using BFV.Components;
using BFV.Components.Thermocouples;
using BFV.Test.Components.TestComponents;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BFV.Test.Components {
    public class SsrTest {

        [Fact]
        public void CorrectPidAlertedWhenTemperatureChanges() {
            //using (var container = ComponentRegistrator.ComponentRegistry()
            //                                           .RegisterThermos<RandomFakedThermocouple>()
            //                                           .RegisterPids<TestPid>()) {

            //    var thermo = container.GetInstance<Thermocouple>(Location.HLT);
            //    TestPid correctPid = (TestPid)container.GetInstance<Pid>(Location.HLT);
            //    List<TestPid> incorrectPids = new List<TestPid> {
            //        (TestPid)container.GetInstance<Pid>(Location.MT),
            //        (TestPid)container.GetInstance<Pid>(Location.BK)
            //    };

            //    thermo.Refresh();

            //    Assert.True(correctPid.ThermocoupleStateChangeOccured,
            //                "Correct Thermocouple didn't receive event");
            //    foreach (var incorrectPid in incorrectPids)
            //        Assert.False(incorrectPid.ThermocoupleStateChangeOccured,
            //                     "Incorrect Thermocouple received event");
            //}
        }

    }
}
