using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samples
{
    public class Program
    {
        static string  m_conString = "Endpoint=sb://iotlabcore.servicebus.windows.net/;SharedAccessKeyName=iotLabCore;SharedAccessKey=QbH/yR4rUHa80Lmiz7oCVl0bBIEclfMBZ/luFkQv0sA=";
        static string m_path = "iotqueue0"; //Name of the Queue
        public static void Main(string[] args)
        {
            Sender sender = new Sender();
            Receiver receiver = new Receiver();

            sender.SendMessageToQueue();
            receiver.ReadMessageFromQueue();

            //sender.SendClassObjectToQueueUsingHttp();
            //receiver.ReadClassObjectFromQueueUsingHttp();

            //sender.SendClassObjectToQueueUsingAmqpWithDataContractSerializer();
            //receiver.ReadClassObjectFromQueueUsingAmqpWithDataContractSerializer();

            //sender.SendClassObjectToQueueUsingAmqpWithJsonConvert();
            //receiver.ReadClassObjectFromQueueUsingAmqpWithJsonConvert();

            //sender.SendClassObjectToQueueUsingAmqpWithXmlSerializer();
            //receiver.ReadClassObjectFromQueueUsingAmqpWithXmlSerializer();

            //sender.SendMessageToTopic();
            //receiver.ReceiveMessageFromSubscription();   

            Console.WriteLine("Please enter!!");
            Console.ReadLine();
        }
    }
}
