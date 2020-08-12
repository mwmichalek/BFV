using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appliance.WorkerService.Appliances;
using BFV.Appliance.WorkerService.Appliances;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BFV.Simulation.WorkerService {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => {
                    logging.AddConsole();
                    logging.AddDebug();
                });

            var isSimulation = Environment.OSVersion.Platform == System.PlatformID.Win32NT;

            if (isSimulation)
                hostBuilder.ConfigureServices((hostContext, services) => {
                    services.RegisterSimulatedApplianceComponents();
                    services.AddHostedService<Worker>();
                });
            else
                hostBuilder.ConfigureServices((hostContext, services) => {
                    services.RegisterPiApplianceComponents();
                    services.AddHostedService<Worker>();
                });

            return hostBuilder;
        }
    }
}





































//private static Container container;

//static void Main(string[] args) {

//    container = ComponentRegistrator.ComponentRegistry()
//                                    .RegisterComponentsForSimulation();
//    var timer = new Timer(2000);
//    timer.Elapsed += RefreshThermocouples;
//    timer.Start();

//    var pidUpdateRequest = new ComponentStateRequest<PidState> {
//        Location = Location.HLT,
//        Updates = (state) => {
//            state.IsEngaged = true;
//            state.Temperature = Temperature.RoomTemp;
//            state.SetPoint = 140;
//        }
//    };




//    var hub = container.GetInstance<Hub>();
//    hub.Publish(pidUpdateRequest);

//    System.Console.ReadLine();
//}

//static void RefreshThermocouples(Object source, System.Timers.ElapsedEventArgs e) {
//    System.Console.WriteLine("Refreshing!");
//    container.RefreshThermocouples();
//}