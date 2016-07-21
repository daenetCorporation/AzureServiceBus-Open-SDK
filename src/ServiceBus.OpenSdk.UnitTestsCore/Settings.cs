using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBus.OpenSdk.UnitTestsCore
{
    public class Settings
    {
        public static string EndPoint { get; set; }

        static Settings()
        {
            string credentialsConfigFileName = "credentials.json";

            var builder = new ConfigurationBuilder();

            if (File.Exists(credentialsConfigFileName))
            {
                builder.AddJsonFile(credentialsConfigFileName);

                var cfg = builder.Build();

                EndPoint = cfg["SbConnectionString"];
            }
            else
                throw new Exception(@"Please provide the credentials for Service Bus entities in the config file ${credentialsConfigFileName}.");
        }
    }
}
