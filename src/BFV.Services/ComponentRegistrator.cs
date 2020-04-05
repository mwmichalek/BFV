using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PubSub;
using BFV.Common;
using System.Linq;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using BFV.Common.Events;
using BFV.Components.Displays;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using BFV.Components;

namespace BFV.Services {
    public static class ComponentRegistrator {


        public interface IHub {
            bool Exists<T>();
            bool Exists<T>(object subscriber);
            bool Exists<T>(object subscriber, Action<T> handler);
            void Publish<T>(T data = default);
            
            Task PublishAsync<T>(T data = default);
            void Subscribe<T>(Action<T> handler);
            void Subscribe<T>(object subscriber, Action<T> handler);
            void Subscribe<T>(Func<T, Task> handler);
            void Subscribe<T>(object subscriber, Func<T, Task> handler);
            void Unsubscribe();
            void Unsubscribe(object subscriber);
            void Unsubscribe<T>();
            void Unsubscribe<T>(Action<T> handler);
            void Unsubscribe<T>(object subscriber, Action<T> handler = null);
        }


        //public class HubbedContainer : Container {

        //    public Hub Hub { get; set; } = new Hub();

        //    protected override void Dispose(bool disposing) {
        //        //TODO: Hub Unsubscribe all components?
        //        base.Dispose(disposing);
        //    }

        //}

        //public static Container ComponentRegistry() {
        //    return new HubbedContainer().RegisterLogging().RegisterHub(); 
        //}

        public static IServiceCollection RegisterComponentsForSimulation(this IServiceCollection services) {
            //services.RegisterThermos<SimulationThermocouple>();

            services.RegisterPids();

            services.RegisterSsrs();

            services.RegisterDisplays();

            return services;
        }

        

        public static IServiceCollection RegisterAllComponents(this IServiceCollection services) {

            //services.RegisterThermos();

            services.RegisterPids();

            services.RegisterSsrs();

            services.RegisterDisplays();

            return services;
        }

 
        public static IServiceCollection RegisterHub(this IServiceCollection services) {

            services.AddSingleton<Hub>();

            //if (container is HubbedContainer hubbedContainer) 
            //    container.RegisterSingleton<Hub>(() => { return hubbedContainer.Hub; });
            
            return services;
        }

        //public static IServiceCollection RegisterThermos<TThermo>(this IServiceCollection services) where TThermo : Thermocouple {

        //    services.
          
        //    List<IThermocouple> thermos = LocationHelper.AllLocations.Select(l => {
        //        TThermo thermo = (TThermo)Activator.CreateInstance(typeof(TThermo), new object[] { Log.Logger });
        //        thermo.Location = l;
        //        return thermo;
        //    }).ToList<IThermocouple>();

        //    return services.RegisterThermos(thermos);
        //}

        //public static IServiceCollection RegisterThermos(this IServiceCollection services, List<IThermocouple> preexistingThermocouples = null) {
        //    List<IThermocouple> thermos = (preexistingThermocouples == null) ?
        //        LocationHelper.AllLocations.Select(l => new Thermocouple(Log.Logger) { Location = l }).ToList<IThermocouple>() :
        //        preexistingThermocouples;

        //    //container.Collection.Register<IThermocouple>(thermos);

        //    //if (container is HubbedContainer hubbedContainer) {
        //    //    foreach (var thermo in thermos) {
        //    //        thermo.ComponentStateChangePublisher(hubbedContainer.Hub.Publish<ComponentStateChange<ThermocoupleState>>);

        //    //        // For simulation purposes.
        //    //        if (thermo is SimulationThermocouple ssrAwareThermo)
        //    //            hubbedContainer.Hub.Subscribe<ComponentStateChange<SsrState>>((Action<ComponentStateChange<SsrState>>)ssrAwareThermo.ComponentStateChangeOccurred);
        //    //    }
        //    //}

        //    return services;
        //}

        //public static IServiceCollection RegisterPids<TPid>(this IServiceCollection container) where TPid : Pid {
        //    List<IPid> pids = LocationHelper.PidLocations.Select(l => {
        //        TPid pid = (TPid)Activator.CreateInstance(typeof(TPid), new object[] { Log.Logger });
        //        pid.Location = l;
        //        return pid;
        //    }).ToList<IPid>();

        //    return container.RegisterPids(pids);
        //}

        public static IServiceCollection RegisterPids(this IServiceCollection container, List<IPid> preexistingPids = null) {

            //List<IPid> pids = (preexistingPids == null) ?
            //    LocationHelper.PidLocations.Select(l => new Pid(Log.Logger) { Location = l }).ToList<IPid>() :
            //    preexistingPids;
            //container.Collection.Register<IPid>(pids);

            //if (container is HubbedContainer hubbedContainer) {
            //    foreach (var pid in pids) {
            //        hubbedContainer.Hub.Subscribe<ComponentStateChange<ThermocoupleState>>((Action<ComponentStateChange<ThermocoupleState>>)pid.ComponentStateChangeOccurred);
            //        hubbedContainer.Hub.Subscribe<ComponentStateRequest<PidState>>((Action<ComponentStateRequest<PidState>>)pid.ComponentStateRequestOccurred);
            //        pid.ComponentStateChangePublisher(hubbedContainer.Hub.Publish<ComponentStateChange<PidState>>);
            //        pid.ComponentStateRequestPublisher(hubbedContainer.Hub.Publish<ComponentStateRequest<PidState>>);
            //        pid.ComponentStateRequestPublisher(hubbedContainer.Hub.Publish<ComponentStateRequest<SsrState>>);
            //    }
            //}

            return container;
        }

        //public static IServiceCollection RegisterSsrs<TSsr>(this IServiceCollection container) where TSsr : Ssr {
        //    //List<Ssr> ssrs = LocationHelper.SsrLocations.Select(l => {
        //    //    TSsr ssr = (TSsr)Activator.CreateInstance(typeof(TSsr), new object[] { Log.Logger });
        //    //    ssr.Location = l;
        //    //    return ssr;
        //    //}).ToList<Ssr>();

        //    return container.RegisterSsrs(ssrs);
        //}

        public static IServiceCollection RegisterSsrs(this IServiceCollection container, List<Ssr> preexistingSsrs = null) {
            //List<Ssr> ssrs = (preexistingSsrs == null) ?
            //    LocationHelper.SsrLocations.Select(l => new Ssr(Log.Logger) { Location = l }).ToList() :
            //    preexistingSsrs;
            //container.Collection.Register<Ssr>(ssrs);

            //if (container is HubbedContainer hubbedContainer) {
            //    foreach (var ssr in ssrs) {
            //        hubbedContainer.Hub.Subscribe<ComponentStateRequest<SsrState>>((Action<ComponentStateRequest<SsrState>>)ssr.ComponentStateRequestOccurred);
            //        ssr.ComponentStateChangePublisher(hubbedContainer.Hub.Publish<ComponentStateChange<SsrState>>);
            //    }
            //}

            return container;
        }

        //public static Container RegisterDisplays<TDisplay>(this Container container) where TDisplay : Display {

        //    var display = new LogDisplay(Log.Logger);
        //    container.Register<Display>(() => display);

        //    if (container is HubbedContainer hubbedContainer) {
        //        hubbedContainer.Hub.Subscribe<ComponentStateChange<ThermocoupleState>>(display.ComponentStateChangeOccurred);
        //        hubbedContainer.Hub.Subscribe<ComponentStateChange<PidState>>(display.ComponentStateChangeOccurred);
        //    }

        //    return container;
        //}

        public static IServiceCollection RegisterDisplays(this IServiceCollection container, List<Display> preexistingDisplays = null) {

            //List<Display> displays = (preexistingDisplays == null) ?
            //    new List<Display> { new LogDisplay(Log.Logger) } :
            //    preexistingDisplays;
            //container.Collection.Register<Display>(displays);

            ////var display = new LogDisplay(Log.Logger);
            ////container.Register<Display>(() => display);

            //if (container is HubbedContainer hubbedContainer) {
            //    foreach (var display in displays) {
            //        hubbedContainer.Hub.Subscribe<ComponentStateChange<ThermocoupleState>>((Action<ComponentStateChange<ThermocoupleState>>)display.ComponentStateChangeOccurred);
            //        hubbedContainer.Hub.Subscribe<ComponentStateChange<PidState>>((Action<ComponentStateChange<PidState>>)display.ComponentStateChangeOccurred);
            //    }
            //}

            return container;
        }

        public static IServiceCollection RefreshThermocouples(this IServiceCollection container) {
            //foreach (var thermo in container.GetAllInstances<IThermocouple>()) {
            //    thermo.Refresh();
            //}

            return container;
        }

        //public static TComp GetInstance<TComp>(this IServiceCollection container, Location location) where TComp : class, ILocatableComponent {
        //    return (TComp)(container.GetAllInstances<TComp>()).SingleOrDefault(c => c.Location == location);
        //}
    }

    //public class SerilogContextualLoggerInjectionBehavior : IDependencyInjectionBehavior {
    //    private readonly IDependencyInjectionBehavior _original;
    //    private readonly Container _container;

    //    public SerilogContextualLoggerInjectionBehavior(ContainerOptions options) {
    //        _original = options.DependencyInjectionBehavior;
    //        _container = options.Container;
    //    }

    //    public void Verify(InjectionConsumerInfo consumer) => _original.Verify(consumer);

    //    public InstanceProducer GetInstanceProducer(InjectionConsumerInfo i, bool t) =>
    //        i.Target.TargetType == typeof(ILogger)
    //            ? GetLoggerInstanceProducer(i.ImplementationType)
    //            : _original.GetInstanceProducer(i, t);

    //    private InstanceProducer<ILogger> GetLoggerInstanceProducer(Type type) =>
    //        Lifestyle.Transient.CreateProducer(() => Log.ForContext(type), _container);
    //}

    public static class InterfaceHelper { 
        public static bool Implements<I>(this Type type, I @interface) where I : class {
            if (((@interface as Type) == null) || !(@interface as Type).IsInterface)
                return false;
            return (@interface as Type).IsAssignableFrom(type);
        }

    }
}
