using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using PubSub;
using SimpleInjector;
using System;
using System.Linq;
using Xunit;

namespace BFV.Test.Components {
    public class EventTests {
        [Fact]
        public void TriggeredChange() {

            var container = ComponentRegistrator.ComponentRegistry(testMode: true);

            //var pid = container.GetInstance<Pid>(Location.HLT);
            var thermo = container.GetInstance<Thermocouple>(Location.HLT);
            
            
            //bool publishedStateChanged = false;
            //Action<ComponentStateChange<PidState>> publishStateChange = (cs) => {
            //    publishedStateChanged = true;
            //};


            //pid.ComponentStateChangePublisher(publishStateChange);

            //var display = container.GetInstance<LcdDisplay>();



            

            //thermo.ComponentStateChangePublisher(hub.Publish<ComponentStateChange<ThermocoupleState>>);
            //hub.Subscribe<ComponentStateChange<ThermocoupleState>>(pid.ComponentStateChangeOccurred);


            thermo.Refresh();

            //pid.PidStateChangePublisher(hub.Publish<PidStateChange>);
            //pid.ComponentStateChangePublisher(hub.Publish<ComponentStateChange<PidState>>);


            //hub.Subscribe<ComponentStateChange<PidState>>(display.ComponentStateChangeOccurred);



            //Assert.True(publishedStateChanged);
        }
    }
}
