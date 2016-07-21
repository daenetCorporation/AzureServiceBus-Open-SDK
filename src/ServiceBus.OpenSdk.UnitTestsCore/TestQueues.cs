using System;
//using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ServiceBus.OpenSdk;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ServiceBus.OpenSdk.UnitTestsCore
{
    
    public class TestQueues
    {

        Settings settings;
        public TestQueues()
        {
            settings = new Settings()
            {
                EndPoint = ""

            };
        }

        /// <summary>
        /// Returns the queue client for service bus
        /// </summary>
        /// <param name="path">Relative path of queue in service bus</param>
        /// <param name="protocol">Protocol over which the message should be sent to queue - AMQP or HTTP</param>
        /// <returns>Queue Client</returns>
        private QueueClient getQueueClient(string path, string protocol)
        {
            return QueueClient.FromConnectionString(settings.EndPoint, path, protocol);
        }


        /// <summary>
        /// Send message to Queue via amqp protocol
        /// It sends an integer property asserts whether the received value is integer
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_Int32()
        {
            QueueClient client = getQueueClient("iotqueue", "amqp");
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(rcvMsg.Properties[key].GetType().Name == "Int32");
            Assert.True((Int32)rcvMsg.Properties[key] == value);
        }

        /// <summary>
        ///  Send message to Queue via amqp protocol
        /// It sends an double property asserts whether the received value is double
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_Double()
        {
            QueueClient client = getQueueClient("iotqueue", "amqp");
            string key = "test";
            double value = 12345;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value }, { "blah", "12345" } },
                MessageId = "8121345hk"
            };
            client.Send(msg);
            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            
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
            QueueClient client = getQueueClient("iotqueue", "amqp");
            string key = "test";
            float value = 12345.89f;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;


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
            QueueClient client = getQueueClient("iotqueue", "http");
            string key = "test";
            int value = 12345;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            //Known Issue - Http returns every property value as string 
            Assert.True(Int32.Parse((String)rcvMsg.Properties[key]) == value);
        }

        /// <summary>
        ///  Send message to Queue via amqp protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_Int64()
        {
            QueueClient client = getQueueClient("iotqueue", "amqp");
            string key = "test";
            Int64 value = 12345;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True((Int64)rcvMsg.Properties[key] == value);
            Assert.True(rcvMsg.GetBody<string>() == "Sample message");
        }

        /// <summary>
        ///  Send message to Queue via http protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// </summary>
        [Fact]
        public void SendToQueueUsingHttp_Int64()
        {
            QueueClient client = getQueueClient("iotqueue", "http");
            string key = "test";
            Int64 value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True(Int64.Parse((String)rcvMsg.Properties[key]) == value);
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
            QueueClient client = getQueueClient("iotqueue", "amqp");
            string key = "test";
            Int64 value = 12345;
            TestClass cls = new TestClass();
            Message msg = new Message(cls)
            {
                Properties = { { key, value } }
            };
            client.Send(msg);
            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            Assert.True(Int64.Parse((String)rcvMsg.Properties[key]) == value);
            Assert.True(rcvMsg.GetBody<TestClass>() != null);
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
            QueueClient client = getQueueClient("iotqueue", "http");
            String key = "test";
            Int64 value = 12345;
            Message msg = new Message(new TestClass())
            {
                Properties = { { key, value } }
            };
            client.Send(msg);
            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
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
            QueueClient client = getQueueClient("iotqueue", "amqp");
            Int64 value = 12345;
            String key = "test";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };
            while (true)
            {
                var rcvMessage = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
                if (rcvMessage == null)
                    break;
            }
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            client.Complete(rcvMsg);
            rcvMsg = client.Receive(ReceiveMode.PeekLock).Result;
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
            QueueClient client = getQueueClient("iotqueue", "amqp");
            String key = "test";
            Int64 value = 12345;
            String id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id

            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);

            client.Abandon(rcvMsg);
            rcvMsg = client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
        }
         [Fact]
         public void RcvAllMsgs()
        {
            QueueClient client = getQueueClient("iotqueue", "http");
            while (true)
            {
                var rcvMessage = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
                if (rcvMessage == null)
                    break;
            }
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
            QueueClient client = getQueueClient("iotqueue", "http");
            String key = "test";
            Int64 value = 12345;
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };

            while (true)
            {
                var rcvMessage = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
                if (rcvMessage == null)
                    break;
            }
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            client.Complete(rcvMsg);
            rcvMsg = client.Receive(ReceiveMode.PeekLock).Result;
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
            QueueClient client = getQueueClient("iotqueue", "http");
            Int64 value = 12345;
            String key = "value";
            string id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);

            client.Abandon(rcvMsg);
            rcvMsg = client.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
        }

        /// <summary>
        /// Send message id to Queue via http protocol
        /// It sends an integer property
        /// Assert if the same message is received
        /// </summary>
        [Fact]
        public void SendToQueueUsingHttp_MessageId()
        {
            QueueClient client = getQueueClient("iotqueue", "http");
            string key = "test";
            Int32 value = 12345;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } },
                MessageId = "8121345testing"
            };
            client.Send(msg);

            var MessageId = client.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(MessageId != null);
            Assert.True(MessageId.Properties != null);
            Assert.True(MessageId.Properties.ContainsKey(key));
            Assert.True(Int32.Parse((String)MessageId.Properties[key]) == value);
        }

        /// <summary>
        /// Send message to Queue via http protocol
        /// It sends message body and asserts if the body has same value as it has been sent
        /// </summary>
        [Fact]
        public void SendToQueueUsingHttp_ReadBody()
        {
            QueueClient client = getQueueClient("iotqueue", "http");
            string key = "test";
            int value = 12345;
            string body = "Sample Message";
            Message msg = new Message(body)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(strm != null);
            string rcvdBody = rcvMsg.GetBody<string>();
            Assert.True(rcvdBody == body);
            //Known Issue - Http returns every property value as string 
            Assert.True(Int32.Parse((String)rcvMsg.Properties[key]) == value);
        }

        /// <summary>
        /// Send message to Queue via amqp protocol
        ///  It sends message body and asserts if the body has same value as it has been sent
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_ReadBody()
        {
            QueueClient client = getQueueClient("iotqueue", "amqp");
            string key = "test";
            int value = 12345;
            string body = "Sample Message";
            Message msg = new Message(body)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
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
            QueueClient client = getQueueClient("iotqueue", "http");
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass();
            Message msg = new Message(tClass)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(strm != null);

            TestClass rcvdClass = rcvMsg.GetBody<TestClass>();
            Assert.True(rcvdClass == tClass);
            //Known Issue - Http returns every property value as string 
            Assert.True(Int32.Parse((String)rcvMsg.Properties[key]) == value);
        }

        /// <summary>
        /// Send message to Queue via amqp protocol
        ///  It sends message custom body and asserts if the custom body has same type as it has been sent
        ///  [Known Issue] - Cannot send custom body through AMQP - Status - FAIL
        /// </summary
        [Fact]
        public void SendToQueueUsingAmqp_ReadCustomBody()
        {
            QueueClient client = getQueueClient("iotqueue", "amqp");
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass();
            Message msg = new Message(tClass)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            client.Send(msg);

            var rcvMsg = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(strm != null);

            TestClass rcvdClass = rcvMsg.GetBody<TestClass>();
            Assert.True(rcvdClass == tClass);
            
        }
    }

}



