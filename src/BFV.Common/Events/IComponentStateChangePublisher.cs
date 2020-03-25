using BFV.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common {

    public interface IComponentStateChangePublisher<T> where T : IComponentState {

        void ComponentStateChangePublisher(Action<ComponentStateChange<T>> publishStateChange);

    }
}
