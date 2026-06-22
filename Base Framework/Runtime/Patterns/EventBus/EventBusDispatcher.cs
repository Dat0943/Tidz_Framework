using System;
using System.Collections.Generic;

namespace Tidz
{
    static class EventBusDispatcher<T> where T : IEvent
    {
        static readonly Dictionary<EventBus, Action<T>> s_Actions = new Dictionary<EventBus, Action<T>>();

        public static void Subscribe(EventBus bus, Action<T> listener)
        {
            s_Actions.TryGetValue(bus, out var existing);
            s_Actions[bus] = existing + listener;
        }

        public static void Unsubscribe(EventBus bus, Action<T> listener)
        {
            if (s_Actions.ContainsKey(bus))
            {
                s_Actions[bus] -= listener;
            }
        }

        public static void Dispatch(EventBus bus, T @event)
        {
            if (s_Actions.TryGetValue(bus, out var action))
            {
                action.Invoke(@event);
            }
        }
    }
}
