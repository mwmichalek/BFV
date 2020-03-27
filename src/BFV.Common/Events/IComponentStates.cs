using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {

    public interface IComponentState {
    }

    public abstract class ComponentState<TSubclass> : IComponentState where TSubclass : IComponentState {

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public abstract TSubclass Clone();

    }

    //public interface IComponentRequestedState {
    //}

    //public abstract class ComponentRequestedState<TSubclass> : IComponentState where TSubclass : IComponentState {

    //    public DateTime Timestamp { get; set; } = DateTime.Now;

    //    public abstract TSubclass Clone();

    //}

}
