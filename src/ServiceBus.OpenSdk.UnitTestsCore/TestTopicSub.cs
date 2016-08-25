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
using System;
using Xunit;

namespace ServiceBus.OpenSdk.UnitTestsCore
{
    public class TestTopicSub
    {
        private TopicClient topicClient;
        private SubscriptionClient subscriptionClient;
        private SubscriptionClient subscriptionClient1;
        private SubscriptionClient subscriptionClient2;
        Settings settings;
        public TestTopicSub()
        {
        //    SubscriptionClient client = getSubscriptionClient("iottopic", "iotsubscription", "http");
        //    while (true)
        //    {
        //        var rcvMessage = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
        //        if (rcvMessage == null)
        //            break;
        //    }
        }
        /// <summary>
        /// Returns the topic client for service bus
        /// </summary>
        /// <param name="topicName">Relative path of topic in service bus</param>
        /// <param name="protocol">>Protocol over which the message should be sent to topic - AMQP or HTTP</param>
        /// <returns>Topic client</returns>
        private TopicClient getTopicClient(string topicName, string protocol)
        {
            return TopicClient.FromConnectionString(Settings.EndPoint, topicName, protocol);
        }

        private SubscriptionClient getSubscriptionClient(string topicName, string subName, string protocol)
        {
            return SubscriptionClient.FromConnectionString(Settings.EndPoint, topicName, subName, protocol);
        }

        /// <summary>
        /// Send message to Topic via http protocol
        /// It sends an integer property asserts whether the received value is integer
        /// </summary>
        [Fact]
        public void SendToTopicUsingHttp_Int32()
        {
            topicClient = getTopicClient("iottopic", "http");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();

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
            topicClient = getTopicClient("iottopic", "http");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            //Int64 value = 12345;
            long value = 1111133111111112345L;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True(long.Parse((String)rcvMsg.Properties[key]) == value);
        }
        /// <summary>
        /// Send message to Topic via http protocol
        /// It sends an double property asserts whether the received value is double
        /// [Known Issue]HTTP protocol returns message property value in String regardless of original type of property value. Status - FAIL
        /// </summary>
        [Fact]
        public void SendToTopicUsingHttp_Double()
        {
            topicClient = getTopicClient("iottopic", "http");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            double value = 12345.0;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            //HTTP protocol returns message property value in String regardless of original type of property value
            var valueType = rcvMsg.Properties[key].GetType().Name;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "double");
            Assert.True(double.Parse(rcvMsg.Properties[key].ToString()) == value);
        }

        /// <summary>
        /// Send message to Topic via http protocol
        /// It sends an float property asserts whether the received value is float
        /// </summary>
        [Fact]
        public void SendToTopicUsingHttp_Float()
        {
            topicClient = getTopicClient("iottopic", "http");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            float value = 12345f;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();

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
            topicClient = getTopicClient("iottopic", "amqp");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();

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
            topicClient = getTopicClient("iottopic", "amqp");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            //Int64 value = 12345;
            long value = 1111133111111112345L;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Int64");
            Assert.True((long)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        /// Send message to Topic via amqp protocol
        /// It sends an double property asserts whether the received value is double
        /// </summary>
        [Fact]
        public void SendToTopicUsingAmqp_Double()
        {
            topicClient = getTopicClient("iottopic", "amqp");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            double value = 12345.0;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();

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
            topicClient = getTopicClient("iottopic", "amqp");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            float value = 12345.89f;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();

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
            topicClient = getTopicClient("iottopic", "amqp");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            string key = "test";
            //Int64 value = 12345;
            long value = 1111133111111112345L;
            TestClass cls = new TestClass();
            Message msg = new Message(cls)
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();
            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True((long)rcvMsg.Properties[key] == value);
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
            topicClient = getTopicClient("iottopic", "http");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            string key = "test";
            // Int64 value = 12345;
            long value = 1111133111111112345L;
            TestClass cls = new TestClass();
            Message msg = new Message(cls)
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();
            var rcvMsg = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True((long)rcvMsg.Properties[key] == value);
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
            topicClient = getTopicClient("iottopic", "amqp");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            String key = "test";
            int value = 12345;
            String id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id

            };
            topicClient.Send(msg).Wait();

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);

            subscriptionClient.Abandon(rcvMsg);
            var rcvMsg2 = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg2 != null);
            Assert.True(rcvMsg2.MessageId == rcvMsg.MessageId);
            subscriptionClient.Complete(rcvMsg2);
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

            topicClient = getTopicClient("iottopic", "amqp");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            int value = 12345;
            String key = "test";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };
            //while (true)
            //{
            //    var rcvMessage = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            //    if (rcvMessage == null)
            //        break;
            //}
            topicClient.Send(msg).Wait();

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            subscriptionClient.Complete(rcvMsg);
            var rcvMsg2 = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg2 == null);
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
            topicClient = getTopicClient("iottopic", "http");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            String key = "test";
            int value = 12345;
            String id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id

            };
            topicClient.Send(msg).Wait();

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

            topicClient = getTopicClient("iottopic", "http");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "http");
            //Int64 value = 12345;
            long value = 1111133111111112345L;
            String key = "test";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };
           
            topicClient.Send(msg).Wait();

            var rcvMsg = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            subscriptionClient.Complete(rcvMsg);
           var rcvMsg1 = subscriptionClient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg1 == null);
        }

        /// <summary>
        /// Message send to topic via amqp protocol
        /// Receive messages with three subscriptions clients
        /// Assert if all the three subscriptions clients have received the same message as it has been sent
        /// </summary>
        [Fact]
        public void sendToTopicUsingAmqp_RecvWithThreeSubClients()
        {
            topicClient = getTopicClient("iottopic", "amqp");
            subscriptionClient = getSubscriptionClient("iottopic", "iotsubscription", "amqp");
            subscriptionClient1 = getSubscriptionClient("iottopic", "iotsubscription2", "amqp");
            subscriptionClient2 = getSubscriptionClient("iottopic", "iotsubscription3", "amqp");

            string key = "test";
            int value = 12345;
            string id = "sampleMessage";
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } },
                MessageId = id
            };
            topicClient.Send(msg).Wait();

            var rcvMsg1 = subscriptionClient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            var rcvMsg2 = subscriptionClient1.Receive(ReceiveMode.ReceiveAndDelete).Result;
            var rcvMsg3 = subscriptionClient2.Receive(ReceiveMode.ReceiveAndDelete).Result;

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

