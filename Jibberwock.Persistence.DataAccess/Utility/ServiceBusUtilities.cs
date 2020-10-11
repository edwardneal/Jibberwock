using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Jibberwock.Persistence.DataAccess.Utility
{
    /// <summary>
    /// Contains extension methods which allow an ad-hoc object to be sent as a message to a Service Bus QueueClient.
    /// </summary>
    internal static class ServiceBusUtilities
    {
        /// <summary>
        /// Generates a <see cref="Message"/> based upon a strongly-typed input object.
        /// </summary>
        /// <typeparam name="T">The type of object to serialise.</typeparam>
        /// <param name="inputObject">The contents of the message, to be serialised as a JSON object.</param>
        /// <param name="messageId">The client-defined message ID.</param>
        /// <returns>The generated message.</returns>
        public static Message GenerateMessage<T>(T inputObject, string messageId)
        {
            var messageBytes = JsonSerializer.SerializeToUtf8Bytes(inputObject);
            var msg = new Message(messageBytes) { MessageId = messageId };

            return msg;
        }

        /// <summary>
        /// Generates a <see cref="Message"/> based upon a strongly-typed input object.
        /// </summary>
        /// <typeparam name="T">The type of object to serialise.</typeparam>
        /// <param name="inputObject">The contents of the message, to be serialised as a JSON object.</param>
        /// <param name="messageId">The client-defined message ID.</param>
        /// <param name="deliveryDate">The date when the message will be delivered.</param>
        /// <returns>The generated message.</returns>
        public static Message GenerateMessage<T>(T inputObject, string messageId, DateTimeOffset? deliveryDate)
        {
            var msg = GenerateMessage(inputObject, messageId);

            if (deliveryDate.HasValue)
            { msg.ScheduledEnqueueTimeUtc = deliveryDate.Value.UtcDateTime; }

            return msg;
        }
    }
}
