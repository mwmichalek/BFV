using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PubSub;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BFV.Services.Hub;

namespace BFV.Services.Appliance {

    public interface IAppliance : IRefreshableComponent {

        IThermocouple GetThermocouple(Location location);

        ISsr GetSsr(Location location);

        IPid GetPid(Location location);

    }

    public abstract class Appliance : IAppliance {

        protected readonly ILogger<Appliance> _logger;

        protected IList<IThermocouple> thermocouples = new List<IThermocouple>();

        protected IList<IPid> pids = new List<IPid>();

        protected IList<ISsr> ssrs = new List<ISsr>();

        public Appliance(ILogger<Appliance> logger) {
            _logger = logger;
        }

        public IThermocouple GetThermocouple(Location location) {
            return thermocouples.SingleOrDefault(t => t.Location == location);
        }

        public ISsr GetSsr(Location location) {
            return ssrs.SingleOrDefault(s => s.Location == location);
        }

        public IPid GetPid(Location location) {
            return pids.SingleOrDefault(p => p.Location == location);
        }

        protected abstract void Configure();

        public abstract void Refresh();

    }




    public class SimulatedAppliance : Appliance {

        private IHub _hub;

        public SimulatedAppliance(ILogger<Appliance> logger,
                                  IHub hub,
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
                                  ISsr ssr2) : base(logger) {
            _hub = hub;
            
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

            _hub.Publish<ComponentStateRequest<PidState>>(new ComponentStateRequest<PidState> {
                Location = Location.HLT,
                Updates = (state) => {
                    state.SetPoint = Temperature.RoomTemp + 10;
                    state.PidMode = PidMode.Temperature;
                    state.IsEngaged = true;
                }
            });
        }

        public override void Refresh() {
            foreach (var thermocouple in thermocouples)
                thermocouple.Refresh();
        }

        protected override void Configure() {
            ConfigureThermocouples();
            ConfigureSsrs();
            ConfigurePids();
        }

        private void ConfigureThermocouples() {
            int index = 0;
            foreach (var thermocouple in thermocouples) {
                thermocouple.Location = LocationHelper.AllLocations[index];
                thermocouple.ComponentStateChangePublisher(_hub.Publish<ComponentStateChange<ThermocoupleState>>);
                if (thermocouple is SimulationThermocouple ssrAwareThermo)
                    _hub.Subscribe<ComponentStateChange<SsrState>>((Action<ComponentStateChange<SsrState>>)ssrAwareThermo.ComponentStateChangeOccurred);
                index++;
            }
        }

        

        private void ConfigureSsrs() {
            int index = 0;
            foreach (var ssr in ssrs) {
                ssr.Location = LocationHelper.SsrLocations[index];
                _hub.Subscribe<ComponentStateRequest<SsrState>>((Action<ComponentStateRequest<SsrState>>)ssr.ComponentStateRequestOccurred);
                ssr.ComponentStateChangePublisher(_hub.Publish<ComponentStateChange<SsrState>>);
                index++;
            }
        }

        private void ConfigurePids() {
            int index = 0;
            foreach (var pid in pids) {
                pid.Location = LocationHelper.PidLocations[index];
                _hub.Subscribe<ComponentStateChange<ThermocoupleState>>((Action<ComponentStateChange<ThermocoupleState>>)pid.ComponentStateChangeOccurred);
                _hub.Subscribe<ComponentStateRequest<PidState>>((Action<ComponentStateRequest<PidState>>)pid.ComponentStateRequestOccurred);
                pid.ComponentStateChangePublisher(_hub.Publish<ComponentStateChange<PidState>>);
                pid.ComponentStateRequestPublisher(_hub.Publish<ComponentStateRequest<PidState>>);
                pid.ComponentStateRequestPublisher(_hub.Publish<ComponentStateRequest<SsrState>>);
                index++;
            }
        }

        
    }

    public static class SimulatedApplianceHelper {

        public static IServiceCollection RegisterSimulatedApplianceComponents(this IServiceCollection services, IHub hub = null) {

            services.AddTransient<IThermocouple, SimulationThermocouple>();
            services.AddTransient<IPid, Pid>();
            services.AddTransient<ISsr, Ssr>();
            services.AddSingleton<IAppliance, SimulatedAppliance>();

            if (hub == null)
                services.AddSingleton<IHub>((serviceProvider) => new PubSubHub());
            else
                services.AddSingleton<IHub>((serviceProvider) => hub);

            return services;
        }

    }
}
