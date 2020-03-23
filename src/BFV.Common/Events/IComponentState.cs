using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {
    public interface IComponentState {
    }

    public abstract class ComponentState : IComponentState {

        public DateTime Timestamp { get; set; } = DateTime.Now;

    }
}
