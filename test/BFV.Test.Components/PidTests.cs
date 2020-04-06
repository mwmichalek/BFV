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
using static BFV.Services.ComponentRegistrator;
using BFV.Services.Hub;
using Microsoft.Extensions.Logging;

namespace BFV.Test.Components {
    public class PidTests : SimulatedApplianceBaseTests {

        [Fact]
        public void EnablingPidSendsRequestToDisableOtherPids() {
            var Utils = CreateHub(Location.HLT, new PidState {
                IsEngaged = false
            });

            List<ComponentStateRequest<PidState>> pidRequests = new List<ComponentStateRequest<PidState>>();

            Utils.MockHub.Setup(hb => hb.Publish<ComponentStateRequest<PidState>>(It.IsAny<ComponentStateRequest<PidState>>()))
                         .Callback<ComponentStateRequest<PidState>>((req) => pidRequests.Add(req));

            Utils.Pid.ComponentStateRequestPublisher(Utils.Hub.Publish<ComponentStateRequest<PidState>>);

            Utils.Pid.ComponentStateRequestOccurred(new ComponentStateRequest<PidState> {
                Location = Location.HLT,
                Updates = (state) => {
                    state.IsEngaged = true;
                    state.SetPoint = 140;
                    state.Temperature = Temperature.RoomTemp;
                }
            });

            Assert.True(pidRequests.Count == 2, "2 requests should have been published");

            var mtRequest = pidRequests.SingleOrDefault(r => r.Location == Location.MT);
            Assert.True(mtRequest != null, "MT request wasn't published");
            var mtState = mtRequest.UpdateState(new PidState());
            Assert.True(mtState.IsEngaged == false, "MT request was to engage");

            var bkRequest = pidRequests.SingleOrDefault(r => r.Location == Location.BK);
            Assert.True(bkRequest != null, "BK request wasn't published");
            var bkState = bkRequest.UpdateState(new PidState());
            Assert.True(bkState.IsEngaged == false, "BK request was to engage");
        }

        [Fact]
        public void DisablingPidDisablesSsrs() {
            var Utils = CreateHub(Location.HLT, new PidState {
                IsEngaged = true,
                SetPoint = Temperature.BoilingTemp,
                Temperature = Temperature.BoilingTemp
            });

            ComponentStateRequest<SsrState> ssrRequest = null;

            Utils.MockHub.Setup(hb => hb.Publish<ComponentStateRequest<SsrState>>(It.IsAny<ComponentStateRequest<SsrState>>()))
                         .Callback<ComponentStateRequest<SsrState>>((req) => ssrRequest = req);

            Utils.Pid.ComponentStateRequestPublisher(Utils.Hub.Publish<ComponentStateRequest<SsrState>>);

            Utils.Pid.ComponentStateRequestOccurred(new ComponentStateRequest<PidState> {
                Location = Location.HLT,
                Updates = (state) => {
                    state.IsEngaged = false;
                }
            });

            Assert.True(ssrRequest != null, "ComponentStateRequest<SsrState> was never published");
            var ssrState = ssrRequest.UpdateState(new SsrState());
            Assert.True(ssrState.IsEngaged == false, "Ssr request wasn't to disengage");
        }


        [Fact]
        public void ThermoChangeTriggersSsrRequest() {

            var Utils = CreateHub(Location.HLT, new PidState {
                IsEngaged = true,
                SetPoint = 140,
                Temperature = Temperature.RoomTemp
            });

            ComponentStateRequest<SsrState> ssrRequest = null;

            Utils.MockHub.Setup(hb => hb.Publish<ComponentStateRequest<SsrState>>(It.IsAny<ComponentStateRequest<SsrState>>()))
                         .Callback<ComponentStateRequest<SsrState>>((req) => ssrRequest = req);

            Utils.Pid.ComponentStateRequestPublisher(Utils.Hub.Publish<ComponentStateRequest<SsrState>>);

            Utils.Pid.ComponentStateChangeOccurred(new ComponentStateChange<ThermocoupleState> {
                Location = Location.HLT,
                CurrentState = new ThermocoupleState {
                    Temperature = Temperature.RoomTemp + 10
                }
            });

            Assert.True(ssrRequest != null, "ComponentStateRequest<SsrState> was never published");

            var updatedSsrState = ssrRequest.UpdateState(new SsrState());
            Assert.True(updatedSsrState.Percentage == 100, "Requested Ssr Percentage was not equal to 100");

            // This is no longer needed but lets keep it so we know how to do it next time.
            //Utils.MockHub.Verify(hb => hb.Publish(It.Is<ComponentStateRequest<SsrState>>(req => req.Location == Location.HLT)));
        }

        [Fact]
        public void PidRequestTriggersPidChange() {

            var Utils = CreateHub(Location.HLT, new PidState {
                IsEngaged = true,
                SetPoint = 140,
                Temperature = Temperature.RoomTemp
            });

            ComponentStateChange<PidState> pidChange = null;

            Utils.MockHub.Setup(hb => hb.Publish<ComponentStateChange<PidState>>(It.IsAny<ComponentStateChange<PidState>>()))
                         .Callback<ComponentStateChange<PidState>>((req) => pidChange = req);

            Utils.Pid.ComponentStateChangePublisher(Utils.Hub.Publish<ComponentStateChange<PidState>>);

            Utils.Pid.ComponentStateRequestOccurred(new ComponentStateRequest<PidState> {
                Location = Location.HLT,
                Updates = (state) => {
                    state.SetPoint = 120;
                }
            });

            Assert.True(pidChange != null, "ComponentStateChange<PidState> was never published");
            Assert.True(pidChange.CurrentState.SetPoint == 120, "Requested Pid SetPoint was not equal to 120");
        }

        private (Mock<IHub> MockHub, IHub Hub, IPid Pid) CreateHub(Location location, PidState state) {
            var logger = new Mock<ILogger<Pid>>();
            var mockHub = new Mock<IHub>();
            var hub = mockHub.Object;

            var pid = new Pid(logger.Object);
            pid.CurrentState = state;
            pid.Location = location;
            return (mockHub, hub, pid);
        }

    }
}
