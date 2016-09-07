using ServiceBus.OpenSdk;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Samples
{
    public class Sender
    {
        public void SendMessageToQueue()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "amqp"; //Protocol name, currently support http and amqp protocols 
            QueueClient client = QueueClient.FromConnectionString(connectionString, path, protocol);
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            client.Send(msg).Wait();
        }

        public void SendClassObjectToQueueUsingHttp()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "http"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass("ServiceBussSdk", 123456);
            Message msg = new Message(tClass)
            {
                Properties = { { key, value } },
                MessageId = "12345"
            };
            qclient.Send(msg).Wait();
        }

        public void SendClassObjectToQueueUsingAmqpWithDataContractSerializer()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "amqp"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
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
                }
            }
        }

        public void SendClassObjectToQueueUsingAmqpWithJsonConvert()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "amqp"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
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
        }
        public void SendClassObjectToQueueUsingAmqpWithXmlSerializer()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "amqp"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
            string key = "test";
            int value = 12345;
            TestClass tClass = new TestClass("ServicebussOpenSdk", 522);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestClass));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, tClass);
                var txt = textWriter.ToString();
                Message msg = new Message(txt)
                {
                    Properties = { { key, value } },
                    MessageId = "12345"
                };
                qclient.Send(msg).Wait();
            }
        }

        //-----------------------Topic----------------------------------
        public void SendMessageToTopic()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var topicName = "iottopic"; //Name of the Topic
            var protocol = "http"; //Protocol name, currently support "http" and "amqp" protocols
            TopicClient topicClient = TopicClient.FromConnectionString(connectionString, topicName, protocol);
            string key = "test";
            int value = 12345;
            Message msg = new Message("Sample message")
            {
                Properties = { { key, value } }
            };
            topicClient.Send(msg).Wait();
        }

        //--------------------Event Hub------------------------------------
        public void SendMessageToEventHub()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var eventHubName = "eventHubName"; //Name of the Event Hub
            var protocol = "http"; //Protocol name, currently support http and amqp protocols
            EventHubClient eventHubClient = EventHubClient.FromConnectionString(connectionString, eventHubName, protocol);
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
