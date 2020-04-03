using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;

namespace BFV.Simulation.WebJob {
    
    class Program {
        //https://docs.microsoft.com/en-us/azure/app-service/webjobs-sdk-get-started
        static void Main(string[] args) {
            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b => {
                //b.AddAzureStorageCoreServices();
            });
            var host = builder.Build();
            using (host) {
                host.Run();
            }
        }
    }
}
