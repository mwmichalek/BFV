using BFV.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.States {
    public class SsrState : ComponentState<SsrState> {

        public bool IsEngaged { get => Percentage > 0; }

        public int Percentage { get; set; } = 0;

        public bool IsFiring { get; set; } = false;

        public override string ToString() {
            return $"SSR - %: {Percentage} IsFiring: {IsFiring}, Time: {Timestamp.Second}:{Timestamp.Millisecond}";
        }

        public override SsrState Clone() {
            return new SsrState {
                Percentage = Percentage,
                IsFiring = IsFiring
            };
        }

    }
}
