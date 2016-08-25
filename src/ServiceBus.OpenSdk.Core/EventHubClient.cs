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
    public class EventHubClient : MessagingClient
    {
        public EventHubClient(string sbnamespace, string entityPath,
           TokenProvider tokenProvider, string protocol) :
            base(sbnamespace, entityPath, tokenProvider, protocol)
        {                                                                          
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
