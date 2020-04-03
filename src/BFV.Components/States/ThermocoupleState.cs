using BFV.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.States {

    public static class Temperature {

        public const int RoomTemp = 60;
        public const int BoilingTemp = 212;

    }

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
