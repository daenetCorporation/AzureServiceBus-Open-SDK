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
