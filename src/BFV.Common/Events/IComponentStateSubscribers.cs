using BFV.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BFV.Common.Events {

    public interface IComponentStateChangeSubscriber<T> where T : IComponentState {

        void ComponentStateChangeOccurred(ComponentStateChange<T> stateChange);

    }

    public interface IComponentStateRequestSubscriber<T> where T : IComponentState {

        void ComponentStateRequestOccurred(ComponentStateRequest<T> stateRequest);

    }
}
