using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PubSub;

namespace BFV.Components.Hub {

    public interface IHub {
        bool Exists<T>();
        bool Exists<T>(object subscriber);
        bool Exists<T>(object subscriber, Action<T> handler);
        void Publish<T>(T data = default);

        Task PublishAsync<T>(T data = default);
        void Subscribe<T>(Action<T> handler);
        void Subscribe<T>(object subscriber, Action<T> handler);
        void Subscribe<T>(Func<T, Task> handler);
        void Subscribe<T>(object subscriber, Func<T, Task> handler);
        void Unsubscribe();
        void Unsubscribe(object subscriber);
        void Unsubscribe<T>();
        void Unsubscribe<T>(Action<T> handler);
        void Unsubscribe<T>(object subscriber, Action<T> handler = null);
    }

    public class PubSubHub : PubSub.Hub, IHub {
    }
}
