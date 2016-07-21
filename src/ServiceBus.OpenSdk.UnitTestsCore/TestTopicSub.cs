//using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ServiceBus.OpenSdk;
using ServiceBus.OpenSdk.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBus.OpenSdk.UnitTestsCore
{ 
    public class TestTopicSub
    {
        //TODO.. Ata
        //Test1 - Send to Topic over HTTP Int32
        //Test2 - Send to Topic over HTTP Int64
        //Test3 - Send to Topic over HTTP Double
        //Test4 - Send to Topic over HTTP Float
        //Test5 - Send to Topic over AMQP Int32   
        //Test6 - Send to Topic over AMQP Int64
        //Test7 - Send to Topic over AMQP Double
        //Test8 - Send to Topic over AMQP Float
        //Test9 - Send to Topic over AMQP ClassObj as Message Body
        //Test10 - Send to Topic over HTTP ClassObj as Message Body
        //Test11 - Receive From Subscription over AMQP Abandon
        //Test12 - Receive From Subscription over AMQP Complete
        //Test13 - Receive From Subscription over HTTP Abandon
        //Test14 - Receive From Subscription over HTTP Complete
        //Test15 - Create one topic client, send sample message to this topic. Then create three Subscription clients, and receive same message on all three
        Settings settings;
        public TestTopicSub()
        {

            settings = new Settings()
            {
                EndPoint = "Endpoint=sb://iotlabcore.servicebus.windows.net/;SharedAccessKeyName=iotLabCore;SharedAccessKey=QbH/yR4rUHa80Lmiz7oCVl0bBIEclfMBZ/luFkQv0sA="
            };
        }
        /// <summary>
        /// Returns the topic client for service bus
        /// </summary>
        /// <param name="topicName">Relative path of topic in service bus</param>
        /// <param name="protocol">>Protocol over which the message should be sent to topic - AMQP or HTTP</param>
        /// <returns>Topic client</returns>
        private TopicClient getTopicClient(string topicName, string protocol)
        {
            return TopicClient.FromConnectionString(settings.EndPoint, topicName, protocol);
        }

        private SubscriptionClient getSubscriptionClient(string topicName, string subName, string protocol)
        {
            return SubscriptionClient.FromConnectionString(settings.EndPoint, topicName, subName, protocol);
        }

        /// <summary>
        /// Send message to Topic via http protocol
        /// It sends an integer property asserts whether the received value is integer
        /// </summary>
        [Fact]
        public void SendToTopicUsingHttp_Int32()
        {
            TopicClient topicClient = getTopicClient("iottopic", "http");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(Int32.Parse((string)rcvMsg.Properties[key]) == value);
        }
        /// <summary>
        /// Send message to Topic via http protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// </summary>
        [Fact]
        public void SendToTopicUsingHttp_Int64()
        {
            TopicClient topicClient = getTopicClient("iottopic", "http");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            Int64 value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            
            Assert.True(Int64.Parse((String)rcvMsg.Properties[key]) == value);
        }
        /// <summary>
        /// Send message to Topic via http protocol
        /// It sends an double property asserts whether the received value is double
        /// [Known Issue]HTTP protocol returns message property value in String regardless of original type of property value. Status - FAIL
        /// </summary>
        [Fact]
        public void SendToTopicUsingHttp_Double()
        {
            TopicClient topicClient = getTopicClient("iottopic", "http");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            double value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "double");
            Assert.True((double)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        /// Send message to Topic via http protocol
        /// It sends an float property asserts whether the received value is float
        /// </summary>
        [Fact]
        public void SendToTopicUsingHttp_Float()
        {
            TopicClient topicClient = getTopicClient("iottopic", "http");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            float value = 12345f;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(float.Parse((string)rcvMsg.Properties[key]) == value);
        }

        /// <summary>
        /// Send message to Topic via amqp protocol
        /// It sends an integer property asserts whether the received value is integer
        /// </summary>
        [Fact]
        public void SendToTopicUsingAmqp_Int32()
        {
            TopicClient topicClient = getTopicClient("iottopic", "amqp");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Int32");
            Assert.True((Int32)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        /// Send message to Topic via amqp protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// </summary>
        [Fact]
        public void SendToTopicUsingAmqp_Int64()
        {
            TopicClient topicClient = getTopicClient("iottopic", "amqp");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            Int64 value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Int64");
            Assert.True((Int64)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        /// Send message to Topic via amqp protocol
        /// It sends an double property asserts whether the received value is double
        /// </summary>
        [Fact]
        public void SendToTopicUsingAmqp_Double()
        {
            TopicClient topicClient = getTopicClient("iottopic", "amqp");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            double value = -12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Double");
            Assert.True((double)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        /// Send message to Topic via amqp protocol
        /// It sends an float property asserts whether the received value is float
        /// </summary>
        [Fact]
        public void SendToTopicUsingAmqp_Float()
        {
            TopicClient topicClient = getTopicClient("iottopic", "amqp");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            float value = 12345.89f;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Single");
            Assert.True((float)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        ///  Send message to topic via amqp protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// Send custom class and asserts if custom class type has same value as it has been sent
        /// </summary>
        [Fact]
        public void SendToTopicUsingAmqp_ClsObj()
        {
            TopicClient topicClient = getTopicClient("iottopic", "amqp");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            Int64 value = 12345;
            TestClass cls = new TestClass();
            Message msg = new Message(cls)
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);
            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True((Int64)rcvMsg.Properties[key] == value);
            Assert.True(rcvMsg.GetBody<TestClass>() != null);
        }
        /// <summary>
        ///  Send message to topic via http protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// Send custom class and asserts if custom class has same type as it has been sent
        /// </summary>
        [Fact]
        public void SendToTopicUsingHttp_ClsObj()
        {
            TopicClient topicClient = getTopicClient("iottopic", "http");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            Int64 value = 12345;
            TestClass cls = new TestClass();
            Message msg = new Message(cls)
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg);
            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True((Int64)rcvMsg.Properties[key] == value);
            Assert.True(rcvMsg.GetBody<TestClass>() != null);
        }

        /// <summary>
        /// Send messsage to topic via amqp protocol
        /// Abandon the received message from Subscription
        /// It sends an integer64 property and message Id
        /// Assert if the sent message has been received and if message id has the same value as it has been sent 
        /// </summary>
        [Fact]
        public void RcvAbandonFromSubscriptionUsingAmqp()
        {
            TopicClient topicClient = getTopicClient("iottopic", "amqp");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            String key = "test";
            Int64 value = 12345;
            String id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id

            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);

            subscriptionClient.Abandon(rcvMsg);
            rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
        }

        /// <summary>
        /// Send messsage to topic via amqp protocol
        /// Receive all the  messages from subscription and complete them
        /// It sends an integer64 property
        /// Assert if the sent message has been received 
        /// </summary>
        [Fact]
        public void RcvCompleteFromSubscriptionUsingAmqp()
        {

            TopicClient topicClient = getTopicClient("iottopic", "amqp");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            Int64 value = 12345;
            String key = "test";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };
            while (true)
            {
                var rcvMessage = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;
                if (rcvMessage == null)
                    break;
            }
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            subscriptionClient.Complete(rcvMsg);
            rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg == null);
        }

        /// <summary>
        /// Send messsage to topic via http protocol
        /// Receive and complete the message from subscription 
        /// It sends an integer64 property
        /// Assert if the same message is received
        /// </summary>
        [Fact]
        public void RcvAbandonFromSubscriptionUsingHttp()
        {
            TopicClient topicClient = getTopicClient("iottopic", "http");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            String key = "test";
            Int64 value = 12345;
            String id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id

            };
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);

            subscriptionClient.Abandon(rcvMsg);
            rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
        }

        /// <summary>
        /// Send messsage to topic via http protocol
        /// Receive all the  messages from subscription and complete them
        /// It sends an integer64 property
        /// Assert if the sent message has been received 
        /// </summary>
        [Fact]
        public void RcvCompleteFromSubscriptionUsingHttp()
        {

            TopicClient topicClient = getTopicClient("iottopic", "http");
            SubscriptionClient subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            Int64 value = 12345;
            String key = "test";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };
            while (true)
            {
                var rcvMessage = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;
                if (rcvMessage == null)
                    break;
            }
            topicClient.Send(msg);

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            subscriptionClient.Complete(rcvMsg);
            rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg == null);
        }

        /// <summary>
        /// Message send to topic via amqp protocol
        /// Receive messages with three subscriptions clients
        /// Assert if all the three subscriptions clients have received the same message as it has been sent
        /// </summary>
        [Fact]
        public void sendToTopicUsingAmqp_RecvWithThreeSubClients()
        {
            TopicClient topicClient = getTopicClient("iottopic", "amqp");
            SubscriptionClient subscriptionClient1 = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            SubscriptionClient subscriptionClient2 = getSubscriptionClient("iottopic", "iotsubscription2", "amqp");
            SubscriptionClient subscriptionClient3 = getSubscriptionClient("iottopic", "iotsubscription3", "amqp");

            string key = "test";
            int value = 12345;
            string id = "sampleMessage";
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } },
                MessageId = id
            };
            topicClient.Send(msg);

            var rcvMsg1 = subscriptionClient1.Receive(ReceiveMode.ReceiveAndDelete).Result;
            var rcvMsg2 = subscriptionClient2.Receive(ReceiveMode.ReceiveAndDelete).Result;
            var rcvMsg3 = subscriptionClient3.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.False(rcvMsg1 == null);
            Assert.True(rcvMsg1.Properties != null);
            Assert.True(rcvMsg1.MessageId == id);

            Assert.True(rcvMsg2 != null);
            Assert.True(rcvMsg2.Properties != null);
            Assert.True(rcvMsg2.MessageId == id);

            Assert.True(rcvMsg3 != null);
            Assert.True(rcvMsg3.Properties != null);
            Assert.True(rcvMsg3.MessageId == id);
        }
    }
}

