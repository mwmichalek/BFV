using BFV.Common;
using BFV.Components;
using BFV.Components.EventRecorders;
using BFV.Components.Ssrs;
using BFV.Components.Thermocouples;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFV.Services.Appliances {
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

        protected IList<IEventRecorder> eventRecorders = new List<IEventRecorder>();

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
}
