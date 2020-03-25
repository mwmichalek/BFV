using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {

    public class ComponentStateChange<TState> where TState : IComponentState {

        public Location Location { get; set; }

        public TState PriorState { get; set; }

        public TState CurrentState { get; set; }
    }

}
