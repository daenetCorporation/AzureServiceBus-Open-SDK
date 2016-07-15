//#define CERTS                  
// 
//  (c) Microsoft Corporation. See LICENSE.TXT file for licensing details
//  
namespace ServiceBus.OpenSdk
{
    using IotHub.OpenSdk;
    using System;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    
    public class MessagingClient
    {
        public const string cKeyTokenProvider = "tokenProvider";
#if CERTS
        static X509Certificate[] caCerts;
#endif

        protected Uri BaseAddress
        {
            get
            {
                return m_Transport.Paremeters["baseAddress"] as Uri;
            }
            set
            {
                m_Transport.Paremeters["baseAddress"] = value;
            }
        }


        private readonly IIotTransport m_Transport;

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

        public const string SBNAMESPACE = "sbNamespace";
        public const string KEY_VALUE = "keyValue";
        public const string KEY_NAME = "keyName";
        public const string TOKEN_PROVIDER = "tokenProvider";
        public const string ENTITY = "entity";
        public const string PROTOCOL = "protocol";

        public MessagingClient(string sbnamespace, string entityPath,
            TokenProvider tokenProvider, string protocol)
        {
            //this.BaseAddress = new Uri(String.Format("https://{0}.servicebus.windows.net", sbnamespace));
            //this.tokenProvider = tokenProvider;
            //this.m_EntityPath = entityPath;

            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add(SBNAMESPACE, sbnamespace);
            args.Add(ENTITY, entityPath);
            args.Add(TOKEN_PROVIDER, tokenProvider);

            
            m_Transport = getInstance(args, protocol);
        }


        protected IIotTransport getInstance(Dictionary<string, object> args, string protocol)
        {
            if (protocol != null && protocol.ToLower() == "http")
            {

                AssemblyName asmName = new AssemblyName("IotHub.Http");
                //AssemblyName asmName = new AssemblyName("IotHub.Http.Win32");
                var asm = Assembly.Load(asmName);

                var itransportType = asm.GetType("IotHub.OpenSdk.HttpTransport");
                IIotTransport transportInstance = Activator.CreateInstance(itransportType, args) as IIotTransport;
                return transportInstance;
            }
            else if (protocol != null && protocol.ToLower() == "amqp")
            {
                AssemblyName asmName = new AssemblyName("IotHub.Amqp");
                //AssemblyName asmName = new AssemblyName("IotHub.Amqp.Win32");
                var asm = Assembly.Load(asmName);
                var itransportType = asm.GetType("IotHub.OpenSdk.AmqpTransport");
                IIotTransport transportInstance = Activator.CreateInstance(itransportType, args) as IIotTransport;
                return transportInstance;
            }
            throw new ArgumentException("Unsupported protocol!");
        }


        protected static MessagingClient FromConnectionString(string connStr)
        {
            // See in constructor what we need.
            throw new Exception("You must implement this method.");
        }

        public async Task<Message> Receive(TimeSpan timeout,
            ReceiveMode receiveMode = ReceiveMode.ReceiveAndDelete, long sequenceNumber = 0)
        {
            return await m_Transport.Receive(new ReceiveOptions()
            { ReceiveMode = receiveMode, TimeOut = timeout, SequenceNumber = sequenceNumber });
        }

        public async Task<Message> Receive(ReceiveMode receiveMode = ReceiveMode.ReceiveAndDelete, long sequenceNumber = 0)
        {
            return await Receive(TimeSpan.FromSeconds(30), receiveMode, sequenceNumber);
        }


        private static HttpClient getClient()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Date = new DateTimeOffset(DateTime.Now.ToUniversalTime());

            return httpClient;
        }


        public async Task Send(Message message)
        {
            await m_Transport.Send(message, new SendOptions());
        }

        public Task Complete(Uri messageLockUri)
        {
            return m_Transport.Complete(messageLockUri);
        }

        public Task Abandon(Uri messageLockUri)
        {
            return m_Transport.Abandon(messageLockUri);
        }

        public void Complete(OpenSdk.Message message)
        {
             m_Transport.Complete(message);
        }

        public void Abandon(OpenSdk.Message message)
        {
             m_Transport.Abandon(message);
        }

        protected static Dictionary<string, object> ParseConnectionString(string connectionString)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();

            //Regex rgx = new Regex("^Endpoint=sb://[a-zA-Z0-9]+.servicebus.windows.net/;SharedAccessKeyName=[a-zA-Z0-9+_-]+;SharedAccessKey=[a-zA-Z0-9+/]+=;EntityPath=[a-zA-Z0-9+/]+;TransportType=[a-zA-Z0-9]+$");
            Regex rgx = new Regex("^Endpoint=sb://[a-zA-Z0-9]+.servicebus.+[a-zA-Z0-9]+.[a-zA-Z0-9]{2,5}/;SharedAccessKeyName=[a-zA-Z0-9+_-]+;SharedAccessKey=[a-zA-Z0-9+/]+=;EntityPath=[a-zA-Z0-9+/]+;TransportType=[a-zA-Z0-9]+$");

            //"Endpoint=sb://iotlab2.servicebus.windows.net/;SharedAccessKeyName=can_read_send;SharedAccessKey=N0tu6S65oNIWELenHQKvrHHlKzscgfXom2Ke1bOV244=;Entity=samplequeue;TransportType=http";
            if (rgx.IsMatch(connectionString))
            {
                string[] words = connectionString.Split(';');
                foreach (string word in words)
                {
                    string substring1 = "Endpoint=sb://";
                    string substring2 = "SharedAccessKeyName=";
                    string substring3 = "SharedAccessKey=";
                    string substring4 = ".servicebus.windows.net/";
                    string substring5 = "EntityPath=";
                    string substring6 = "TransportType=";

                    if (word.IndexOf(substring1) == 0)
                    {
                        args.Add(SBNAMESPACE, word.Substring(substring1.Length, word.Length - substring1.Length - substring4.Length));
                    }
                    else if (word.IndexOf(substring2) == 0)
                    {
                        args.Add(KEY_NAME, word.Substring(substring2.Length, word.Length - substring2.Length));
                    }
                    else if (word.IndexOf(substring3) == 0)
                    {
                        args.Add(KEY_VALUE, word.Substring(substring3.Length, word.Length - substring3.Length));
                    }
                    else if (word.IndexOf(substring5) == 0)
                    {
                        args.Add(ENTITY, word.Substring(substring5.Length, word.Length - substring5.Length));
                    }
                    else if (word.IndexOf(substring6) == 0)
                    {
                        args.Add(PROTOCOL, word.Substring(substring6.Length, word.Length - substring6.Length));
                    }

                }

                args.Add(TOKEN_PROVIDER, new SASTokenProvider(args[KEY_NAME].ToString(), args[KEY_VALUE].ToString()));
                return args;
            }
            else
            {

                throw new ArgumentException("Invalid connection string");
                
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