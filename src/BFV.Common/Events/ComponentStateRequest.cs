using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {

    public interface IComponentStateRequest { }


    public class ComponentStateRequest<T> : IComponentStateRequest where T : IComponentState {

        public Location Location { get; set; }

        public T RequestedState { get; set; }

   
    }
}
