using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BFV.Components;
using BFV.Components.Thermocouples;
using BFV.Test.Components.TestComponents;
using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using PubSub;
using Moq;
using System.Linq;

namespace BFV.Test.Components {
    public class ThermocoupleTests : SimulatedApplianceBaseTests {

        [Fact]
        public void Balls() {

            //Action<ComponentStateChange<ThermocoupleState>> componentStateChangePublisher = (shizzle) => { };

            //var mockThermos = LocationHelper.AllLocations.Select(l => new Mock<IThermocouple>()
            //                                                            .SetupProperty(x => x.Location, l)
            //                                                            //.Setup(x => { x.ComponentStateChangePublisher(componentStateChangePublisher); })
            //    ).ToList();


            //var thermos = mockThermos.Select(mp => mp.Object).ToList<IThermocouple>();//

            //using (var container = ComponentRegistrator.ComponentRegistry()
            //                                           .RegisterThermos(thermos)) {

            //    foreach (var thermo in thermos) {
            //        thermo.Refresh();
            //        var mockThermo = mockThermos.SingleOrDefault(mt => mt.Object.Location == thermo.Location);
            //        mockThermo.Verify(mt => mt.ComponentStateChangePublisher(null), Times.Exactly(1));

            //    }
               


            //    //Assert.True(correctPid.ThermocoupleStateChangeOccured,
            //    //            "Correct Thermocouple didn't receive event");
            //    //foreach (var incorrectPid in incorrectPids)
            //    //    Assert.False(incorrectPid.ThermocoupleStateChangeOccured, 
            //    //                 "Incorrect Thermocouple received event");
            //}
        }


    }
}
