using BFV.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.States {
    public class ThermocoupleState : ComponentState<ThermocoupleState> {

        public double Temperature { get; set; }

        public override ThermocoupleState Clone() {
            return new ThermocoupleState {
                Temperature = Temperature,
                Timestamp = Timestamp
            };
        }



        public override string ToString() {
            return $"Temperature: {Temperature} Time: {Timestamp.Second}:{Timestamp.Millisecond}";
        }

    }
}
