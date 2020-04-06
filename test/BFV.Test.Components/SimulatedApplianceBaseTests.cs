using BFV.Common;
using BFV.Services.Appliance;
using BFV.Services.Hub;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFV.Test.Components {
    public abstract class SimulatedApplianceBaseTests {

        private static IServiceProvider serviceProvider;
        protected IServiceProvider RetrieveServiceProvider(IHub hub = null) {
            if (serviceProvider == null)
                serviceProvider = new ServiceCollection().AddLogging()
                                                         .RegisterSimulatedApplianceComponents(hub)
                                                         .BuildServiceProvider();
            return serviceProvider;
        }

        protected T RetrieveEntity<T>() where T : IComponent {
            return RetrieveServiceProvider().GetService<T>();
        }

    }
}
