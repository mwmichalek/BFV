using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.EventRecorders;
using BFV.Components.Hub;
using BFV.Components.Ssrs;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFV.Components.Appliances {
    public interface IAppliance : IRefreshableComponent {

        IThermocouple GetThermocouple(Location location);

        ISsr GetSsr(Location location);

        IPid GetPid(Location location);

    }

    public abstract class BaseAppliance : IAppliance {

        protected readonly ILogger<BaseAppliance> _logger;

        protected IHub _hub;

        protected IList<IThermocouple> thermocouples = new List<IThermocouple>();

        protected IList<IPid> pids = new List<IPid>();

        protected IList<ISsr> ssrs = new List<ISsr>();

        protected IList<IEventRecorder> eventRecorders = new List<IEventRecorder>();

        public BaseAppliance(ILogger<BaseAppliance> logger) {
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

        //protected abstract void Configure();

        //public abstract void Refresh();

        public virtual void Refresh() {
            foreach (var thermocouple in thermocouples)
                thermocouple.Refresh();
        }

        protected virtual void Configure() {
            ConfigureThermocouples();
            ConfigureSsrs();
            ConfigurePids();
            ConfigureEventRecorders();
        }

        protected virtual void ConfigureThermocouples() {
            int index = 0;
            foreach (var thermocouple in thermocouples) {
                thermocouple.Location = LocationHelper.AllLocations[index];
                thermocouple.ComponentStateChangePublisher(_hub.Publish<ComponentStateChange<ThermocoupleState>>);
                if (thermocouple is SimulationThermocouple ssrAwareThermo)
                    _hub.Subscribe<ComponentStateChange<SsrState>>((Action<ComponentStateChange<SsrState>>)ssrAwareThermo.ComponentStateChangeOccurred);
                index++;
            }
        }

        protected virtual void ConfigureSsrs() {
            int index = 0;
            foreach (var ssr in ssrs) {
                ssr.Location = LocationHelper.SsrLocations[index];
                _hub.Subscribe<ComponentStateRequest<SsrState>>((Action<ComponentStateRequest<SsrState>>)ssr.ComponentStateRequestOccurred);
                ssr.ComponentStateChangePublisher(_hub.Publish<ComponentStateChange<SsrState>>);
                index++;
            }
        }

        protected virtual void ConfigurePids() {
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

        protected virtual void ConfigureEventRecorders() {
            int index = 0;
            foreach (var eventRecorder in eventRecorders) {
                _hub.Subscribe<ComponentStateChange<ThermocoupleState>>((Action<ComponentStateChange<ThermocoupleState>>)eventRecorder.ComponentStateChangeOccurred);
                _hub.Subscribe<ComponentStateChange<PumpState>>((Action<ComponentStateChange<PumpState>>)eventRecorder.ComponentStateChangeOccurred);
                _hub.Subscribe<ComponentStateChange<PidState>>((Action<ComponentStateChange<PidState>>)eventRecorder.ComponentStateChangeOccurred);
                _hub.Subscribe<ComponentStateChange<SsrState>>((Action<ComponentStateChange<SsrState>>)eventRecorder.ComponentStateChangeOccurred);

                index++;
            }
        }

    }
}
