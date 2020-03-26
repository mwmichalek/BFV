using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Test.Components.TestComponents {
    public class TestPid : Pid {

        public bool ThermocoupleStateChangeOccured { get; set; }

        public ComponentStateChange<ThermocoupleState> LastThermocoupleStateChange { get; set; }

        public TestPid(ILogger logger) : base(logger) { }

        public override void ComponentStateChangeOccurred(ComponentStateChange<ThermocoupleState> stateChange) {
            if (stateChange.Location == Location) {
                _logger.Debug($"ComponentStateChange<ThermocoupleState> Occurred.");
                ThermocoupleStateChangeOccured = true;
                LastThermocoupleStateChange = stateChange;
            }
        }

    }
}
