using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTTracker.Data
{
    public class OvertimeSourceSummary
    {
        public int ActivitySessionID { get; set; }
        public DateTime Date { get; set; }
        public double AvailableOvertimeMinutes { get; set; }
    }
}
