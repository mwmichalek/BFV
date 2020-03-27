using BFV.Common;
using BFV.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Components.States {
    public class PidState : ComponentState<PidState> {

        public bool IsEngaged { get; set; } = false;

        public PidMode PidMode { get; set; } = PidMode.Temperature;

        public double SetPoint { get; set; } = double.MinValue;

        public double Temperature { get; set; } = double.MinValue;

        public double GainProportional { get; set; } = 1;

        public double GainIntegral { get; set; } = 1;

        public double GainDerivative { get; set; } = 1;

        public override string ToString() {
            return $"PID - SetPoint: {SetPoint} Time: {Timestamp.Second}:{Timestamp.Millisecond}";
        }

        public override PidState Clone() {
            return new PidState {
                IsEngaged = IsEngaged,
                PidMode = PidMode,
                SetPoint = SetPoint,
                Temperature = Temperature,
                GainProportional = GainProportional,
                GainIntegral = GainIntegral,
                GainDerivative = GainDerivative
            };
        }

    }
}
