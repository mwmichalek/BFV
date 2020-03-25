﻿using BFV.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common {
    public interface IComponent {

        

    }

    public interface IRefreshableComponent {

        void Refresh();

    }

    public interface ILocatableComponent {

        Location Location { get; set; }

    }

    public abstract class StateComponent<TState> : IComponent where TState : IComponentState {

        public TState PriorState { get; set; }

        public TState CurrentState { get; set; }

        public ComponentStateChange<TState> ToComponentStateChange() {
            if (this is ILocatableComponent) {
                return new ComponentStateChange<TState> {
                    Location = ((ILocatableComponent)this).Location,
                    PriorState = PriorState,
                    CurrentState = CurrentState
                };
            }

            return new ComponentStateChange<TState> {
                PriorState = PriorState,
                CurrentState = CurrentState
            };
        }

    }
}
