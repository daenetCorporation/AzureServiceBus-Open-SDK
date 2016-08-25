//=======================================================================================
// Copyright © daenet GmbH Frankfurt am Main
//
// LICENSED UNDER THE APACHE LICENSE, VERSION 2.0 (THE "LICENSE"); YOU MAY NOT USE THESE
// FILES EXCEPT IN COMPLIANCE WITH THE LICENSE. YOU MAY OBTAIN A COPY OF THE LICENSE AT
// http://www.apache.org/licenses/LICENSE-2.0
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER THE
// LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED. SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING
// PERMISSIONS AND LIMITATIONS UNDER THE LICENSE.
//=======================================================================================
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
