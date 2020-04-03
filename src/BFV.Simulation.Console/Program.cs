using BFV.Components;
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

            System.Console.ReadLine();
        }

        static void RefreshThermocouples(Object source, System.Timers.ElapsedEventArgs e) {
            System.Console.WriteLine("Refreshing!");
            container.RefreshThermocouples();
        }
    }
}
