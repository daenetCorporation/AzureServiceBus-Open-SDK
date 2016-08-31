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
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace ServiceBus.OpenSdk.UnitTestsCore
{

    public class TestQueues
    { 

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
            var qclient = getQueueClient(Settings.Queue0, "amqp");
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;
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
             var qclient = getQueueClient(Settings.Queue1, "amqp");
            string key = "test";
            double value = 12345.0;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value }, { "blah", "12345" } },
                MessageId = "8121345hk"
            };
            qclient.Send(msg).Wait();
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;

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
            var qclient = getQueueClient(Settings.Queue2, "amqp");
            string key = "test";
            float value = 12345.89f;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;


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
            var qclient = getQueueClient(Settings.Queue3, "http");
            string key = "test";
            int value = 12345;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key)); 
            Assert.True(int.Parse((String)rcvMsg.Properties[key]) == value);
        }

        /// <summary>
        ///  Send message to Queue via amqp protocol
        /// It sends an integer64 property asserts whether the received value is integer64
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_Int64()
        {
            var qclient = getQueueClient(Settings.Queue4, "amqp");
            string key = "test";     
            long value = 4294967296L;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;

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
            var qclient = getQueueClient(Settings.Queue5, "http");
            string key = "test";
            long value = 1111133111111112345L;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;
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
            var qclient = getQueueClient(Settings.Queue6, "amqp");
            string key = "test";
            Int64 value = 12345L;
            TestClass cls = new TestClass("ServiceBus", 420);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestClass));
            using (StringWriter txtWriter = new StringWriter())
            {
                xmlSerializer.Serialize(txtWriter, cls);
                var txtMgs = txtWriter.ToString();
                Message msg = new Message(txtMgs)
                {
                    Properties = { { key, value } }
                };
                qclient.Send(msg).Wait();
            }
               
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;

            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));

            var rcvTxt = rcvMsg.GetBody<string>();
            using(StringReader txtReader = new StringReader(rcvTxt))
            {
               var rcvObj =  xmlSerializer.Deserialize(txtReader) as TestClass;

                Assert.True(rcvObj.PropertyOne == cls.PropertyOne);
                Assert.True(rcvObj.PropertyTwo == cls.PropertyTwo);
            }
           
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
            var qclient = getQueueClient(Settings.Queue7, "http");
            String key = "test";
            Int64 value = 12345L;
            Message msg = new Message(new TestClass("ServiceBusSdk", 6))
            {
                Properties = { { key, value } }
            };
            qclient.Send(msg).Wait();
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;
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
            var qclient = getQueueClient(Settings.Queue8, "amqp");
            Int64 value = 12345L;
            String key = "test";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };

            qclient.Send(msg).Wait(); 
            var rcvMsg = qclient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null); 
            qclient.Complete(rcvMsg);
            rcvMsg = qclient.Receive(ReceiveMode.PeekLock).Result;
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
            var qclient = getQueueClient(Settings.Queue9, "amqp");
            String key = "test";
            Int64 value = 12345L;
            String id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id

            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
            qclient.Abandon(rcvMsg);
            rcvMsg = qclient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
            qclient.Complete(rcvMsg);

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
            var qclient = getQueueClient(Settings.Queue10.ToString(), "http");
            String key = "test";
            Int64 value = 12345L;
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } }
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);

            qclient.Complete(rcvMsg);
            rcvMsg = qclient.Receive(ReceiveMode.PeekLock).Result;
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
            var qclient = getQueueClient(Settings.Queue11, "http");
            Int64 value = 12345L;
            String key = "value";
            string id = "myId";
            Message msg = new Message("Sample Message")
            {
                Properties = { { key, value } },
                MessageId = id
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);

            qclient.Abandon(rcvMsg);
            rcvMsg = qclient.Receive(ReceiveMode.PeekLock).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.MessageId == id);
            qclient.Complete(rcvMsg);
        }

        /// <summary>
        /// Send message id to Queue via http protocol
        /// It sends an integer property
        /// Assert if the same message is received
        /// </summary>
        [Fact]
        public void SendToQueueUsingHttp_MessageId()
        {
            var qclient = getQueueClient(Settings.Queue12, "http");
            string key = "test";
            int value = 12345;

            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } },
                MessageId = "8121345testing"
            };
            qclient.Send(msg).Wait();

            var message = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;

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
            var qclient = getQueueClient(Settings.Queue13, "http");
            string key = "test";
            int value = 12345;
            string body = "Sample Message";
            Message msg = new Message(body)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Stream strm = rcvMsg.GetBodyStream();
            Assert.True(strm != null);
            string rcvdBody = rcvMsg.GetBody<string>();
            Assert.True(rcvdBody == body);
            Assert.True(int.Parse((String)rcvMsg.Properties[key]) == value);
        }

        /// <summary>
        /// Send message to Queue via amqp protocol
        ///  It sends message body and asserts if the body has same value as it has been sent
        /// </summary>
        [Fact]
        public void SendToQueueUsingAmqp_ReadBody()
        {
            var qclient = getQueueClient(Settings.Queue14, "amqp");
            string key = "test";
            int value = 12345;
            string body = "Sample Message";
            Message msg = new Message(body)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            qclient.Send(msg).Wait();

            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;
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
            var qclient = getQueueClient(Settings.Queue15, "http"); 
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass("ServiceBussSdk", 123456);
            Message msg = new Message(tClass)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            qclient.Send(msg).Wait();
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            Assert.True(rcvMsg != null);
            Assert.True(rcvMsg.Properties != null);
            Assert.True(rcvMsg.Properties.ContainsKey(key));
            Assert.True(int.Parse(rcvMsg.Properties[key].ToString()) == value);
            Assert.True(rcvMsg.GetBody<TestClass>() != null);
        }

        /// <summary>
        /// Send message to Queue via amqp protocol
        ///  It sends message custom body and asserts if the custom body has same type as it has been sent
        /// </summary
        [Fact]
        public void SendToQueueUsingAmqp_ReadCustomBodyDataContractSerializer()
        {
            var qclient = getQueueClient(Settings.Queue16, "amqp");
            var dataConSer = new DataContractSerializer(typeof(TestClass));

            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass("ServiceBussSdk", 123456);

            using (MemoryStream stream = new MemoryStream())
            {
                dataConSer.WriteObject(stream, tClass);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    string strMsg = reader.ReadToEnd();
                    Message msg = new Message(strMsg)
                    {
                        Properties = { { key, value } },
                        MessageId = "12345"
                    };

                    qclient.Send(msg).Wait();

                    var rcvStrMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;

                    Assert.True(rcvStrMsg != null);

                    var msgStream = rcvStrMsg.GetBodyStream();
                    dataConSer = new DataContractSerializer(typeof(string));
                    string msgAsString = dataConSer.ReadObject(msgStream) as string;
                    using (MemoryStream memStr = new MemoryStream())
                    {
                        using (StreamWriter sW = new StreamWriter(memStr))
                        {
                            sW.Write(msgAsString);

                            sW.Flush();

                            memStr.Position = 0;
                            dataConSer = new DataContractSerializer(typeof(TestClass));

                            TestClass rcvObj = dataConSer.ReadObject(memStr) as TestClass;
                            Assert.True(rcvObj != null);
                        }

                    }  
                }
            }
        }
        /// <summary>
        /// Send message to Queue via amqp protocol
        ///  It sends message custom body and asserts if the custom body has same type as it has been sent
        /// </summary
        [Fact]
        public void SendToQueueUsingAmqp_CustomBodyJsonConvert()
        {
            var qclient = getQueueClient(Settings.Queue17, "amqp");
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass("ServiceBussSdk", 123456);
            var strMgs = JsonConvert.SerializeObject(tClass);
            Message msg = new Message(strMgs)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            qclient.Send(msg).Wait();
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result.GetBody<string>();
            Assert.True(rcvMsg != null);
            TestClass rcvObj = JsonConvert.DeserializeObject<TestClass>(rcvMsg);
            Assert.True(rcvObj.PropertyOne == tClass.PropertyOne);
            Assert.True(rcvObj.PropertyTwo == tClass.PropertyTwo);
        }
        /// <summary>
        /// Send message to Queue via amqp protocol
        ///  It sends message custom body and asserts if the custom body has same type as it has been sent
        /// </summary
        [Fact]
        public void SendToQueueUsingAmqp_CustomBodyXmlSerializer()
        {
            var qclient = getQueueClient(Settings.Queue18, "amqp");
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass("ServicebussOpenSdk",522);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestClass));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, tClass);
                var txt =  textWriter.ToString();
                Message msg = new Message(txt)
                {
                    Properties = { { key, value } },
                    MessageId = "12345"
                };
                qclient.Send(msg).Wait();
            }
           
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result.GetBody<string>();
            Assert.True(rcvMsg != null);
            using (StringReader textReader = new StringReader(rcvMsg))
            {
                var rcvObj = xmlSerializer.Deserialize(textReader) as TestClass;

                Assert.True(rcvObj.PropertyOne == tClass.PropertyOne);
                Assert.True(rcvObj.PropertyTwo == tClass.PropertyTwo);
            }  
        }

    }

}



