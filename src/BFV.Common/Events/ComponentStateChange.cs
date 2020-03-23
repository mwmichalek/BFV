using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {

    public interface IComponentStateChange { }


    public class ComponentStateChange<TState> : IComponentStateChange where TState : IComponentState {

        public Location Location { get; set; }

        public TState PriorState { get; set; }

        public TState CurrentState { get; set; }
    }

}
