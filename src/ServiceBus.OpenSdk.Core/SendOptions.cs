using System;
using System.Collections.Generic;

namespace ServiceBus.OpenSdk
{
    public class SendOptions
    {
        public Dictionary<string, object> Properties { get; set; }
        public SendOptions()
        {
            Properties = new Dictionary<string, object>();
            Properties.Add("Timeout", TimeSpan.FromSeconds(60));         
        }      
    }
}
