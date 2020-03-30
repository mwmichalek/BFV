using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using BFV.Test.Components.TestComponents;
using PubSub;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BFV.Test.Components {
    public class SsrTests {

        [Fact]
        public void CorrectPidAlertedWhenTemperatureChanges() {
            using (var container = ComponentRegistrator.ComponentRegistry()
                                                       .RegisterPids<TestPid>()
                                                       .RegisterSsrs<TestSsr>()) {

                
                TestPid pid = (TestPid)container.GetInstance<Pid>(Location.HLT);
                TestSsr ssr = (TestSsr)container.GetInstance<Ssr>(Location.HLT);

                pid.CurrentState.Temperature = 70;

                var pidUpdateRequest = new ComponentStateRequest<PidState> {
                    Location = Location.HLT,
                    Updates = (initialState) => {
                        initialState.IsEngaged = true;
                        initialState.SetPoint = 212;
                        initialState.Temperature = 70;
                    }
                };

                var hub = container.GetInstance<Hub>();
                Assert.True(hub.Exists<ComponentStateRequest<PidState>>(),
                            "Hub is missing subscriptions for ComponentStateRequest<PidState>");
                Assert.True(hub.Exists<ComponentStateRequest<SsrState>>(),
                            "Hub is missing subscriptions for ComponentStateRequest<SsrState>");

                hub.Publish<ComponentStateRequest<PidState>>(pidUpdateRequest);
                
                Assert.True(pid.CurrentState.IsEngaged, "Correct PID is not enabled");
                Assert.True(pid.CurrentState.SetPoint == 212, "Correct PID is not set to proper SetPoint");

                Assert.True(ssr.SsrStateRequestOccured, "Ssr wasn't requested to update");
                Assert.True(ssr.CurrentState.IsEngaged, "Ssr is not currently enabled");
                Assert.True(ssr.CurrentState.Percentage > 0, "Ssr not set to respectable level");

            }
        }

    }
}
