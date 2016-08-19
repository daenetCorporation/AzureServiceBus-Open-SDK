namespace Microsoft.ServiceBus.Micro
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http.Headers;

    public class BrokeredMessage
    {
        readonly MessagingClient client;
        readonly string messageLockUri;
        public string ContentType;
        public object Object { get; set; }

        public BrokeredMessage(object obj = null)
        {
            this.Object = obj;
            this.client = null;
            this.messageLockUri = null;
            this.Properties = new Dictionary<object, object>();
            this.BrokerProperties = new Dictionary<object, object>();
        }

        internal BrokeredMessage(Stream bodyStream, string contentType, HttpResponseHeaders headers, MessagingClient client)
        {
            this.BodyStream = bodyStream;
            this.ContentType = contentType;
            this.Properties = new Dictionary<object, object>();
            this.BrokerProperties = new Dictionary<object, object>();

            foreach (var header in headers)
            {
                string val = header.Value.GetEnumerator().Current;

                if (header.Key.Equals("Location"))
                {                   
                    this.messageLockUri = val;
                }

                this.Properties.Add(header, val.Trim('\"'));
            }

            this.client = client;
        }

        public Dictionary<object, object> BrokerProperties { get; set; }
        public Dictionary<object, object> Properties { get; set; }
        public Stream BodyStream { get; set; }

        public void Complete()
        {
            if (this.client == null ||
                this.messageLockUri == null)
            {
                throw new InvalidOperationException();
            }
            this.client.Complete(new Uri(this.messageLockUri));
        }

        public void Abandon()
        {
            if (this.client == null ||
                this.messageLockUri == null)
            {
                throw new InvalidOperationException();
            }
            this.client.Abandon(new Uri(this.messageLockUri));
        }
    }
}