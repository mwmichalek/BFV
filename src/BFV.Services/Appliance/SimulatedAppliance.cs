using BFV.Common;
using BFV.Components;
using BFV.Components.Thermocouples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Services.Appliance {

    public interface IAppliance {

    }
    public class SimulatedAppliance : IAppliance {

        protected readonly ILogger<SimulatedAppliance> _logger;

        private IList<IThermocouple> thermocouples = new List<IThermocouple>();

        private IList<IPid> pids = new List<IPid>();

        private IList<ISsr> ssrs = new List<ISsr>();

        public SimulatedAppliance(ILogger<SimulatedAppliance> logger,
                                  IThermocouple thermo1,
                                  IThermocouple thermo2,
                                  IThermocouple thermo3,
                                  IThermocouple thermo4,
                                  IThermocouple thermo5,
                                  IThermocouple thermo6,
                                  IThermocouple thermo7,
                                  IThermocouple thermo8,
                                  IPid pid1,
                                  IPid pid2,
                                  IPid pid3,
                                  ISsr ssr1,
                                  ISsr ssr2) {
            _logger = logger;
            thermocouples.Add(thermo1);
            thermocouples.Add(thermo2);
            thermocouples.Add(thermo3);
            thermocouples.Add(thermo4);
            thermocouples.Add(thermo5);
            thermocouples.Add(thermo6);
            thermocouples.Add(thermo7);
            thermocouples.Add(thermo8);

            pids.Add(pid1);
            pids.Add(pid2);
            pids.Add(pid3);

            ssrs.Add(ssr1);
            ssrs.Add(ssr2);

            Configure();
        }

        private void Configure() {
            for (int x = 0; x < thermocouples.Count; x++)
                thermocouples[x].Location = LocationHelper.AllLocations[x];

            for (int x = 0; x < pids.Count; x++)
                pids[x].Location = LocationHelper.PidLocations[x];

            for (int x = 0; x < ssrs.Count; x++)
                ssrs[x].Location = LocationHelper.SsrLocations[x];

            //TODO: Wire up hub
        }

        
    }

    public static class SimulatedApplianceHelper {

        public static IServiceCollection RegisterSimulatedApplianceComponents(this IServiceCollection services) {

            services.AddTransient<IThermocouple, SimulationThermocouple>();
            services.AddTransient<IPid, Pid>();
            services.AddTransient<ISsr, Ssr>();
            services.AddSingleton<IAppliance, SimulatedAppliance>();

            return services;

        }

    }
}
