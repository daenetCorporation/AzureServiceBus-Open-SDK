using ServiceBus.OpenSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBus.OpenSdk.UnitTestsCore
{ 
    public class TestEventHub
    {
        //TODO..
        //Test1 - Send message to EventHub using HTTP
        //Test2 - Send message to EventHub using AMQP

        Settings settings;
        public TestEventHub()
        {

            settings = new Settings()
            {
                EndPoint = "Endpoint=sb://iotlabcore.servicebus.windows.net/;SharedAccessKeyName=iotLabCore;SharedAccessKey=QbH/yR4rUHa80Lmiz7oCVl0bBIEclfMBZ/luFkQv0sA="
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHubName">Relative path of eventhub in service bus</param>
        /// <param name="protocol">Protocol over which the message should be sent to eventhub - AMQP or HTTP</param>
        /// <returns></returns>
        private EventHubClient getEventHubClient(string eventHubName, string protocol)
        {
            return EventHubClient.FromConnectionString(settings.EndPoint, protocol);
        }
        /// <summary>
        /// Send message to evethub via http protocol 
        /// </summary>
        [Fact]
        public void SendToEventHubUsingHttp()
        {
            EventHubClient eventHubClient = getEventHubClient("iotEventHub", "http");
            
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };

            eventHubClient.Send(msg);
        }

        /// <summary>
        /// Send message to evethub via amqp protocol 
        /// </summary>
        [Fact]
        public void SendToEventHubUsingAmqp()
        {
            EventHubClient eventHubClient = getEventHubClient("iotEventHub", "amqp");
           
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };

            eventHubClient.Send(msg);
        }

    }
}
