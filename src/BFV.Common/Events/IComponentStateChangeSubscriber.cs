using BFV.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {
    public interface IComponentStateChangeSubscriber<T> where T : IComponentStateChange {

        void ComponentStateChangeOccurred(T stateChange);

    }

    public interface IComponentStateChangeSubscriberNew<T> where T : IComponentState {

        void ComponentStateChangeOccurred(ComponentStateChange<T> stateChange);

    }
}
