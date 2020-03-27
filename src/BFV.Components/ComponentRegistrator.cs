using Serilog;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PubSub;
using SimpleInjector.Advanced;
using BFV.Common;
using System.Linq;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using BFV.Common.Events;

namespace BFV.Components {
    public static class ComponentRegistrator {

        public static Container ComponentRegistry() {
            return new Container();
        }

        public static Container RegisterAllComponents(this Container container) {

            container.RegisterLogging();

            container.Register<Hub>(() => Hub.Default);

            container.RegisterThermos();

            container.RegisterPids();

            container.RegisterSsrs();

            container.RegisterDisplay();

            return container;
        }

        public static Container DeregisterAllComponents(this Container container) {

            //try {
            //    Hub.Default.Unsubscribe();
            //} catch (Exception) { }

            //container.DeregisterAllComponents();

            return container;
        }

        public static Container RegisterLogging(this Container container) {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Trace()
                .CreateLogger();

            container.Options.DependencyInjectionBehavior = new SerilogContextualLoggerInjectionBehavior(container.Options);
            container.Register<ILogger>(() => Log.Logger);

            return container;
        }

        public static Container RegisterThermos<TThermo>(this Container container) where TThermo : Thermocouple {
            List<Thermocouple> thermos = LocationHelper.AllLocations.Select(l => {
                TThermo thermo = (TThermo)Activator.CreateInstance(typeof(TThermo), new object[] { Log.Logger });
                thermo.Location = l;
                return thermo;
            }).ToList<Thermocouple>();

            return container.RegisterThermos(thermos);
        }

        public static Container RegisterThermos(this Container container, List<Thermocouple> preexistingThermocouples = null) {
            List<Thermocouple> thermos = (preexistingThermocouples == null) ?
                LocationHelper.AllLocations.Select(l => new Thermocouple(Log.Logger) { Location = l }).ToList<Thermocouple>() :
                preexistingThermocouples;

            container.Collection.Register<Thermocouple>(thermos);

            foreach (var thermo in thermos) {
                thermo.ComponentStateChangePublisher(Hub.Default.Publish<ComponentStateChange<ThermocoupleState>>);

                // For simulation purposes.
                if (thermo is SsrAwareFakedThermocouple ssrAwareThermo)
                    Hub.Default.Subscribe<ComponentStateChange<SsrState>>(ssrAwareThermo.ComponentStateChangeOccurred);
            }

            return container;
        }

        public static Container RegisterPids<TPid>(this Container container) where TPid : Pid {
            List<Pid> pids = LocationHelper.PidLocations.Select(l => {
                TPid pid = (TPid)Activator.CreateInstance(typeof(TPid), new object[] { Log.Logger });
                pid.Location = l;
                return pid;
            }).ToList<Pid>();

            return container.RegisterPids(pids);
        }

        public static Container RegisterPids(this Container container, List<Pid> preexistingPids = null) {

            List<Pid> pids = (preexistingPids == null) ?
                LocationHelper.PidLocations.Select(l => new Pid(Log.Logger) { Location = l }).ToList() :
                preexistingPids;
            container.Collection.Register<Pid>(pids);

            foreach (var pid in pids) {
                Hub.Default.Subscribe<ComponentStateChange<ThermocoupleState>>(pid.ComponentStateChangeOccurred);
                Hub.Default.Subscribe<ComponentStateRequest<PidState>>(pid.ComponentStateRequestOccurred);
                pid.ComponentStateChangePublisher(Hub.Default.Publish<ComponentStateChange<PidState>>);
            }

            return container;
        }

        public static Container RegisterSsrs<TSsr>(this Container container) where TSsr : Ssr {
            List<Ssr> ssrs = LocationHelper.SsrLocations.Select(l => {
                TSsr ssr = (TSsr)Activator.CreateInstance(typeof(TSsr), new object[] { Log.Logger });
                ssr.Location = l;
                return ssr;
            }).ToList<Ssr>();

            return container.RegisterSsrs(ssrs);
        }

        public static Container RegisterSsrs(this Container container, List<Ssr> preexistingSsrs = null) {
            List<Ssr> ssrs = (preexistingSsrs == null) ?
                LocationHelper.SsrLocations.Select(l => new Ssr(Log.Logger) { Location = l }).ToList() :
                preexistingSsrs;
            container.Collection.Register<Ssr>(ssrs);

            foreach (var ssr in ssrs) {
                Hub.Default.Subscribe<ComponentStateChange<PidState>>(ssr.ComponentStateChangeOccurred);
                ssr.ComponentStateChangePublisher(Hub.Default.Publish<ComponentStateChange<SsrState>>);
            }

            return container;
        }

        public static Container RegisterDisplay(this Container container) {

            var display = new LcdDisplay(Log.Logger);
            container.Register<LcdDisplay>(() => display);

            Hub.Default.Subscribe<ComponentStateChange<ThermocoupleState>>(display.ComponentStateChangeOccurred);
            Hub.Default.Subscribe<ComponentStateChange<PidState>>(display.ComponentStateChangeOccurred);

            return container;
        }

        public static TComp GetInstance<TComp>(this Container container, Location location) where TComp : class, ILocatableComponent {
            return (TComp)(container.GetAllInstances<TComp>()).SingleOrDefault(c => c.Location == location);
        }
    }

    public class SerilogContextualLoggerInjectionBehavior : IDependencyInjectionBehavior {
        private readonly IDependencyInjectionBehavior _original;
        private readonly Container _container;

        public SerilogContextualLoggerInjectionBehavior(ContainerOptions options) {
            _original = options.DependencyInjectionBehavior;
            _container = options.Container;
        }

        public void Verify(InjectionConsumerInfo consumer) => _original.Verify(consumer);

        public InstanceProducer GetInstanceProducer(InjectionConsumerInfo i, bool t) =>
            i.Target.TargetType == typeof(ILogger)
                ? GetLoggerInstanceProducer(i.ImplementationType)
                : _original.GetInstanceProducer(i, t);

        private InstanceProducer<ILogger> GetLoggerInstanceProducer(Type type) =>
            Lifestyle.Transient.CreateProducer(() => Log.ForContext(type), _container);
    }

    public static class InterfaceHelper { 
        public static bool Implements<I>(this Type type, I @interface) where I : class {
            if (((@interface as Type) == null) || !(@interface as Type).IsInterface)
                return false;
            return (@interface as Type).IsAssignableFrom(type);
        }

    }
}
