using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTTracker.Data
{
    public class ActivitySession
    {
        public int ID { get; set; }
        public int ConfigurationID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double DurationMinutes { get; set; }

        public string SessionType { get; set; }
        public double OvertimeMinutes { get; set; }
        public string Notes { get; set; }
        public string WorkLocation { get; set; }// "home" or "office"
    }
}
