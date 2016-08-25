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
namespace ServiceBus.OpenSdk.EnvironmentSetup
{
    class Program
    {
        private static string ConnectionString = "Endpoint=sb://iotlabcore.servicebus.windows.net/;SharedAccessKeyName=iotLabCore;SharedAccessKey=QbH/yR4rUHa80Lmiz7oCVl0bBIEclfMBZ/luFkQv0sA=";
        static void Main(string[] args)
        {
            QueueSetup.CreateQueue(17, ConnectionString);

        }
    }
}
