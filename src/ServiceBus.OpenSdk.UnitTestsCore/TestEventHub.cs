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
using Xunit;

namespace ServiceBus.OpenSdk.UnitTestsCore
{
    public class TestEventHub
    {
        private EventHubClient eventHubClient;
        Settings settings;
        public TestEventHub()
        {
                 
         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHubName">Relative path of eventhub in service bus</param>
        /// <param name="protocol">Protocol over which the message should be sent to eventhub - AMQP or HTTP</param>
        /// <returns></returns>
        private EventHubClient getEventHubClient(string eventHubName, string protocol)
        {
            return EventHubClient.FromConnectionString(Settings.EndPoint,eventHubName, protocol);
        }

        /// <summary>
        /// Send message to evethub via http protocol 
        /// </summary>
        [Fact]
        public void SendToEventHubUsingHttp()
        {
             eventHubClient = getEventHubClient("iotEventHub", "http");
            
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };

            eventHubClient.Send(msg).Wait();
        }

        /// <summary>
        /// Send message to evethub via amqp protocol 
        /// </summary>
        [Fact]
        public void SendToEventHubUsingAmqp()
        {
             eventHubClient = getEventHubClient("iotEventHub", "amqp");
           
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };

            eventHubClient.Send(msg).Wait();
        }

    }
}
