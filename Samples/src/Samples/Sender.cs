using ServiceBus.OpenSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samples
{
    public class Sender
    {
        public static void SendMessageToQueue(string mgs, string connectionString, string path, string protocol)
        {
            try
            {
                var qclient = QueueClient.FromConnectionString(connectionString, path, protocol);
                string key = "test";
                int value = 12345;
                Message msg = new Message(mgs)
                {
                    Properties = { { key, value } }
                };
                qclient.Send(msg).Wait();
                Console.WriteLine("Send message: " + mgs);
                Console.WriteLine("Message send successfully!!!");
            }catch(Exception e)
            {
                Console.WriteLine("Message did not send, Please try again !!");
            }
        }
    }
}
