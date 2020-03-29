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

namespace BFV.Test.Components {
    public class PidTests {

        [Fact]
        public void CorrectPidAlertedWhenTemperatureChanges() {
            using (var container = ComponentRegistrator.ComponentRegistry()
                                                .RegisterLogging()
                                                .RegisterThermos<RandomFakedThermocouple>()
                                                .RegisterPids<TestPid>()) {

                var thermo = container.GetInstance<Thermocouple>(Location.HLT);
                TestPid correctPid = (TestPid)container.GetInstance<Pid>(Location.HLT);
                TestPid incorrectPid = (TestPid)container.GetInstance<Pid>(Location.BK);

                thermo.Refresh();

                Assert.True(correctPid.ThermocoupleStateChangeOccured);
                Assert.False(incorrectPid.ThermocoupleStateChangeOccured);

            }
        }

        [Fact]
        public void SendPidUpdate() {
            using (var container = ComponentRegistrator.ComponentRegistry()
                                                .RegisterLogging()
                                                //.RegisterThermos<RandomFakedThermocouple>()
                                                .RegisterPids<TestPid>()) {

                TestPid pid = (TestPid)container.GetInstance<Pid>(Location.HLT);


                var targetTemperature = 99;

                var pidUpdateRequest = new ComponentStateRequest<PidState> {
                    Updates = (initialState) => {
                        initialState.Temperature = targetTemperature;
                    }
                };

                var hub = container.GetInstance<Hub>();
                Assert.True(hub.Exists<ComponentStateRequest<PidState>>());

                hub.Publish<ComponentStateRequest<PidState>>(pidUpdateRequest);

                Assert.Equal(targetTemperature, pid.CurrentState.Temperature);
            };
        }

    }
}
