using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {

    public interface IComponentState {

        DateTime Timestamp { get; set; }
    }

    public abstract class ComponentState<TSubclass> : IComponentState where TSubclass : IComponentState {

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public abstract TSubclass Clone();

    }

}
