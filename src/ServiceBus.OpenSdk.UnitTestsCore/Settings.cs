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
        public static string Queue0 { get; set; }
        public static string Queue1 { get; set; }
        public static string Queue2 { get; set; }
        public static string Queue3 { get; set; }
        public static string Queue4 { get; set; }
        public static string Queue5 { get; set; }
        public static string Queue6 { get; set; }
        public static string Queue7 { get; set; }
        public static string Queue8 { get; set; }
        public static string Queue9 { get; set; }
        public static string Queue10 { get; set; }
        public static string Queue11 { get; set; }
        public static string Queue12 { get; set; }
        public static string Queue13 { get; set; }
        public static string Queue14 { get; set; }
        public static string Queue15 { get; set; }
        public static string Queue16 { get; set; }
        public static string Queue17 { get; set; }
        public static string Queue18 { get; set; }
        public static string Topic0 { get; set; }
        public static string Topic1 { get; set; }
        public static string Topic2 { get; set; }
        public static string Topic3 { get; set; }
        public static string Topic4 { get; set; }
        public static string Topic5 { get; set; }
        public static string Topic6 { get; set; }
        public static string Topic7 { get; set; }
        public static string Topic8 { get; set; }
        public static string Topic9 { get; set; }
        public static string Topic10 { get; set; }
        public static string Topic11 { get; set; }
        public static string Topic12 { get; set; }
        public static string Topic13 { get; set; }
        public static string Topic14 { get; set; }
        public static string Topic15 { get; set; }

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
                Queue17 = entities["QueueName17"];
                Queue18 = entities["QueueName18"];
                Topic0 = entities["TopicName0"];
                Topic1 = entities["TopicName1"];
                Topic2 = entities["TopicName2"];
                Topic3 = entities["TopicName3"];
                Topic4 = entities["TopicName4"];
                Topic5 = entities["TopicName5"];
                Topic6 = entities["TopicName6"];
                Topic7 = entities["TopicName7"];
                Topic8 = entities["TopicName8"];
                Topic9 = entities["TopicName9"];
                Topic10 = entities["TopicName10"];
                Topic11 = entities["TopicName11"];
                Topic12 = entities["TopicName12"];
                Topic13 = entities["TopicName13"];
                Topic14 = entities["TopicName14"];
                Topic15 = entities["TopicName15"];




            }
            else
                throw new Exception(@"Please provide the credentials for Service Bus entities in the config file ${credentialsConfigFileName}.");
        }
    }
}
