using System.Data;

namespace WOTTracker.Data
{
    public class ComprehensiveReport
    {
        public DataTable SummaryData { get; set; }
        public DataTable DailyBreakdownData { get; set; }
        public DataTable RawActivityData { get; set; }
        public DataTable CompensationData { get; set; }
        public DataTable DowntimeData { get; set; } // The property is already here.

        public ComprehensiveReport()
        {
            // Initialize the tables with their columns
            SummaryData = new DataTable("Summary");
            SummaryData.Columns.Add("Category", typeof(string));
            SummaryData.Columns.Add("TotalOvertime", typeof(string));

            DailyBreakdownData = new DataTable("DailyBreakdown");
            DailyBreakdownData.Columns.Add("Date", typeof(string));
            DailyBreakdownData.Columns.Add("DayType", typeof(string));
            DailyBreakdownData.Columns.Add("TotalActiveTime", typeof(string));
            DailyBreakdownData.Columns.Add("OfficialDowntime", typeof(string));
            DailyBreakdownData.Columns.Add("OfficialOvertime", typeof(string));

            // --- THE FIX IS HERE ---
            // We need to initialize the DowntimeData DataTable.
            // We don't need to define columns here, as it will be filled directly
            // by the SqlCeDataAdapter, which creates the columns automatically.
            DowntimeData = new DataTable("Downtime");
        }
    }
}