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


        public static Container ComponentRegistry(bool testMode = false) {
            return new Container().RegisterComponents(testMode);
        }

        public static Container RegisterComponents(this Container container, bool testMode = false) {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Trace()
                .CreateLogger();

            container.Options.DependencyInjectionBehavior = new SerilogContextualLoggerInjectionBehavior(container.Options);
            container.Register<ILogger>(() => Log.Logger);

            var hub = Hub.Default;
            container.Register<Hub>(() => hub);

            container.RegisterThermos(hub, testMode);

            container.RegisterPids(hub, testMode);

            container.RegisterDisplay(hub, testMode);

            return container;
        }

        private static Container RegisterThermos(this Container container, Hub hub, bool testMode = false) {
            List<Thermocouple> thermos = (testMode) ?
                LocationHelper.AllLocations.Select(l => new RandomFakedThermocouple(Log.Logger) { Location = l }).ToList<Thermocouple>() :
                LocationHelper.AllLocations.Select(l => new Thermocouple(Log.Logger) { Location = l }).ToList<Thermocouple>();
            container.Collection.Register<Thermocouple>(thermos);

            foreach (var thermo in thermos)
                thermo.ComponentStateChangePublisher(hub.Publish<ComponentStateChange<ThermocoupleState>>);

            // If in testMode, RandomFakedThermocouple should listen to PID changes

            return container;
        }

        private static Container RegisterPids(this Container container, Hub hub, bool testMode = false) {

            var pids = LocationHelper.PidLocations.Select(l => new Pid(Log.Logger) { Location = l }).ToList();
            container.Collection.Register<Pid>(pids);

            foreach (var pid in pids) {
                hub.Subscribe<ComponentStateChange<ThermocoupleState>>(pid.ComponentStateChangeOccurred);
                pid.ComponentStateChangePublisher(hub.Publish<ComponentStateChange<PidState>>);
            }

            return container;
        }

        private static Container RegisterDisplay(this Container container, Hub hub, bool testMode = false) {

            var display = new LcdDisplay(Log.Logger);
            container.Register<LcdDisplay>(() => display);

            hub.Subscribe<ComponentStateChange<ThermocoupleState>>(display.ComponentStateChangeOccurred);
            hub.Subscribe<ComponentStateChange<PidState>>(display.ComponentStateChangeOccurred);

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
