using BFV.Common;
using BFV.Common.Events;
using BFV.Components.States;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.EventRecorders {

    public interface IEventRecorder : 
                            IRefreshableComponent,
                            IComponentStateChangeSubscriber<PumpState>,
                            IComponentStateChangeSubscriber<PidState>,
                            IComponentStateChangeSubscriber<ThermocoupleState>,
                            IComponentStateChangeSubscriber<SsrState> {
    }

    public class LoggerEventRecorder : IEventRecorder {

        protected readonly ILogger<LoggerEventRecorder> _logger;

        public LoggerEventRecorder(ILogger<LoggerEventRecorder> logger) {
            _logger = logger;
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<PumpState> stateChange) {
            _logger.LogInformation($"PumpState: {stateChange.Location} {stateChange.CurrentState.IsEngaged}");
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<PidState> stateChange) {
            _logger.LogInformation($"PidState: {stateChange.Location} {stateChange.CurrentState.IsEngaged} {stateChange.CurrentState.SetPoint}");
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<ThermocoupleState> stateChange) {
            _logger.LogInformation($"ThermocoupleState: {stateChange.Location} {stateChange.CurrentState.Temperature.ToString("F2")}");
        }

        public void ComponentStateChangeOccurred(ComponentStateChange<SsrState> stateChange) {
            _logger.LogInformation($"SsrState: {stateChange.Location} {stateChange.CurrentState.IsFiring} {stateChange.CurrentState.Percentage}");
        }

        public void Refresh() {
           
        }
    }
}
