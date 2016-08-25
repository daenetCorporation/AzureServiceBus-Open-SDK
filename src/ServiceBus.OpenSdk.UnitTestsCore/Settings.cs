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
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ServiceBus.OpenSdk.UnitTestsCore
{
    public class Settings
    {
        public static string EndPoint { get; set; }
        public static string Queue0;
        public static string Queue1;
        public static string Queue2;
        public static string Queue3;
        public static string Queue4;
        public static string Queue5;
        public static string Queue6;
        public static string Queue7;
        public static string Queue8;
        public static string Queue9;
        public static string Queue10;
        public static string Queue11;
        public static string Queue12;
        public static string Queue13;
        public static string Queue14;
        public static string Queue15;
        public static string Queue16;

        static Settings()
        {
            string credentialsConfigFileName = "credentials.json";

            var builder = new ConfigurationBuilder();

            if (File.Exists(credentialsConfigFileName))
            {
                builder.AddJsonFile(credentialsConfigFileName);

                var cfg = builder.Build();

                EndPoint = cfg["SbConnectionString"];
                var entities = cfg.GetSection("RequiredEntities");
                Queue0 = entities["QueueName0"];
                Queue1 = entities["QueueName1"];
                Queue2 = entities["QueueName2"];
                Queue3 = entities["QueueName3"];
                Queue4 = entities["QueueName4"];
                Queue5 = entities["QueueName5"];
                Queue6 = entities["QueueName6"];
                Queue7 = entities["QueueName7"];
                Queue8 = entities["QueueName8"];
                Queue9 = entities["QueueName9"];
                Queue10 = entities["QueueName10"];
                Queue11 = entities["QueueName11"];
                Queue12 = entities["QueueName12"];
                Queue13 = entities["QueueName13"];
                Queue14 = entities["QueueName14"];
                Queue15 = entities["QueueName15"];
                Queue16 = entities["QueueName16"]; 


            }
            else
                throw new Exception(@"Please provide the credentials for Service Bus entities in the config file ${credentialsConfigFileName}.");
        }
    }
}
