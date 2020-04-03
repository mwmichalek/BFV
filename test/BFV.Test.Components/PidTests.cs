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

            var Utils = CreateHub(Location.HLT, new PidState {
                IsEngaged = true,
                SetPoint = 140,
                Temperature = Temperature.RoomTemp
            });

            ComponentStateRequest<SsrState> request = null;

            Utils.MockHub.Setup(hb => hb.Publish<ComponentStateRequest<SsrState>>(It.IsAny<ComponentStateRequest<SsrState>>()))
                         .Callback<ComponentStateRequest<SsrState>>((req) => request = req);

            Utils.Pid.ComponentStateRequestPublisher(Utils.Hub.Publish<ComponentStateRequest<SsrState>>);

            Utils.Pid.ComponentStateChangeOccurred(new ComponentStateChange<ThermocoupleState> {
                Location = Location.HLT,
                CurrentState = new ThermocoupleState {
                    Temperature = Temperature.RoomTemp + 10
                }
            });

            //TODO: Analyze request!!!!

            Utils.MockHub.Verify(hb => hb.Publish(It.Is<ComponentStateRequest<SsrState>>(req => req.Location == Location.HLT)));
        }

        [Fact]
        public void PidRequestTriggersPidChange() {

            var Utils = CreateHub(Location.HLT, new PidState {
                IsEngaged = true,
                SetPoint = 140,
                Temperature = Temperature.RoomTemp
            });

            Utils.Pid.ComponentStateChangePublisher(Utils.Hub.Publish<ComponentStateChange<PidState>>);

            Utils.Pid.ComponentStateRequestOccurred(new ComponentStateRequest<PidState> {
                Location = Location.HLT,
                Updates = (state) => {
                    state.SetPoint = 120;
                }
            });

            Utils.MockHub.Verify(hb => hb.Publish(It.Is<ComponentStateChange<PidState>>(req => req.Location == Location.HLT)));
        }

        private (Mock<IHub> MockHub, IHub Hub, IPid Pid) CreateHub(Location location, PidState state) {
            var logger = new Mock<ILogger>();
            var mockHub = new Mock<IHub>();
            var hub = mockHub.Object;

            var pid = new Pid(logger.Object);
            pid.CurrentState = state;
            pid.Location = location;
            return (mockHub, hub, pid);
        }

    }
}
