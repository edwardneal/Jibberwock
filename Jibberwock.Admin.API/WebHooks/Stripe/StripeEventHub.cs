using Stripe;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jibberwock.Admin.API.WebHooks.Stripe
{
    /// <summary>
    /// This class handles the event handling process. It enables me to decouple the Jibberwock-specific logic from the generic
    /// Application Insights event recording.
    /// </summary>
    public class StripeEventHub
    {
        private ConcurrentDictionary<string, ConcurrentBag<Func<IHasObject, Task>>> _events = new ConcurrentDictionary<string, ConcurrentBag<Func<IHasObject, Task>>>();

        // NB: this enables me to handle a Subscription, etc. in a type-safe, asynchronous manner.
        public StripeEventHub SubscribeEvent<T>(string eventName, Func<T, Task> handler) where T : class, IHasObject
        {
            var handlerList = _events.GetOrAdd(eventName.ToLower(), new ConcurrentBag<Func<IHasObject, Task>>());

            handlerList.Add(async (ih) => await handler(ih as T));

            return this;
        }

        // NB: I can't simply await Task.WhenAll because that could push the handlers onto separate threads - and having two threads
        // share a SQL connection doesn't work!
        public async Task RaiseEvent(Event stripeEvent)
        {
            var handlerList = _events.GetOrAdd(stripeEvent.Type.ToLower(), new ConcurrentBag<Func<IHasObject, Task>>());

            foreach (var handler in handlerList.ToArray())
                await handler(stripeEvent.Data.Object);
        }
    }
}
