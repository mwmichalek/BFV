using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using BFV.Services.Hub;
using Microsoft.Extensions.Logging;
using Moq;
using PubSub;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BFV.Test.Components {
    public class SsrTests : SimulatedApplianceBaseTests {

        [Fact]
        public void SsrRequestTriggersSsrChange() {

            var Utils = CreateUtils(Location.HLT, new SsrState {
                Percentage = 0
            });

            ComponentStateChange<SsrState> ssrChange = null;

            Utils.MockHub.Setup(hb => hb.Publish<ComponentStateChange<SsrState>>(It.IsAny<ComponentStateChange<SsrState>>()))
                         .Callback<ComponentStateChange<SsrState>>((chg) => ssrChange = chg);

            Utils.Ssr.ComponentStateChangePublisher(Utils.Hub.Publish<ComponentStateChange<SsrState>>);

            Utils.Ssr.ComponentStateRequestOccurred(new ComponentStateRequest<SsrState> {
                Location = Location.HLT,
                Updates = (state) => {
                    state.Percentage = 10;
                }
            });

            Assert.True(ssrChange != null, "ComponentStateChange<SsrState> was never published");
            Assert.True(ssrChange.Location == Location.HLT, "Ssr change Location is not HLT");
            Assert.True(ssrChange.CurrentState.Percentage == 10, "Ssr change Percentage was not equal to 10");
        }

        private (Mock<IHub> MockHub, IHub Hub, ISsr Ssr) CreateUtils(Location location, SsrState state) {
            var logger = new Mock<ILogger<Ssr>>();
            var mockHub = new Mock<IHub>();
            var hub = mockHub.Object;

            var ssr = new Ssr(logger.Object);
            ssr.CurrentState = state;
            ssr.Location = location;
            return (mockHub, hub, ssr);
        }

    }
}
