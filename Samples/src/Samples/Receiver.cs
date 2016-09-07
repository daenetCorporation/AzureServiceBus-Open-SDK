using Newtonsoft.Json;
using ServiceBus.OpenSdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Samples
{
    public class Receiver
    {
        public void ReadMessageFromQueue()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "amqp"; //Protocol name, currently support "http" and "amqp" protocols
            QueueClient client = QueueClient.FromConnectionString(connectionString, path, protocol);
            while (true)
            {
                var rcvMessage = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
                if (rcvMessage == null)
                    break;
            }
        }
        public void ReadClassObjectFromQueueUsingHttp()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "http"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;
            TestClass classObject = rcvMsg.GetBody<TestClass>();
        }
        public void ReadClassObjectFromQueueUsingAmqpWithDataContractSerializer()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "amqp"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
            var rcvStrMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result;  
            var msgStream = rcvStrMsg.GetBodyStream();
            var dataConSer = new DataContractSerializer(typeof(string));
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
                }
            }
        }
        public void ReadClassObjectFromQueueUsingAmqpWithXmlSerializer()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "amqp"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result.GetBody<string>();
            TestClass rcvObj;
            var xmlSerializer = new XmlSerializer(typeof(TestClass));
            using (StringReader textReader = new StringReader(rcvMsg))
            {
                 rcvObj = xmlSerializer.Deserialize(textReader) as TestClass;
            }
        }
        public void ReadClassObjectFromQueueUsingAmqpWithJsonConvert()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "amqp"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
            var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result.GetBody<string>();
            TestClass rcvObj = JsonConvert.DeserializeObject<TestClass>(rcvMsg);
        }
        public void RcvAbandonFromQueue()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "http"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
            var rcvMsg = qclient.Receive(ReceiveMode.PeekLock);
            qclient.Abandon(rcvMsg.Result);
            rcvMsg = qclient.Receive(ReceiveMode.PeekLock);
        }

        public void RcvCompleteFromQueue()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var path = "queuename"; //Name of the Queue
            var protocol = "http"; //Protocol name, currently support http and amqp protocols
            var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
            var rcvMsg = qclient.Receive(ReceiveMode.PeekLock);
            qclient.Complete(rcvMsg.Result);
            rcvMsg = qclient.Receive(ReceiveMode.PeekLock);
        }

        //--------------------Subscription---------------------------
        public void ReceiveMessageFromSubscription()
        {
            var connectionString = "Endpoint=sb:// ;SharedAccessKeyName= ;SharedAccessKey= "; // "Your Connection String with manage rights";
            var topicName = "iottopic"; //Name of the Topic
            var subName = "iotsubscription";// Subscription name in specified Topic
            var protocol = "http"; //Protocol name, currently support http and amqp protocols
            SubscriptionClient client = SubscriptionClient.FromConnectionString(connectionString, topicName, subName, protocol);
            while (true)
            {
                Message rcvMessage = client.Receive(ReceiveMode.ReceiveAndDelete).Result;
                if (rcvMessage == null)
                    break;
            }
        }
    }
}

