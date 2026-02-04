using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOTTracker.Data;

namespace WOTTracker.Forms
{
    public partial class RecoverySummaryForm : Form
    {
        private readonly List<ActivitySession> _recoveredSessions;

        // --- STEP 1: Modify the constructor to accept the list ---
        public RecoverySummaryForm(List<ActivitySession> recoveredSessions)
        {
            InitializeComponent();

            _recoveredSessions = recoveredSessions;
            PopulateSummary();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            

            // Position the form
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(workingArea.Right - this.Width, workingArea.Bottom - this.Height);
        }


        /// <summary>
        /// Formats the list of sessions and displays it in the ListBox.
        /// </summary>
        private void PopulateSummary()
        {
            // Clear any existing items from the ListBox.
            listBoxSummary.Items.Clear();

            // Group the recovered sessions by date for a clean, day-by-day summary.
            var sessionsByDay = _recoveredSessions.GroupBy(s => s.StartTime.Date)
                                                  .OrderBy(g => g.Key); // Order the days chronologically

            foreach (var dayGroup in sessionsByDay)
            {
                // --- Add a header for each day ---
                // We add the date as a separate, bolded-looking item.
                listBoxSummary.Items.Add("");
                listBoxSummary.Items.Add($"{dayGroup.Key:dddd, MMMM dd, yyyy}");

                // --- Add the individual sessions for that day ---
                foreach (var session in dayGroup.OrderBy(s => s.StartTime))
                {
                    string sessionDetails = $"--> From: {session.StartTime:HH:mm:ss}  To: {session.EndTime:HH:mm:ss}  ({FormatDuration(session.DurationMinutes)})";
                    listBoxSummary.Items.Add(sessionDetails);
                }
            }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string FormatDuration(double minutes)
        {
            if (minutes < 1) return "< 1 min";
            var t = TimeSpan.FromMinutes(Math.Round(minutes));
            return $"{(int)t.TotalHours}h {t.Minutes}m";
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {

        }

        private void labelInfo_Click(object sender, EventArgs e)
        {

        }

        private void RecoverySummaryForm_Load(object sender, EventArgs e)
        {

        }
    }
}
