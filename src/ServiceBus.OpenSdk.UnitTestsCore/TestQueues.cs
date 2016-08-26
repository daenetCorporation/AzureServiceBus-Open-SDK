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
using System.IO;
using System.Runtime.Serialization;
using Xunit;

namespace ServiceBus.OpenSdk.UnitTestsCore
{

    public class TestQueues
    {
        private QueueClient m_client;
        private Settings m_settings;
        private DataContractSerializer m_dataConSer;

        public TestQueues()
        {
            
        }

        /// <summary>
        /// Returns the queue client for service bus
        /// </summary>
        /// <param name="path">Relative path of queue in service bus</param>
        /// <param name="protocol">Protocol over which the message should be sent to queue - AMQP or HTTP</param>
        /// <returns>Queue Client</returns>
        private QueueClient getQueueClient(string path, string protocol)
        {
            return QueueClient.FromConnectionString(Settings.EndPoint, path, protocol);
        }


        /// <summary>
        /// Send message to Queue via amqp protocol
        /// It sends an integer property asserts whether the received value is integer
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_Int32()
        {
            m_client = getQueueClient(Settings.Queue0, "amqp");
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            m_client.Send(msg).Wait();
 
            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Int32");
            Assert.True((int)rcvMsg.Properties[key] == value);
           
        }

        /// <summary>
        ///  Send message to Queue via amqp protocol
        /// It sends an double property asserts whether the received value is double
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_Double()
        {
            m_client = getQueueClient(Settings.Queue1, "amqp");
            string key = "test";
            double value = 12345.0;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value }, { "blah", "12345" } },
                MessageId = "8121345hk"
            };
            m_client.Send(msg).Wait();
            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Double");
            Assert.True((double)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        ///  Send message to Queue via amqp protocol
        /// It sends an float property asserts whether the received value is float
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_Float()
        {
            m_client = getQueueClient(Settings.Queue2, "amqp");
            string key = "test";
            float value = 12345.89f;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;


            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Single");
            Assert.True((float)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        ///  Send message to Queue via http protocol
        /// It sends an integer property asserts whether the received value is integer
        /// </summary>

        [Fact]
        public void SendToQueueUsingHttp_Int32()
        {
            m_client = getQueueClient(Settings.Queue3, "http");
            string key = "test";
            int value = 12345;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            //Known Issue - Http returns every property value as string 
            Assert.True(int.Parse((String)rcvMsg.Properties[key]) == value);
        }

        /// <summary>
        ///  Send message to Queue via amqp protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_Int64()
        {
            m_client = getQueueClient(Settings.Queue4, "amqp");
            string key = "test";
            // Int64 value = 123456789;
            long value = 4294967296L;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True((long)rcvMsg.Properties[key] == value);
            Assert.True(rcvMsg.GetBody<string>() == "Sample message");
        }

        /// <summary>
        ///  Send message to Queue via http protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// </summary>
        [Fact]
        public void SendToQueueUsingHttp_Int64()
        {
            m_client = getQueueClient(Settings.Queue5, "http");
            string key = "test";
            long value = 1111133111111112345L;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(long.Parse((String)rcvMsg.Properties[key]) == value);
            Assert.True(rcvMsg.GetBody<string>() == "Sample message");
        }

        /// <summary>
        ///  Send message to Queue via amqp protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// Send custom class and asserts if custom class type has same value as it has been sent
        /// [Known Issue] - Cannot send custom body through AMQP - Status - FAIL
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_ClsObj()
        {
            m_client = getQueueClient(Settings.Queue6, "amqp");
            string key = "test";
            Int64 value = 12345L;
            TestClass cls = new TestClass("",2);
            Message msg = new Message("abc")
            {
                Properties = { { key, value } }
            };

            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            // TODO:
            // AMQP lite cannot do such kind of serialization tight now.
            // We will provide in the future a solution for this issue.
            // This is why we here ignore these two exceptions. See http?git..
            var ex = Assert.Throws<InvalidCastException>(() => Int64.Parse((String)rcvMsg.Properties[key]) == value);

            ex = Assert.Throws<InvalidCastException>(() => rcvMsg.GetBody<TestClass>());
        }

        /// <summary>
        ///  Send message to Queue via http protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// Send custom class and asserts if custom class has same type as it has been sent
        /// [Known Issue] - Cannot receive custom body through HTTP - Status - FAIL
        /// </summary>
        [Fact]
        public void SendToQueueUsingHttp_ClsObj()
        {
            m_client = getQueueClient(Settings.Queue7, "http");
            String key = "test";
            Int64 value = 12345L;
            Message msg = new Message(new TestClass("",6))
            {
                Properties = { { key, value } }
            };
            m_client.Send(msg).Wait();
            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True(Int64.Parse((String)rcvMsg.Properties[key]) == value);
            Assert.True(rcvMsg.GetBody<TestClass>() != null);
        }

        /// <summary>
        /// A loop of messages will send to Queue via amqp protocol
        /// complete the received message loop from Queue
        /// It sends an integer64 property 
        /// Assert if the sent message has been received 
        /// </summary>
        [Fact]
        public void RcvCompleteFromQueueUsingAmqp()
        {
            m_client = getQueueClient(Settings.Queue8, "amqp");
            Int64 value = 12345L;
            String key = "test";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };
            
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            m_client.Complete(rcvMsg);
            rcvMsg = m_client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg == null);
        }

        /// <summary>
        /// Send message to Queue via amqp protocol
        /// Abandon the received message from Queue
        /// It sends an integer64 property and message Id
        /// Assert if the sent message has been received and if message id has the same value as it has been sent 
        /// </summary>
        [Fact]
        public void RcvAbandonFromQueueUsingAmqp()
        {
            m_client = getQueueClient(Settings.Queue9, "amqp");
            String key = "test";
            Int64 value = 12345L;
            String id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id

            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);

            m_client.Abandon(rcvMsg);
            rcvMsg = m_client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
            m_client.Complete(rcvMsg);
            
        }

        /// <summary>
        /// Send message to Queue via http protocol
        /// Receive all the  messages from Queue and complete them
        /// It sends an integer64 property
        /// Assert if the sent message has been received 
        /// </summary>
        [Fact]
        public void RcvCompleteFromQueueUsingHttp()
        {
            m_client = getQueueClient(Settings.Queue10.ToString(), "http");
            String key = "test";
            Int64 value = 12345L;
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            m_client.Complete(rcvMsg);
            rcvMsg = m_client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg == null);
        }

        /// <summary>
        /// Send message to Queue via http protocol
        /// Receive all the  messages from Queue and complete them
        /// It sends an integer64 property
        /// Assert if the sent message has been received and if message id has the same value as it has been sent  
        /// </summary>
        [Fact]
        public void RcvAbandonFromQueueUsingHttp()
        {
            m_client = getQueueClient(Settings.Queue11, "http");
            Int64 value = 12345L;
            String key = "value";
            string id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id
            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);

            m_client.Abandon(rcvMsg);
            rcvMsg = m_client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
            m_client.Complete(rcvMsg);
        }

        /// <summary>
        /// Send message id to Queue via http protocol
        /// It sends an integer property
        /// Assert if the same message is received
        /// </summary>
        [Fact]
        public void SendToQueueUsingHttp_MessageId()
        {
            m_client = getQueueClient(Settings.Queue12, "http");
            string key = "test";
            int value = 12345;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } },
                MessageId = "8121345testing"
            };
            m_client.Send(msg).Wait();

            var message = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(message != null);
            Assert.True(message.Properties != null);
            Assert.True(message.Properties.ContainsKey(key));
            Assert.True(int.Parse((String)message.Properties[key]) == value);
        }

        /// <summary>
        /// Send message to Queue via http protocol
        /// It sends message body and asserts if the body has same value as it has been sent
        /// </summary>
        [Fact]
        public void SendToQueueUsingHttp_ReadBody()
        {
            m_client = getQueueClient(Settings.Queue13, "http");
            string key = "test";
            int value = 12345;
            string body = "Sample Message";
            Message msg = new Message(body)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            m_client.Send(msg).Wait();                                                

            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(strm != null);
            string rcvdBody = rcvMsg.GetBody<string>();
            Assert.True(rcvdBody == body);
            //Known Issue - Http returns every property value as string but it passed
            Assert.True(int.Parse((String)rcvMsg.Properties[key]) == value);
        }

        /// <summary>
        /// Send message to Queue via amqp protocol
        ///  It sends message body and asserts if the body has same value as it has been sent
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_ReadBody()
        {
            m_client = getQueueClient(Settings.Queue14, "amqp");
            string key = "test";
            int value = 12345;
            string body = "Sample Message";
            Message msg = new Message(body)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(strm != null);
            string rcvdBody = rcvMsg.GetBody<string>();
            Assert.True(rcvdBody == body);

        }

        /// <summary>
        /// Send message to Queue via http protocol
        ///  It sends message custom body and asserts if the custom body has same type as it has been sent
        ///  [Known Issue] - Cannot receive custom body through HTTP - Status - FAIL
        /// </summary
        [Fact]
        public void SendToQueueUsingHttp_ReadCustomBody()
        {     
            m_client = getQueueClient(Settings.Queue15, "http");
            m_dataConSer = new DataContractSerializer(typeof(TestClass));
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass("ServiceBussSdk",123456);
            Message msg = new Message(tClass, m_dataConSer)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            m_client.Send(msg).Wait(); 
            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result; 
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(int.Parse(rcvMsg.Properties[key].ToString()) == value);
            Assert.True(rcvMsg.GetBody<TestClass>() != null);
        }

        /// <summary>
        /// Send message to Queue via amqp protocol
        ///  It sends message custom body and asserts if the custom body has same type as it has been sent
        ///  [Known Issue] - Cannot send custom body through AMQP - Status - FAIL
        /// </summary
        [Fact]
        public void SendToQueueUsingAmqp_ReadCustomBody()
        {
            m_client = getQueueClient(Settings.Queue16, "amqp");
            m_dataConSer = new DataContractSerializer(typeof(TestClass));
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass("ServiceBussSdk", 123456);
            Message msg = new Message(tClass,m_dataConSer)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            m_client.Send(msg).Wait();

            var rcvMsg = m_client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(strm != null);

            TestClass rcvdClass = rcvMsg.GetBody<TestClass>();
            Assert.True(rcvdClass == tClass);

        }
    }

}



