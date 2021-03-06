﻿//=======================================================================================
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
using Microsoft.ServiceBus;

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
