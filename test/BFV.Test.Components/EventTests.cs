using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using PubSub;
using SimpleInjector;
using System;
using System.Linq;
using Xunit;

namespace BFV.Test.Components {
    public class EventTests {
        [Fact]
        public void TriggeredChange() {

            var container = ComponentRegistrator.ComponentRegistry();

            var pid = container.GetInstance<Pid>(Location.HLT);

            bool publishedStateChanged = false;
            Action<ComponentStateChange<PidState>> publishStateChange = (cs) => {
                publishedStateChanged = true;
            };


            pid.ComponentStateChangePublisher(publishStateChange);

            var display = container.GetInstance<LcdDisplay>();



            var hub = Hub.Default;

          
            //pid.PidStateChangePublisher(hub.Publish<PidStateChange>);
            //pid.ComponentStateChangePublisher(hub.Publish<ComponentStateChange<PidState>>);


            hub.Subscribe<ComponentStateChange<PidState>>(display.ComponentStateChangeOccurred);

            pid.Test();

            Assert.True(publishedStateChanged);
        }
    }
}
