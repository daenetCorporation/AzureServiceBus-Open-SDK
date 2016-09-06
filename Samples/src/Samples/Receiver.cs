using ServiceBus.OpenSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samples
{
    public class Receiver
    {
        public static void ReadMessageFromQueue(string connectionString, string path, string protocol)
        {
            try
            {
                var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);

                var rcvMsg = qclient.Receive(ReceiveMode.ReceiveAndDelete).Result.GetBody<string>();
                Console.WriteLine("Received message: " + rcvMsg);
            }catch(Exception e)
            {
                Console.WriteLine("Message did not receive!!!");
            }
        }
    }
}
