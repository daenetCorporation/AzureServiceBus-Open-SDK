using ServiceBus.OpenSdk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBus.OpenSdk
{
    /// <summary>
    /// Implementor of IoT protocol must implement this interface.
    /// </summary>
    public interface IIotTransport
    {
        Dictionary<string, object> Paremeters  { get; set; }

        Task Send(Message msg, SendOptions sendOptions);

        Task SendBatch(IEnumerable<Message> msg);


        Task Abandon(Message message);

        Task Complete(Message message);

        Task Abandon(Uri messageLockUri);

        Task Complete(Uri messageLockUri);

        Task<Message> Receive(ReceiveOptions receiveOptions);

        Task<Message> ReceiveBatch();

        Task<Message> ReceiveBatch(TimeSpan timeout);
    }
}
