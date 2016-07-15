using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.OpenSdk
{
    public class TopicClient : MessagingClient
    {
        public TopicClient(string sbnamespace, string entityPath,
            TokenProvider tokenProvider, string protocol) :
            base(sbnamespace, entityPath, tokenProvider, protocol)
        {

        }

        public static TopicClient FromConnectionString(string connStr)
        {
            if (!connStr.Contains("TransportType"))
                connStr = connStr + ";TransportType=http"; 
            var dict = ParseConnectionString(connStr);
            return new TopicClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }

        public static TopicClient FromConnectionString(string connStr, string path)
        {

            connStr = connStr + ";EntityPath=" + path;
            return FromConnectionString(connStr);
        }

        public static TopicClient FromConnectionString(string connStr, string path, string protocol)
        {
            connStr = connStr + ";EntityPath=" + path + ";TransportType=" + protocol;
            var dict = ParseConnectionString(connStr);
            return new TopicClient(dict[SBNAMESPACE].ToString(), dict[ENTITY].ToString(), (TokenProvider)dict[TOKEN_PROVIDER], dict[PROTOCOL].ToString());
        }
    }
}
