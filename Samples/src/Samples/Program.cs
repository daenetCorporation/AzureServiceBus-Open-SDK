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
            Sender.SendMessageToQueue("Hello world", m_conString, m_path, "http");
            Receiver.ReadMessageFromQueue(m_conString, m_path, "http");

            Console.WriteLine("Please enter!!");
            Console.ReadLine();
        }
    }
}
