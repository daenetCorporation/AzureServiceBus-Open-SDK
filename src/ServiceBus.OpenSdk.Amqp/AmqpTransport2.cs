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
using Amqp;
using Amqp.Framing;
using Amqp.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ServiceBus.OpenSdk
{
    public class AmqpTransport : ServiceBus.OpenSdk.IIotTransport
    {

        private string sbNamespace;

        private string entity; 
        private ServiceBus.OpenSdk.TokenProvider sasToken;


        private Connection connection;
        private Session session;
        private SenderLink sender;

        private ReceiverLink receiver;

        public AmqpTransport(Dictionary<string, object> args)
        {
            if (args[ServiceBus.OpenSdk.MessagingClient.TOKEN_PROVIDER] == null)
                throw new ArgumentException("Argument 'tokenProvider' must be specified");       
            this.sbNamespace = args[ServiceBus.OpenSdk.MessagingClient.SBNAMESPACE].ToString();
            this.entity = args[ServiceBus.OpenSdk.MessagingClient.ENTITY].ToString();
            this.sasToken = args[ServiceBus.OpenSdk.MessagingClient.TOKEN_PROVIDER] as ServiceBus.OpenSdk.TokenProvider;

            Task.Run(async () =>
            {
                await this.CreateConnection();

            }).GetAwaiter().GetResult();                                                  
        }

        private async Task CreateConnection()
        {

            Address address = new Address(sbNamespace + ".servicebus.windows.net", 5671, null, null, "/", "amqps");

            Connection connection = new Connection(address);

            await putTokenAsync(connection);

            session = new Session(connection);

            this.receiver = new ReceiverLink(session, "receive-link", entity);
            this.sender = new SenderLink(session, "sender-link", entity);
        }

        public Dictionary<string, object> Paremeters { get; set; }

        public Task CloseConnection()
        {
            var t = Task.Run(() =>
            {
                this.receiver.Close();
                this.sender.Close();
                this.session.Close();
                this.connection.Close();
            });

            return t;
        }
        public Task Abandon(Uri messageLockUri)
        {

            throw new NotImplementedException();
        }

        public Task Complete(Uri messageLockUri)
        {

            throw new NotImplementedException();
        }

        public async Task Abandon(ServiceBus.OpenSdk.Message message)
        {

            receiver.Release((Amqp.Message)message.amqpMessage);
        }


        public async Task Complete(ServiceBus.OpenSdk.Message message)
        {
            receiver.Accept((Amqp.Message)message.amqpMessage);
        }

        public async Task<ServiceBus.OpenSdk.Message> Receive(ServiceBus.OpenSdk.ReceiveOptions receiveOptions)
        { 
            var message = receiver.Receive();

            if (message == null)
                return null;
            //            var message = receiver.Receive();

            ServiceBus.OpenSdk.Message msg = map(message); 
            if (receiveOptions != null)
            {
                if (receiveOptions.ReceiveMode == ServiceBus.OpenSdk.ReceiveMode.ReceiveAndDelete)
                {
                    receiver.Accept(message);
                }
            }
            else
            {
                receiver.Release(message);
            }

            msg.amqpMessage = message;     
            return msg;   
        }

        public Task<ServiceBus.OpenSdk.Message> ReceiveBatch()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceBus.OpenSdk.Message> ReceiveBatch(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task Send(ServiceBus.OpenSdk.Message msg, ServiceBus.OpenSdk.SendOptions sendOptions)
        {
            var t = new Task(() =>
            {
                int timeOut = Convert.ToInt32(((TimeSpan)sendOptions.Properties["Timeout"]).TotalSeconds);
                this.sender.Send(map(msg));

            });

            t.Start();
            return t;
        }

        public async Task SendBatch(IEnumerable<ServiceBus.OpenSdk.Message> msg)
        {
            var sendOptions = new ServiceBus.OpenSdk.SendOptions();

            foreach (ServiceBus.OpenSdk.Message message in msg)
            {
                await this.Send(message, sendOptions);
            }
        }

        /// <summary>
        /// map from ServiceBus.OpenSdk.Message to Amqp.Message
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private Amqp.Message map(ServiceBus.OpenSdk.Message msg)
        {
            Amqp.Message message = new Amqp.Message(msg.GetBody<object>());
            message.Properties = new Amqp.Framing.Properties();
            message.Header = new Amqp.Framing.Header();
            message.ApplicationProperties = new ApplicationProperties();
            if (msg.CorrelationId != null)
                message.Properties.CorrelationId = msg.CorrelationId;
            if (msg.DeliveryCount != default(int))
                message.Header.DeliveryCount = Convert.ToUInt32(msg.DeliveryCount);
            if (msg.MessageId != null)
                message.Properties.MessageId = msg.MessageId;
            if (msg.ReplyTo != null)
                message.Properties.ReplyTo = msg.ReplyTo;
            if (msg.To != null)
                message.Properties.To = msg.To;
            if (msg.SessionId != null)
                message.Properties.GroupId = msg.SessionId;
            if (msg.ReplyToSessionId != null)
                message.Properties.ReplyToGroupId = msg.ReplyToSessionId;
            message.MessageAnnotations = new MessageAnnotations();
            message.MessageAnnotations[new Symbol("x-opt-sequence-number")] = msg.EnqueuedSequenceNumber;
            message.MessageAnnotations[new Symbol("x-opt-enqueued-time")] = msg.EnqueuedTimeUtc;
            message.MessageAnnotations[new Symbol("x-opt-scheduled-enqueue-time")] = msg.ScheduledEnqueueTimeUtc;
            message.MessageAnnotations[new Symbol("x-opt-partition-key")] = msg.PartitionKey;
            message.MessageAnnotations[new Symbol("x-opt-sequence-number")] = Convert.ToUInt32(msg.SequenceNumber);
            if (msg.EnqueuedTimeUtc != default(DateTime))
                message.Properties.AbsoluteExpiryTime = msg.EnqueuedTimeUtc;
            if (msg.TimeToLive.TotalMilliseconds != default(double))
                message.Header.Ttl = Convert.ToUInt32(msg.TimeToLive.TotalMilliseconds);

            if (msg.Properties != null && msg.Properties.Keys != null)
            {
                if (msg.Properties.ContainsKey("Priority") && msg.Properties["Priority"] != null)
                {
                    message.Header.Priority = Convert.ToByte(Convert.ToInt32(msg.Properties["Priority"].ToString()));
                }

                foreach (string key in msg.Properties.Keys)
                {
                    message.ApplicationProperties[key] = msg.Properties[key];
                }
            }

            return message;
        }


        /// <summary>
        /// map from Amqp.Message to ServiceBus.OpenSdk.Message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private ServiceBus.OpenSdk.Message map(Amqp.Message message)
        {
            if (message == null)
                return null;

            ServiceBus.OpenSdk.Message msg = new ServiceBus.OpenSdk.Message((object)message.Body);

            if (message.Properties.CorrelationId != null)
                msg.CorrelationId = message.Properties.CorrelationId;
            if (message.Header.DeliveryCount != default(uint))
                msg.DeliveryCount = Convert.ToInt32(message.Header.DeliveryCount);
            if (message.Properties.MessageId != null)
                msg.MessageId = message.Properties.MessageId;
            if (message.Properties.ReplyTo != null)
                msg.ReplyTo = message.Properties.ReplyTo;
            if (message.Properties.To != null)
                msg.To = message.Properties.To;
            if (message.Properties.GroupId != null)
                msg.SessionId = message.Properties.GroupId;
            if (message.Properties.ReplyToGroupId != null)
                msg.ReplyToSessionId = message.Properties.ReplyToGroupId;
            if (message.MessageAnnotations[new Symbol("x-opt-sequence-number")] != null)
                msg.EnqueuedSequenceNumber = (long)message.MessageAnnotations[new Symbol("x-opt-sequence-number")];
            if (message.MessageAnnotations[new Symbol("x-opt-enqueued-time")] != null)
                msg.EnqueuedTimeUtc = (DateTime)message.MessageAnnotations[new Symbol("x-opt-enqueued-time")];
            if (message.MessageAnnotations[new Symbol("x-opt-scheduled-enqueue-time")] != null)
                msg.ScheduledEnqueueTimeUtc = (DateTime)message.MessageAnnotations[new Symbol("x-opt-scheduled-enqueue-time")];
            if (message.MessageAnnotations[new Symbol("x-opt-partition-key")] != null)
                msg.PartitionKey = (string)message.MessageAnnotations[new Symbol("x-opt-partition-key")];
            if (message.MessageAnnotations[new Symbol("x-opt-sequence-number")] != null)
                msg.SequenceNumber = (long)message.MessageAnnotations[new Symbol("x-opt-sequence-number")];
            if (message.Properties.AbsoluteExpiryTime != default(DateTime))
                msg.EnqueuedTimeUtc = message.Properties.AbsoluteExpiryTime;
            if (message.Header.Ttl != default(uint))
                msg.TimeToLive = TimeSpan.FromMilliseconds(message.Header.Ttl);
            if (message.Header.Priority != default(byte))
                msg.Properties["Priority"] = Convert.ToInt32(message.Header.Priority);


            if (message.ApplicationProperties != null)
            {   

                foreach (string key in message.ApplicationProperties.Map.Keys)
                { 
                    msg.Properties[key] = (object)message.ApplicationProperties[key];
                }
            }

            if (message.DeliveryTag != null && message.MessageAnnotations[new Symbol("x-opt-sequence-number")] != null)
            {
                long sn = (long)message.MessageAnnotations[new Symbol("x-opt-sequence-number")];
                Guid lockToken = new Guid(message.DeliveryTag);
                string host = sbNamespace + ".servicebus.windows.net";
                string lockLocation = string.Format("https://{0}/{1}/mesages/{2}/{3}", host, entity, sn, lockToken);
                msg.Properties.Add("Location", lockLocation);
            }

            return msg;

        }

        private Task putTokenAsync(Connection connection)
        {
            var t = Task.Run(() =>
            {
                var session = new Session(connection);

                string cbsClientAddress = "cbs-client-reply-to";
                SenderLink cbsSender = new SenderLink(session, "cbs-sender", "$cbs");
                var receiverAttach = new Attach()
                {
                    Source = new Source() { Address = "$cbs" },
                    Target = new Target() { Address = cbsClientAddress }
                };

                ReceiverLink cbsReceiver = new ReceiverLink(session, "cbs-receiver", receiverAttach, null); 
                var sasToken = this.sasToken.GetToken(new Uri(string.Format("http://{0}/{1}", sbNamespace + ".servicebus.windows.net", entity)));
                var request = new Amqp.Message(sasToken);
                request.Properties = new Properties();
                request.Properties.MessageId = "1";
                request.Properties.ReplyTo = cbsClientAddress;
                request.ApplicationProperties = new ApplicationProperties();
                request.ApplicationProperties["operation"] = "put-token";
                request.ApplicationProperties["type"] = "servicebus.windows.net:sastoken"; 
                request.ApplicationProperties["name"] = string.Format("amqp://{0}/{1}", sbNamespace + ".servicebus.windows.net", entity);

                cbsSender.Send(request);  
                var response = cbsReceiver.Receive();
                if (response == null || response.Properties == null || response.ApplicationProperties == null)
                {
                    throw new Exception("invalid response received");
                }

                int statusCode = (int)response.ApplicationProperties["status-code"];
                if (statusCode != (int)HttpStatusCode.Accepted && statusCode != (int)HttpStatusCode.OK)
                {
                    throw new Exception("put-token message was not accepted. Error code: " + statusCode);
                } 
                cbsSender.Close();
                cbsReceiver.Close();
                session.Close();
            });

            return t;
        }
    }
}
