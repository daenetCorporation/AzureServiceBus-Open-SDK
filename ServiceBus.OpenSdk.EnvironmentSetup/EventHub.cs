using Microsoft.ServiceBus;            
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.OpenSdk.EnvironmentSetup
{
    class EventHub
    {
        public static void CreateEventHub(string connectionString)
        {
            var nsMgr = NamespaceManager.CreateFromConnectionString(connectionString);
            if (nsMgr.EventHubExists("iotevenhub"))
                nsMgr.DeleteEventHub("iotevenhub");
            nsMgr.CreateEventHub("iotevenhub");

        }
    }
}
