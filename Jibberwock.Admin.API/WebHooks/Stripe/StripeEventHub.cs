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
        private ConcurrentDictionary<string, ConcurrentBag<Func<IServiceProvider, IHasObject, Task>>> _events = new ConcurrentDictionary<string, ConcurrentBag<Func<IServiceProvider, IHasObject, Task>>>();

        // NB: this enables me to handle a Subscription, etc. in a type-safe, asynchronous manner.
        public StripeEventHub SubscribeEvent<T>(string eventName, Func<IServiceProvider, T, Task> handler) where T : class, IHasObject
        {
            var handlerList = _events.GetOrAdd(eventName.ToLower(), new ConcurrentBag<Func<IServiceProvider, IHasObject, Task>>());

            handlerList.Add(async (iSP, ih) => await handler(iSP, ih as T));

            return this;
        }

        // NB: I can't simply await Task.WhenAll because that could push the handlers onto separate threads - and having two threads
        // share a SQL connection doesn't work!
        public async Task RaiseEvent(IServiceProvider serviceProvider, Event stripeEvent)
        {
            var handlerList = _events.GetOrAdd(stripeEvent.Type.ToLower(), new ConcurrentBag<Func<IServiceProvider, IHasObject, Task>>());

            foreach (var handler in handlerList.ToArray())
                await handler(serviceProvider, stripeEvent.Data.Object);
        }
    }
}
