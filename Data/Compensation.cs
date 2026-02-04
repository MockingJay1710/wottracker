using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTTracker.Data
{
    public class Compensation
    {
        public int ID { get; set; }
        public int DowntimeID { get; set; }
        public int? ActivitySessionID { get; set; }
        public float CompensatedMinutes { get; set; } 
        public string Reason { get; set; } //  "Permission", "Conge", "Compensation"
    }
}
