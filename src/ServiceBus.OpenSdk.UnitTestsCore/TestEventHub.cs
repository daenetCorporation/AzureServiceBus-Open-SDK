using Xunit;

namespace ServiceBus.OpenSdk.UnitTestsCore
{
    public class TestEventHub
    {
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
            EventHubClient eventHubClient = getEventHubClient("iotEventHub", "http");
            
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
            EventHubClient eventHubClient = getEventHubClient("iotEventHub", "amqp");
           
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
