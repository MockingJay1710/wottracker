using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTTracker.Data
{
    public class SystemEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; } // "Startup", "Shutdown", "Sleep", "Resume"
    }
}
