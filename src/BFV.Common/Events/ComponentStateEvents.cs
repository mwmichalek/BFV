using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {
    public class ComponentStateChange<TState> where TState : IComponentState {

        public Location Location { get; set; }

        public TState PriorState { get; set; }

        public TState CurrentState { get; set; }
    }

    /// <summary>
    /// This probably won't work remotely, perhaps have different requests for remote that get converted to this.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class ComponentStateRequest<TState> where TState : IComponentState {

        public Location Location { get; set; }

        public Action<TState> Updates { get; set; }

        public TState UpdateState(TState initialState) {
            Updates(initialState);
            initialState.Timestamp = DateTime.Now;
            return initialState;
        }

    }
}
