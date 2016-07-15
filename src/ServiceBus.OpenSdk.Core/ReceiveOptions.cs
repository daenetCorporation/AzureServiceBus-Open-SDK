using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.OpenSdk
{
    public class ReceiveOptions
    {
        public ReceiveOptions()
        {
            this.TimeOut = TimeSpan.FromSeconds(60);
            this.ReceiveMode = ReceiveMode.PeekLock;
            this.SequenceNumber = 0;
        }

        public TimeSpan TimeOut { get; set; }
        public ReceiveMode  ReceiveMode { get; set; }
        public long SequenceNumber { get; set; }
    }
}
