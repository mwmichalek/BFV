using BFV.Common.Events;
using BFV.Components;
using BFV.Components.States;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Test.Components.TestComponents {
    //public class TestSsr : Ssr {

    //    public TestSsr(ILogger logger) : base(logger) { }

    //    public bool SsrStateRequestOccured { get; set; }

    //    public ComponentStateRequest<SsrState> LastSsrStateRequest { get; set; }

    //    public override void ComponentStateRequestOccurred(ComponentStateRequest<SsrState> stateRequest) {
    //        base.ComponentStateRequestOccurred(stateRequest);

    //        if (stateRequest.Location == Location) {
    //            _logger.Debug($"ComponentStateChange<ThermocoupleState> Occurred.");
    //            SsrStateRequestOccured = true;
    //            LastSsrStateRequest = stateRequest;
    //        }
    //    }

    //}
}
