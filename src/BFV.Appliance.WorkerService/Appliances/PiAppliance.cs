using BFV.Appliance.WorkerService.Appliances;
using BFV.Common;
using BFV.Common.Events;
using BFV.Components;
using BFV.Components.Appliances;
using BFV.Components.EventRecorders;
using BFV.Components.Hub;
using BFV.Components.Pi.Ssrs;
using BFV.Components.Ssrs;
using BFV.Components.States;
using BFV.Components.Thermocouples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appliance.WorkerService.Appliances {
    public class PiAppliance : BaseAppliance {


            public PiAppliance(ILogger<BaseAppliance> logger,
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
                                      ISsr ssr2,
                                      IEventRecorder eventRecorder)
                                    : base(logger) {
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

                eventRecorders.Add(eventRecorder);

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

        }

        public static class PiApplianceHelper {

            public static IServiceCollection RegisterPiApplianceComponents(this IServiceCollection services, IHub hub = null) {

            services.AddTransient<IThermocouple, SimulationThermocouple>();
            services.AddTransient<IPid, Pid>();
            services.AddTransient<ISsr, PiSsr>();
            services.AddSingleton<IEventRecorder, LoggerEventRecorder>();
            services.AddSingleton<IAppliance, SimulatedAppliance>();

            if (hub == null)
                    services.AddSingleton<IHub>((serviceProvider) => new PubSubHub());
                else
                    services.AddSingleton<IHub>((serviceProvider) => hub);

                return services;
            }

        }
    }
