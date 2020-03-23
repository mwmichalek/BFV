using BFV.Common;
using BFV.Components.States;
using System;

namespace BFV.Components {


    public class Thermocouple : StateComponent<ThermocoupleState>, ILocatableComponent {
        public Location Location { get; set; }

    }
}
