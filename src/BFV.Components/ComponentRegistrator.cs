using Serilog;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
            List<IComponent> components = new List<IComponent>();

            container.Options.DependencyInjectionBehavior = new SerilogContextualLoggerInjectionBehavior(container.Options);
            container.Register<ILogger>(() => Log.Logger);


            List<Thermocouple> thermos = (testMode) ?
                LocationHelper.AllLocations.Select(l => new RandomFakedThermocouple(Log.Logger) { Location = l }).ToList<Thermocouple>() :
                LocationHelper.AllLocations.Select(l => new Thermocouple(Log.Logger) { Location = l }).ToList<Thermocouple>();
            container.Collection.Register<Thermocouple>(thermos);
            components.AddRange(thermos);

            var pids = LocationHelper.PidLocations.Select(l => new Pid(Log.Logger) { Location = l }).ToList();
            container.Collection.Register<Pid>(pids);
            components.AddRange(pids);

            var display = new LcdDisplay(Log.Logger);
            container.Register<LcdDisplay>(() => display);
            components.Add(display);

            var bfvAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("BFV.")).ToList();

            var componentStateTypes = bfvAssemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(IComponentState).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x.Name).ToList();

            var publishers = bfvAssemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(IComponentStateChangePublisher<>).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

            var subscribers = bfvAssemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(IComponentStateChangeSubscriber<>).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
            //foreach (var componentStateType in componentStateTypes)


            //thermo.ComponentStateChangePublisher(hub.Publish<ComponentStateChange<ThermocoupleState>>);
            //hub.Subscribe<ComponentStateChange<ThermocoupleState>>(pid.ComponentStateChangeOccurred);


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
}
