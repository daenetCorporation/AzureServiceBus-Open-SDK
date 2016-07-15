using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
