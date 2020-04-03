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
using Serilog;
using static BFV.Components.ComponentRegistrator;

namespace BFV.Test.Components {
    public class PidTests {


        [Fact]
        public void ThermoChangeTriggersSsrRequest() {

            var logger = new Mock<ILogger>();
            var mockHub = new Mock<IHub>();
            var hub = mockHub.Object;

            var pid = new Pid(logger.Object);
            pid.CurrentState = new PidState {
                IsEngaged = true,
                SetPoint = 140,
                Temperature = Temperature.RoomTemp,
            };
            pid.Location = Location.HLT;
            pid.ComponentStateRequestPublisher(hub.Publish<ComponentStateRequest<SsrState>>);

            pid.ComponentStateChangeOccurred(new ComponentStateChange<ThermocoupleState> {
                Location = Location.HLT,
                CurrentState = new ThermocoupleState {
                    Temperature = Temperature.RoomTemp + 10
                }
            });

            mockHub.Verify(x => x.Publish(It.Is<ComponentStateRequest<SsrState>>(req => req.Location == Location.HLT)));
        }



        [Fact]
        public void CorrectPidAlertedWhenTemperatureChanges() {

            var mockPids = LocationHelper.PidLocations.Select(l => new Mock<IPid>().SetupProperty(x => x.Location, l)).ToList();
            var pids = mockPids.Select(mp => mp.Object).ToList<IPid>();//

            using (var container = ComponentRegistrator.ComponentRegistry()
                                                       .RegisterThermos<RandomFakedThermocouple>()
                                                       .RegisterPids(pids)) {

                var thermo = container.GetInstance<Thermocouple>(Location.HLT);
                IPid correctPid = container.GetInstance<IPid>(Location.HLT);
                List<IPid> incorrectPids = new List<IPid> {
                    container.GetInstance<IPid>(Location.MT),
                    container.GetInstance<IPid>(Location.BK)
                };

                thermo.Refresh();

                var correctPidMock = mockPids.SingleOrDefault(mp => mp.Object.Location == Location.HLT);
                correctPidMock.Verify(p => p.ComponentStateChangeOccurred(null), Times.Exactly(1), "Bitch didn't get called!");


                //Assert.True(correctPid.ThermocoupleStateChangeOccured,
                //            "Correct Thermocouple didn't receive event");
                //foreach (var incorrectPid in incorrectPids)
                //    Assert.False(incorrectPid.ThermocoupleStateChangeOccured, 
                //                 "Incorrect Thermocouple received event");
            }
        }

        [Fact]
        public void PidTemperatureUpdatesWithRequest() {
            using (var container = ComponentRegistrator.ComponentRegistry()
                                                       .RegisterPids<TestPid>()) {

                TestPid correctPid = (TestPid)container.GetInstance<Pid>(Location.HLT);
                List<TestPid> incorrectPids = new List<TestPid> {
                    (TestPid)container.GetInstance<Pid>(Location.MT),
                    (TestPid)container.GetInstance<Pid>(Location.BK)
                };

                var targetTemperature = 99;

                var pidUpdateRequest = new ComponentStateRequest<PidState> {
                    Location = Location.HLT,
                    Updates = (initialState) => {
                        initialState.Temperature = targetTemperature;
                    }
                };

                var hub = container.GetInstance<Hub>();
                Assert.True(hub.Exists<ComponentStateRequest<PidState>>(),
                            "Hub is missing subscriptions for ComponentStateRequest<PidState>");

                hub.Publish<ComponentStateRequest<PidState>>(pidUpdateRequest);

                Assert.Equal(targetTemperature, correctPid.CurrentState.Temperature);

                foreach (var incorrectPid in incorrectPids)
                    Assert.False(incorrectPid.PidStateRequestOccured,
                                 "Incorrect PIDs received event");
            };
        }

        [Fact]
        public void PidEnableUpdatesWithReqest() {
            using (var container = ComponentRegistrator.ComponentRegistry()
                                                       .RegisterPids<TestPid>()) {

                TestPid enablePid = (TestPid)container.GetInstance<Pid>(Location.HLT);

                TestPid disablePidMT = (TestPid)container.GetInstance<Pid>(Location.MT);
                TestPid disablePidBK = (TestPid)container.GetInstance<Pid>(Location.BK);

                // Turning on MT to make sure it gets turned off 
                disablePidMT.CurrentState.IsEngaged = true;

                var pidUpdateRequest = new ComponentStateRequest<PidState> {
                    Location = Location.HLT,
                    Updates = (initialState) => {
                        initialState.IsEngaged = true;
                    }
                };

                var hub = container.GetInstance<Hub>();
                Assert.True(hub.Exists<ComponentStateRequest<PidState>>(),
                            "Hub is missing subscriptions for ComponentStateRequest<PidState>");

                hub.Publish<ComponentStateRequest<PidState>>(pidUpdateRequest);

                Assert.True(enablePid.CurrentState.IsEngaged, "Correct PID is not enabled");
                Assert.False(disablePidMT.CurrentState.IsEngaged, "MT failed to disable");
                Assert.False(disablePidBK.CurrentState.IsEngaged, "BK should not be enabled");
            };
        }


        //TODO: Test PID for updates after temperature change

        //TODO: Test PID for updates after PID change



        [Fact]
        public void PidSendSsrRequestWithTemperatureUpdate() {
            //using (var container = ComponentRegistrator.ComponentRegistry()
            //                                           .RegisterPids<TestPid>()) {

            //    TestPid enablePid = (TestPid)container.GetInstance<Pid>(Location.HLT);


            //    var pidUpdateRequest = new ComponentStateRequest<PidState> {
            //        Location = Location.HLT,
            //        Updates = (initialState) => {
            //            initialState.IsEngaged = true;
            //        }
            //    };

            //    var hub = container.GetInstance<Hub>();
            //    Assert.True(hub.Exists<ComponentStateRequest<PidState>>(),
            //                "Hub is missing subscriptions for ComponentStateRequest<PidState>");

            //    hub.Publish<ComponentStateRequest<PidState>>(pidUpdateRequest);


            //};
        }

    }
}
