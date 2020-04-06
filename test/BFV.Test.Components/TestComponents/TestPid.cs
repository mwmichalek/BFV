using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Test.Components.TestComponents {
    //public class TestPid : Pid {

    //    public TestPid(ILogger logger) : base(logger) { }

    //    public bool ThermocoupleStateChangeOccured { get; set; }

    //    public ComponentStateChange<ThermocoupleState> LastThermocoupleStateChange { get; set; }

    //    public override void ComponentStateChangeOccurred(ComponentStateChange<ThermocoupleState> stateChange) {
    //        base.ComponentStateChangeOccurred(stateChange);

    //        if (stateChange.Location == Location) {
    //            _logger.Debug($"ComponentStateChange<ThermocoupleState> Occurred.");
    //            ThermocoupleStateChangeOccured = true;
    //            LastThermocoupleStateChange = stateChange;
    //        }
    //    }

    //    public bool PidStateRequestOccured { get; set; }

    //    public ComponentStateRequest<PidState> LastPidStateRequest { get; set; }

    //    public override void ComponentStateRequestOccurred(ComponentStateRequest<PidState> stateRequest) {
    //        base.ComponentStateRequestOccurred(stateRequest);

    //        if (stateRequest.Location == Location) {
    //            _logger.Debug($"ComponentStateRequest<PidState> Occurred.");
    //            PidStateRequestOccured = true;
    //            LastPidStateRequest = stateRequest;
    //        }

    //    }



        //public bool 
    //}
}
