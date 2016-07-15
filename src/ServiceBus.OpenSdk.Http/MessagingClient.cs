//#define CERTS

// 
//  (c) Microsoft Corporation. See LICENSE.TXT file for licensing details
//  
namespace Microsoft.ServiceBus.Micro
{
    using global::ServiceBus.OpenSdk;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;

    //  using Microsoft.SPOT.Net.Security;

    public class MessagingClient
    {
#if CERTS
        static X509Certificate[] caCerts;
#endif
        readonly Uri m_BaseAddress;
        readonly TokenProvider tokenProvider;
        private string m_EntityPath;

        static MessagingClient()
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

        public MessagingClient(string sbnamespace, string entityPath, TokenProvider tokenProvider)
        {
            this.m_BaseAddress = new Uri( String.Format("https://{0}.servicebus.windows.net", sbnamespace));
            this.tokenProvider = tokenProvider;
            this.m_EntityPath = entityPath;
        }

        public Task<BrokeredMessage> Receive(TimeSpan timeout, ReceiveMode receiveMode)
        {
            if (receiveMode == ReceiveMode.PeekLock)
            {
                return this.PeekLockReceive(timeout);
            }
            else
            {
                return this.ReceiveAndDelete(timeout);
            }
        }

        private async Task<BrokeredMessage> PeekLockReceive(TimeSpan timeout)
        {
            HttpClient httpClient = getClient();

            //var wq = this.CreateReceiveRequest(timeout);
            //wq.Method = "POST";
            //wq.Headers.Add("Date", DateTime.UtcNow.ToString("R"));
            //wq.ContentLength = 0;
            try
            {
                HttpContent c = new StringContent("");

                this.PreprocessRequest(httpClient, c);

                var wq = await httpClient.PostAsync(this.m_BaseAddress.AbsoluteUri + "/messages/head?timeout=" + (timeout.Ticks / TimeSpan.TicksPerSecond), null);

                if (wq.StatusCode == HttpStatusCode.Created)
                {
                    return new BrokeredMessage(await wq.Content.ReadAsStreamAsync(),
                        wq.Content.Headers.ContentType.MediaType,
                        wq.Headers, this);
                }
                else
                {
                    return null;
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

        private static HttpClient getClient()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Date = new DateTimeOffset(DateTime.Now.ToUniversalTime());

            return httpClient;
        }

        async Task<BrokeredMessage> ReceiveAndDelete(TimeSpan timeout)
        {

            HttpClient httpClient = getClient();
            // wq.Method = "DELETE";
            //wq.Headers.Add("Date", DateTime.UtcNow.ToString("R"));
            //this.PreprocessRequest(wq);
            //wq.ContentLength = 0;
            var wq = await httpClient.DeleteAsync(this.m_BaseAddress.AbsoluteUri + "/messages/head?timeout=" + (timeout.Ticks / TimeSpan.TicksPerSecond));
            try
            {

                if (wq.StatusCode == HttpStatusCode.OK &&
                    this.ValidateReceiveResponse(wq))
                {
                    return new BrokeredMessage(await wq.Content.ReadAsStreamAsync(), wq.Content.Headers.ContentType.MediaType,
                       wq.Headers, this);
                }
                else
                {
                    return null;
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

        bool ValidateReceiveResponse(HttpResponseMessage wr)
        {
            if (wr.RequestMessage.RequestUri.Scheme == "https")
            {
                return true;
            }
            else
            {
                return this.tokenProvider.ValidateResponse(wr);
            }
        }

        void PreprocessRequest(HttpClient client, HttpContent content)
        {
            if (client.BaseAddress.Scheme == "http")
            {
                // content.Headers.Add("Endpoint", content.RequestUri.Host + ":" + (wq.RequestUri.Port == -1 ? 80 : wq.RequestUri.Port));
                this.tokenProvider.SignRequest(client, content);
            }
        }

        public async Task Send(Message message, SendOptions sendOptions)
        {
            HttpClient httpClient = new HttpClient();
            
            HttpContent content;
            if (message.GetBody<Stream>() != null)
            {
                // DataContractJsonSerializer ser = new DataContractJsonSerializer()
                content = new StreamContent(message.GetBody<Stream>());
            }
            else if (message.GetBody<object>() != null)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(message.GetBody<object>().GetType());
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, message.GetBody<object>());
                ms.Position = 0;
                content = new StreamContent(ms);
            }
            else
                content = new StringContent(String.Empty);

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

            var requestUriString = this.m_BaseAddress.AbsoluteUri + "/messages";
        
            try
            {
                httpClient.BaseAddress = new Uri(this.m_BaseAddress.AbsoluteUri);
                httpClient.DefaultRequestHeaders.Add("Authorization", 
                    this.tokenProvider.GetToken(this.m_BaseAddress));

                this.PreprocessRequest(httpClient, content);

                var wr = await httpClient.PostAsync(String.Format("{0}/messages", m_EntityPath), content);
             
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

        public async void Complete(Uri messageLockUri)
        {
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent("");
            //var wq = this.CreateLockRequest("DELETE", messageLockUri);
            httpClient.DefaultRequestHeaders.Add("Date", DateTime.UtcNow.ToString("R"));
            httpClient.DefaultRequestHeaders.Add("Authorization", this.tokenProvider.GetToken(this.m_BaseAddress));
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

        public async void Abandon(Uri messageLockUri)
        {
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent("");
            // var wq = this.CreateLockRequest("PUT", messageLockUri);
            httpClient.DefaultRequestHeaders.Add("Date", DateTime.UtcNow.ToString("R"));
            httpClient.DefaultRequestHeaders.Add("Authorization", this.tokenProvider.GetToken(this.m_BaseAddress));
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

        //HttpWebRequest CreateSendRequest()
        //{
        //    var requestUriString = this.entityAddress.AbsoluteUri + "/messages";
        //    var wq = (HttpWebRequest) WebRequest.Create(requestUriString);
        //    wq.ProtocolVersion = HttpVersion.Version11;
        //    wq.KeepAlive = true;
        //    if (wq.RequestUri.Scheme == "https")
        //    {
        //        wq.Headers.Add("Authorization", this.tokenProvider.GetToken(this.entityAddress));
        //    }
        //    return wq;
        //}

        //HttpClient CreateReceiveRequest(TimeSpan timeout)
        //{
        //     var wq = (HttpWebRequest) WebRequest.Create(this.entityAddress.AbsoluteUri + "/messages/head?timeout=" + (timeout.Ticks/TimeSpan.TicksPerSecond));
        //    wq.ProtocolVersion = HttpVersion.Version11;
        //    wq.KeepAlive = true;
        //    if (wq.RequestUri.Scheme == "https")
        //    {
        //        wq.Headers.Add("Authorization", this.tokenProvider.GetToken(this.entityAddress));
        //    }
        //    return wq;
        //}

        //HttpWebRequest CreateLockRequest(string method, Uri lockUri)
        //{
        //    var wq = (HttpWebRequest) WebRequest.Create(lockUri);
        //    wq.ProtocolVersion = HttpVersion.Version11;
        //    wq.KeepAlive = true;
        //    wq.Method = method;
        //    if (wq.RequestUri.Scheme == "https")
        //    {
        //        wq.Headers.Add("Authorization", this.tokenProvider.GetToken(this.entityAddress));
        //    }
        //    return wq;
        //}
    }
}