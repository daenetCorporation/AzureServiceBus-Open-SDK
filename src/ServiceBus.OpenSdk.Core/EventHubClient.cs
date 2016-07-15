using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.OpenSdk
{
    public class EventHubClient : MessagingClient
    {
        public EventHubClient(string sbnamespace, string entityPath,
           TokenProvider tokenProvider, string protocol) :
            base(sbnamespace, entityPath, tokenProvider, protocol)
        {
            //this.BaseAddress = new Uri(this.BaseAddress.AbsoluteUri + entityPath);
        }

        public static EventHubClient FromConnectionString(string connStr)
        {
            connStr = connStr + ";TransportType=" + "http";
            var dict = ParseConnectionString(connStr);
            return new EventHubClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }
        public static EventHubClient FromConnectionString(string connStr, string name)
        {
            connStr = connStr + ";EntityPath=" + name + ";TransportType=" + "http";
            var dict = ParseConnectionString(connStr);
            return new EventHubClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }
        public static EventHubClient FromConnectionString(string connStr, string name, string protocol)
        {
            connStr = connStr + ";EntityPath=" + name + ";TransportType=" + protocol;
            var dict = ParseConnectionString(connStr);
            return new EventHubClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }
    }
}
