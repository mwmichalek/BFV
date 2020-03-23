using BFV.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common {
    public interface IComponent {
    }

    public abstract class Component<TState> : IComponent where TState : IComponentState {

        public TState PriorState { get; set; }

        public TState CurrentState { get; set; }

    }
}
