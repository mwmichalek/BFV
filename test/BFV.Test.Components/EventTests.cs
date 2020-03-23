using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using PubSub;
using SimpleInjector;
using System;
using Xunit;

namespace BFV.Test.Components {
    public class EventTests {
        [Fact]
        public void Test1() {

            var container = ComponentRegistrator.ComponentRegistry();

            var pid = container.GetInstance<Pid>();
            var display = container.GetInstance<LcdDisplay>();



            var hub = Hub.Default;

          
            //pid.PidStateChangePublisher(hub.Publish<PidStateChange>);
            pid.ComponentStateChangePublisher(hub.Publish<ComponentStateChange<PidState>>);


            hub.Subscribe<ComponentStateChange<PidState>>(display.ComponentStateChangeOccurred);

            pid.Test();


        }
    }
}
