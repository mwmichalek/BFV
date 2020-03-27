using BFV.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.States {
    public class PumpState : ComponentState<PumpState> {

        public bool IsEngaged { get; set; } = false;

        public override PumpState Clone() {
            return new PumpState {
                IsEngaged = IsEngaged
            };
        }

    }
}
