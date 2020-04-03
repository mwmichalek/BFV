using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using PubSub;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BFV.Simulation.Console {
    
    class Program {

        private static Container container;

        static void Main(string[] args) {

            container = ComponentRegistrator.ComponentRegistry()
                                            .RegisterComponentsForSimulation();
            var timer = new Timer(2000);
            timer.Elapsed += RefreshThermocouples;
            timer.Start();

            var pidUpdateRequest = new ComponentStateRequest<PidState> {
                Location = Location.HLT,
                Updates = (state) => {
                    state.IsEngaged = true;
                    state.Temperature = Temperature.RoomTemp;
                    state.SetPoint = 140;
                }
            };




            var hub = container.GetInstance<Hub>();
            hub.Publish(pidUpdateRequest);

            System.Console.ReadLine();
        }

        static void RefreshThermocouples(Object source, System.Timers.ElapsedEventArgs e) {
            System.Console.WriteLine("Refreshing!");
            container.RefreshThermocouples();
        }
    }
}
