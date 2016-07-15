//#define CERTS

// 
//  (c) Microsoft Corporation. See LICENSE.TXT file for licensing details
//  
namespace IotHub.OpenSdk
{
    //  using Microsoft.ServiceBus.Micro;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    //  using ServiceBus.OpenSdk;
    using System.Collections.Generic;
    using ServiceBus.OpenSdk;
    using System.Net.Http.Headers;
    using System.Linq;




    //  using Microsoft.SPOT.Net.Security;

    public class HttpTransport : IIotTransport
    {
#if CERTS
        static X509Certificate[] caCerts;
#endif

        //private Uri m_BaseAddress;
        //readonly TokenProvider m_TokenProvider;
        //private string m_EntityPath;
        private Dictionary<string, object> m_Paremeters = new Dictionary<string, object>();

        public Dictionary<string, object> Paremeters
        {
            get
            {
                return m_Paremeters;
            }

            set
            {
                m_Paremeters = value;
            }
        }

        protected Uri BaseAddress
        {
            get
            {
                if (this.Paremeters["baseAddress"] != null)
                    return this.Paremeters["baseAddress"] as Uri;
                else
                    return new Uri(String.Format("https://{0}.servicebus.windows.net", this.Paremeters["sbnamespace"] as string));
            }

            set
            {
                this.Paremeters["baseAddress"] = value;
            }
        }

        public TokenProvider TokenProvider
        {
            get
            {
                return this.Paremeters[MessagingClient.TOKEN_PROVIDER] as TokenProvider;
            }
        }

        public string EntityPath
        {
            get
            {
                return this.Paremeters[MessagingClient.ENTITY] as string;
            }

            set
            {
                this.Paremeters[MessagingClient.ENTITY] = value;
            }
        }

        static HttpTransport()
        {
#if CERTS
            caCerts = new[]
                          {
                              new X509Certificate(Resources.GetBytes(Resources.BinaryResources.gte)),
                              new X509Certificate(Resources.GetBytes(Resources.BinaryResources.mia)),
                              new X509Certificate(Resources.GetBytes(Resources.BinaryResources.mssi))
                          };
#endif
        }

        //public HttpTransport(string sbnamespace, string entityPath, TokenProvider tokenProvider)
        //{
        //    this.BaseAddress = new Uri(String.Format("https://{0}.servicebus.windows.net", sbnamespace));
        //    this.m_TokenProvider = tokenProvider;
        //    this.m_EntityPath = entityPath;
        //}

        public HttpTransport(Dictionary<string, object> args)
        {
            this.Paremeters = args;
            this.BaseAddress = new Uri(String.Format("https://{0}.servicebus.windows.net", args[MessagingClient.SBNAMESPACE] as string));
            //this.m_TokenProvider = args["tokenProvider"] as TokenProvider;
            //this.m_EntityPath = args["entityPath"] as string;
        }



        private async Task<Message> PeekLockReceive(TimeSpan timeout)
        {
            HttpClient httpClient = getClient();

            try
            {
                HttpContent c = new StringContent("");

                httpClient.DefaultRequestHeaders.Add("Authorization",
                 this.TokenProvider.GetToken(this.BaseAddress));

                httpClient.BaseAddress = new Uri(this.BaseAddress.AbsoluteUri);

                this.PreprocessRequest(httpClient, c);

                var str = String.Format("{0}/messages/head?timeout=" + (timeout.Ticks / TimeSpan.TicksPerSecond), this.EntityPath);

                var wq = await httpClient.PostAsync(str, c);

                if (wq.StatusCode == HttpStatusCode.Created)
                {
                    string ex = wq.Content.ToString();
                    return readFromResponse(await wq.Content.ReadAsStreamAsync(),
                        wq.Content.Headers.ContentType.MediaType,
                        wq.Headers);
                }
                else
                {
                    if (wq.StatusCode == HttpStatusCode.NoContent)
                        return null;
                    else
                        throw new Exception(String.Format("Failed with status {0}", wq.StatusCode));
                }
            }
            catch (WebException we)
            {
                if (we.Response == null)
                {
                    throw;
                }
                if (we.Status == WebExceptionStatus.Success &&
                    ((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.NoContent)
                {
                    return null;
                }
                throw;
            }
        }


        private async Task<Message> ReceiveAndDelete(TimeSpan timeout)
        {
            HttpClient httpClient = getClient();

            var str = String.Format(
                "{0}/messages/head?timeout=" + (timeout.Ticks / TimeSpan.TicksPerSecond), this.EntityPath);

            httpClient.DefaultRequestHeaders.Add("Authorization",
               this.TokenProvider.GetToken(this.BaseAddress));

            httpClient.BaseAddress = this.BaseAddress;

            this.PreprocessRequest(httpClient, null);

            var wq = await httpClient.DeleteAsync(str);
            try
            {

                if (wq.StatusCode == HttpStatusCode.OK &&
                    this.ValidateReceiveResponse(wq))
                {
                    return readFromResponse(await wq.Content.ReadAsStreamAsync(),
                        wq.Content.Headers.ContentType.MediaType,
                       wq.Headers);
                }
                else
                {
                    if (wq.StatusCode == HttpStatusCode.NoContent)
                        return null;
                    else
                        throw new Exception(String.Format("Failed with status {0}", wq.StatusCode));
                }
            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.Success &&
                    ((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.NoContent)
                {
                    return null;
                }
                throw;
            }
        }

        private bool ValidateReceiveResponse(HttpResponseMessage wr)
        {
            if (wr.RequestMessage.RequestUri.Scheme == "https")
            {
                return true;
            }
            else
            {
                return this.TokenProvider.ValidateResponse(wr);
            }
        }

        private void PreprocessRequest(HttpClient client, HttpContent content)
        {
            if (client.BaseAddress.Scheme == "http")
            {
                // content.Headers.Add("Endpoint", content.RequestUri.Host + ":" + (wq.RequestUri.Port == -1 ? 80 : wq.RequestUri.Port));
                this.TokenProvider.SignRequest(client, content);
            }
        }


        /// <summary>
        /// Deletes the message from entity
        /// </summary>
        /// <param name="messageLockUri"></param>
        /// <returns></returns>
        public async Task Complete(Uri messageLockUri)
        {
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent("");
            //var wq = this.CreateLockRequest("DELETE", messageLockUri);
            httpClient.DefaultRequestHeaders.Add("Date", DateTime.UtcNow.ToString("R"));
            httpClient.DefaultRequestHeaders.Add("Authorization", this.TokenProvider.GetToken(this.BaseAddress));
            httpClient.BaseAddress = this.BaseAddress;
            this.PreprocessRequest(httpClient, content);

            try
            {
                var wr = await httpClient.DeleteAsync(messageLockUri);
                if (wr.StatusCode !=
                    HttpStatusCode.OK)
                {
                    throw new Exception(wr.ReasonPhrase);
                }
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    //  we.Response.Close();
                }
                throw;
            }
        }

        public async Task Abandon(Uri messageLockUri)
        {
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent("");
            // var wq = this.CreateLockRequest("PUT", messageLockUri);
            httpClient.DefaultRequestHeaders.Add("Date", DateTime.UtcNow.ToString("R"));
            httpClient.DefaultRequestHeaders.Add("Authorization", this.TokenProvider.GetToken(this.BaseAddress));
            httpClient.BaseAddress = this.BaseAddress;
            this.PreprocessRequest(httpClient, content);

            try
            {
                var wr = await httpClient.PutAsync(messageLockUri, content);

                if (wr.StatusCode !=
                    HttpStatusCode.OK)
                {
                    throw new Exception(wr.ReasonPhrase);
                }
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    // we.Response.Close();
                }
                throw;
            }
        }

        public async Task Abandon(ServiceBus.OpenSdk.Message message)
        {
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent("");
            // var wq = this.CreateLockRequest("PUT", messageLockUri);
            string lockUri = message.Properties["Location"].ToString().Replace(this.BaseAddress.ToString(), "");
            httpClient.DefaultRequestHeaders.Add("Date", DateTime.UtcNow.ToString("R"));
            httpClient.DefaultRequestHeaders.Add("Authorization", this.TokenProvider.GetToken(this.BaseAddress));
            httpClient.BaseAddress = this.BaseAddress;
            this.PreprocessRequest(httpClient, content);

            try
            {
                var wr = await httpClient.PutAsync(lockUri, content);

                if (wr.StatusCode !=
                    HttpStatusCode.OK)
                {
                    throw new Exception(wr.ReasonPhrase);
                }
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    // we.Response.Close();
                }
                throw;
            }
        }

        public async Task Complete(ServiceBus.OpenSdk.Message message)
        {
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent("");
            string lockUri = message.Properties["Location"].ToString().Replace(this.BaseAddress.ToString(), "");
            httpClient.DefaultRequestHeaders.Add("Date", DateTime.UtcNow.ToString("R"));
            httpClient.DefaultRequestHeaders.Add("Authorization", this.TokenProvider.GetToken(this.BaseAddress));
            httpClient.BaseAddress = this.BaseAddress;
            this.PreprocessRequest(httpClient, content);
            try
            {
                var wr = await httpClient.DeleteAsync(lockUri);
                if (wr.StatusCode !=
                    HttpStatusCode.OK)
                {
                    throw new Exception(wr.ReasonPhrase);
                }
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    //  we.Response.Close();
                }
                throw;
            }
        }

        public async Task Send(Message message, SendOptions sendOptions)
        {
            HttpClient httpClient = new HttpClient();

            HttpContent content;
            var stream = message.GetBodyStream();
            if (stream != null)
                content = new StreamContent(stream);
            else
                content = new StringContent(String.Empty);

            //if (message.GetBody<Stream>() != null)
            //{
            //    // DataContractJsonSerializer ser = new DataContractJsonSerializer()
            //    content = new StreamContent(message.GetBody<Stream>());
            //}
            //else if (message.GetBody<object>() != null)
            //{
            //    DataContractJsonSerializer ser = new DataContractJsonSerializer(message.GetBody<object>().GetType());
            //    MemoryStream ms = new MemoryStream();
            //    ser.WriteObject(ms, message.GetBody<object>());
            //    ms.Position = 0;
            //    content = new StreamContent(ms);
            //}
            //else
            //    content = new StringContent(String.Empty);

            httpClient.DefaultRequestHeaders.Add("Date", DateTime.UtcNow.ToString("R"));

            if (message.Properties != null && message.Properties.Count > 0)
            {
                string brokerProperties = "";

                foreach (var brokerPropertyName in message.Properties.Keys)
                {
                    string brokerPropertyValue = message.Properties[brokerPropertyName] as string;
                    brokerProperties += "\"" + brokerPropertyName + "\" : \"" + brokerPropertyValue + "\",";
                }

                httpClient.DefaultRequestHeaders.Add("BrokerProperties", "{ " + brokerProperties.TrimEnd(',') + " }");
            }

            foreach (string header in message.Properties.Keys)
            {
                var value = message.Properties[header];
                string valueAsString;
                if (value == null)
                {
                    valueAsString = null;
                }
                else if (value is DateTime)
                {
                    valueAsString = ((DateTime)value).ToString("R");
                }
                else if (value is int || value is Int16 || value is Int32 || value is Int64
                        || value is uint || value is UInt16 || value is UInt32 || value is UInt64
                        || value is float || value is double || value is bool)
                {
                    valueAsString = value.ToString();
                }
                else
                {
                    valueAsString = "\"" + value.ToString() + "\"";
                }
                content.Headers.Add(header, valueAsString);
            }
            if (message.MessageId != null || message.MessageId != "")
                content.Headers.Add("MessageId", message.MessageId);
            try
            {
                httpClient.BaseAddress = new Uri(this.BaseAddress.AbsoluteUri);
                httpClient.DefaultRequestHeaders.Add("Authorization",
                    this.TokenProvider.GetToken(this.BaseAddress));

                this.PreprocessRequest(httpClient, content);

                var wr = await httpClient.PostAsync(String.Format("{0}/messages", this.EntityPath), content);

                if (wr.StatusCode != HttpStatusCode.OK &&
                    wr.StatusCode != HttpStatusCode.Created)
                {
                   throw new Exception(wr.ReasonPhrase);
                }
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    // we.Response.Close();
                }
                throw;
            }
        }

        public Task SendBatch(IEnumerable<Message> msg)
        {
            throw new NotImplementedException();
        }




        public Task<Message> Receive(ReceiveOptions receiveOptions)
        {
            if (receiveOptions != null)
            {
                if (receiveOptions.ReceiveMode == ReceiveMode.PeekLock)
                {
                    return this.PeekLockReceive(receiveOptions.TimeOut);
                }
                else
                {
                    return this.ReceiveAndDelete(receiveOptions.TimeOut);
                }
            }
            else
                return this.ReceiveAndDelete(receiveOptions.TimeOut);
        }

        public Task<Message> Receive(long sequenceNumber)
        {
            throw new NotImplementedException();
        }

        public Task<Message> Receive(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<Message> ReceiveBatch()
        {
            throw new NotImplementedException();
        }

        public Task<Message> ReceiveBatch(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        Task<Message> IIotTransport.ReceiveBatch()
        {
            throw new NotImplementedException();
        }

        Task<Message> IIotTransport.ReceiveBatch(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }


        private static HttpClient getClient()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Date = new DateTimeOffset(DateTime.Now.ToUniversalTime());

            return httpClient;
        }


        private static Message readFromResponse(Stream bodyStream,
            string contentType,
            HttpResponseHeaders headers)
        {
            Message msg = new Message(bodyStream)
            {
                ContentType = contentType,
                Properties = new Dictionary<string, object>(),
                BrokerProperties = new Dictionary<object, object>(),
            };
            
            foreach (var header in headers)
            {
                var enumerator = header.Value.GetEnumerator();

                //if (header.Key == "BrokerProperties")
                //{
                //    while (enumerator.MoveNext())
                //        msg = fillBrokeredProperties(enumerator.Current, msg);
                //}
                foreach (var val in header.Value)
                {
                    //TODO...
                    //If header key is equal to MessageId 
                    //Then add header value to msg.MessagedI
                    //Else
                    //Add the key and prop to msg.Properties

                    
                    if (header.Key == "MessageId")
                    {
                        msg.MessageId = val.Trim('\"');
                    }
                    else
                    {
                        msg.Properties.Add(header.Key, val.Trim('\"'));
                    }
                }
            }
            return msg;
        }

        private static Message fillBrokeredProperties(string properties, Message msg)
        {
            List<char> filteredChar = new List<char>();
            filteredChar.Add(',');
            filteredChar.Add(':');

            IEnumerable<char> chars = filteredChar;
            properties = new string(properties.Where(c => !chars.Contains(c)).ToArray());
            properties = properties.Replace('\"', '#').TrimStart('{').TrimEnd('}');
            string[] props = properties.Split('#');
            props = props.Where(val => val != "").ToArray();

            return msg;
        }

    }
}