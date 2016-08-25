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
namespace ServiceBus.OpenSdk
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