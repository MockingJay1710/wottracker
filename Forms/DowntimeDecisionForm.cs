// In DowntimeDecisionForm.cs
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WOTTracker.Data;

namespace WOTTracker.Forms
{
    public partial class DowntimeDecisionForm : Form
    {
        private readonly DowntimeDaySummary _daySummary;
        private readonly double _availableOvertime;
        private readonly string _userRole;

        // --- NEW: A field to hold the amount just paid ---
        private readonly double? _amountJustPaid;

        public string SelectedReason { get; private set; } = "Acknowledge";

        public DowntimeDecisionForm(DowntimeDaySummary daySummary, double availableOvertime, string userRole, double? amountJustPaid = null)
        {
            InitializeComponent();

            _daySummary = daySummary;
            _availableOvertime = availableOvertime;
            _userRole = userRole;
            _amountJustPaid = amountJustPaid; // Store the amount
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            // 1. Get the primary screen's working area.
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            // 2. Calculate the top-left point for our form.
            int x = workingArea.Right - this.Width;
            int y = workingArea.Bottom - this.Height;

            // 3. Set the form's location.
            this.Location = new Point(x, y);

            PrepareInitialView();
        }

        void CenterLabelsHorizontally()
        {
            labelInfo.Left = (this.ClientSize.Width - labelInfo.Width) / 2;
            labelDate.Left = (this.ClientSize.Width - labelDate.Width) / 2;
            labelQuestion.Left = (this.ClientSize.Width - labelQuestion.Width) / 2;
        }


        /// <summary>
        /// Sets up the initial view of the form (the Yes/No question).
        /// </summary>
        private void PrepareInitialView()
        {
            // --- NEW, CONTEXT-AWARE MESSAGING ---
            if (_amountJustPaid.HasValue && _amountJustPaid.Value > 0)
            {
                // --- PARTIALLY COMPENSATED VIEW ---
                // This view is shown on the second+ iteration of the 'while' loop.
                labelQuestion.Width = this.ClientSize.Width - 40;
                labelInfo.Text = $"You successfully compensated {FormatDuration(_amountJustPaid.Value)}.";
                labelDate.Text = $"{_daySummary.Date:dddd, MMMM dd, yyyy}";
                labelQuestion.Text = $"There is still {FormatDuration(_daySummary.TotalDowntimeMinutes)} remaining. How would you like to proceed?";
                labelQuestion.ForeColor = Color.OrangeRed; // Highlight that action is still needed
                btnNext.Enabled = false; // Disable the Next button since we cant compensate again
                CenterLabelsHorizontally(); 
            }
            else
            {
                // --- INITIAL VIEW ---
                // This is the first time the user sees this day's downtime.
                labelInfo.Text = $"Unresolved downtime of {FormatDuration(_daySummary.TotalDowntimeMinutes)} was detected for:";
                labelDate.Text = $"{_daySummary.Date:dddd, MMMM dd, yyyy}";
                labelQuestion.Text = "Do you want to resolve this now?";
                labelQuestion.ForeColor = Color.White;

                CenterLabelsHorizontally();
            }

            // Populate the ListBox with the specific downtime intervals.
            listBoxDowntimeChunks.Items.Clear();
            foreach (var chunk in _daySummary.DowntimeRecords)
            {
                // Format each chunk for clear display.
                string item = $"-> From {chunk.StartTime:HH:mm} to {chunk.EndTime:HH:mm} ({FormatDuration(chunk.DurationMinutes)})";
                listBoxDowntimeChunks.Items.Add(item);
            }



            // Hide the detailed options panel and its buttons.
            pnlOptions.Visible = false;
            btnApply.Visible = false;
            btnCancel.Visible = false;

            // Show the initial decision buttons.
            btnNext.Visible = true;
            btnAcknowledge.Visible = true;
        }

        /// <summary>
        /// Switches the view to show the resolution options, dynamically adjusting for user role.
        /// </summary>
        private void PrepareOptionsView()
        {
            labelQuestion.Visible = false;
            btnNext.Visible = false;
            btnAcknowledge.Visible = false;

            bool canJustify = (_userRole == "Supervisor" || _userRole == "Manager" || _userRole == "Deputy");

            rbPermission.Visible = canJustify;
            rbCompensate.Visible = (_availableOvertime > 1);
            rbCompensate.Text = $"Compensate with Overtime ({FormatDuration(_availableOvertime)} available)";
            rbConge.Visible = true;

            AdjustOptionPositions(); // 🟢 Move this here *after* visibility is set

            // Select the first visible option
            RadioButton firstVisibleRadioButton = pnlOptions.Controls.OfType<RadioButton>()
                .Where(rb => rb.Visible)
                .OrderBy(rb => rb.Top)
                .FirstOrDefault();

            if (firstVisibleRadioButton != null)
                firstVisibleRadioButton.Checked = true;

            pnlOptions.Visible = true;
            btnApply.Visible = true;
            btnCancel.Visible = true;
        }


        /// <summary>
        /// Helper to dynamically reposition the radio buttons to avoid ugly gaps.
        /// </summary>
        private void AdjustOptionPositions()
        {
            int topPosition = 10; // Start position inside the panel

            // Filter and sort visible radio buttons by design order
            var visibleRadioButtons = new[] { rbCompensate, rbPermission, rbConge }
                .Where(rb => rb.Visible)
                .ToList();

            foreach (var rb in visibleRadioButtons)
            {
                rb.Top = topPosition;
                rb.Left = 10; // Optional: Align horizontally
                topPosition += rb.Height + 5; // Give natural spacing
            }
        }


        private void btnNext_Click(object sender, EventArgs e) => PrepareOptionsView();
        private void btnBack_Click(object sender, EventArgs e) => PrepareInitialView();

        private void btnAcknowledge_Click(object sender, EventArgs e)
        {
            SelectedReason = "Acknowledge";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (rbCompensate.Checked) SelectedReason = "Compensation";
            else if (rbPermission.Checked) SelectedReason = "Permission";
            else if (rbConge.Checked) SelectedReason = "Congé";
            else
            {
                MessageBox.Show("Please select a resolution option.", "Option Required");
                return; // Don't close the form if no option is selected.
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // User wants to cancel the entire process for now.
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #region Helpers

        private string FormatDuration(double minutes)
        {
            if (minutes < 1) return "0 minutes";
            var t = TimeSpan.FromMinutes(Math.Round(minutes));
            return $"{(int)t.TotalHours}h {t.Minutes}m";
        }



        #endregion

        #region Draggable Form Logic
        // This region was missing. Add it back.
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void PanelTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        #endregion

        private void rbCompensate_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}


