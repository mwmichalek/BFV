using Serilog;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

using SimpleInjector.Advanced;

namespace BFV.Components {
    public static class ComponentRegistrator {


        public static Container ComponentRegistry() {
            return new Container().RegisterComponents();
        }

        public static Container RegisterComponents(this Container container) {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Trace()
                .CreateLogger();

            container.Options.DependencyInjectionBehavior = new SerilogContextualLoggerInjectionBehavior(container.Options);

            container.Register<ILogger>(() => Log.Logger);
            container.Register<Pid>();
            container.Register<LcdDisplay>();

            return container;

            //Log.Logger.Information("Testing!");

            //container.

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
