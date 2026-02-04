using System;
using System.Drawing;
using System.Windows.Forms;
using WOTTracker.Configuration;

namespace WOTTracker
{
    public partial class SetupForm : Form
    {
        private readonly ConfigurationSet _existingConfig;

        public SetupForm()
        {
            // The constructor's only job is to initialize the components
            // and set the default values for the controls.
            InitializeComponent();
            InitializeFormDefaults();
        }

        // --- NEW CONSTRUCTOR for reconfiguring ---
        public SetupForm(ConfigurationSet existingConfig)
        {
            InitializeComponent();
            _existingConfig = existingConfig;
            InitializeFormDefaults(existingConfig); // Pass the existing config
        }

        // --- MODIFIED method to handle both cases ---
        private void InitializeFormDefaults(ConfigurationSet config)
        {
            // Populate the Role ComboBox (this is always the same)
            comboRole.Items.AddRange(new string[] { "Coordinator", "Responsible", "Supervisor", "Manager" });
            

            if (config != null)
            {
                // --- PRE-FILL MODE ---
                // We are editing an existing configuration.
                this.Text = "Reconfigure Application";
                label1.Text = "Reconfigure Application";

                comboRole.SelectedItem = config.UserRole;
                userName.Text = config.UserName;
                dtpStartTime.Value = DateTime.Today + config.WorkStartTime;
                dtpEndTime.Value = DateTime.Today + config.WorkEndTime;
                breakStartTime.Value = DateTime.Today + config.BreakStartTime;
                breakEndTime.Value = DateTime.Today + config.BreakEndTime;
                numStartTolerance.Value = config.StartTimeToleranceMinutes;
                numEndTolerance.Value = config.EndTimeToleranceMinutes;

                // Pre-fill Notification settings
                dtpNotificationTime.Value = DateTime.Today + config.NotificationSendTime;
                txtNotificationRecipient.Text = config.NotificationRecipient;
                txtNotificationSubject.Text = config.NotificationSubject;
                txtNotificationBody.Text = config.NotificationBody;
            }
            else
            {
                // --- DEFAULT MODE ---
                // This is the first run.
                comboRole.SelectedIndex = 0;
                userName.Text = Environment.UserName;


                comboRole.SelectedIndex = 0;
                dtpStartTime.Value = DateTime.Today.AddHours(8);
                dtpEndTime.Value = DateTime.Today.AddHours(17);
                breakStartTime.Value = DateTime.Today.AddHours(12.5);
                breakEndTime.Value = DateTime.Today.AddHours(13);
                numStartTolerance.Value = 10;
                numEndTolerance.Value = 10;
                dtpNotificationTime.Value = DateTime.Today.AddHours(15); // Set default to 15:00
                txtNotificationRecipient.Text = "HR@yazaki.com";
                txtNotificationSubject.Text = "Home Office Status for {UserName}";
                txtNotificationBody.Text = "This is an automated notification to confirm that {UserName} is working from home today, {Date}.";
            }
            
            

            dtpStartTime.Format = DateTimePickerFormat.Custom;
            dtpStartTime.CustomFormat = "HH:mm";
            dtpStartTime.ShowUpDown = true;

            dtpEndTime.Format = DateTimePickerFormat.Custom;
            dtpEndTime.CustomFormat = "HH:mm";
            dtpEndTime.ShowUpDown = true;

            breakStartTime.Format = DateTimePickerFormat.Custom;
            breakStartTime.CustomFormat = "HH:mm";
            breakStartTime.ShowUpDown = true;
            

            breakEndTime.Format = DateTimePickerFormat.Custom;
            breakEndTime.CustomFormat = "HH:mm";
            breakEndTime.ShowUpDown = true;
            

            numStartTolerance.Minimum = 0;
            numStartTolerance.Maximum = 60;

            numEndTolerance.Minimum = 0;
            numEndTolerance.Maximum = 60;

            recipientEmail.Text = "omar.temsamani@yazaki-europe.com"; // Default empty, user can set it later
            senderEmail.Text = "OverTimeReport@yazaki-europe.com"; // Default empty, user can set it later
            smtpServer.Text = "smtp.yazaki-europe.com"; // Default empty, user can set it later
           
        }

        /// <summary>
        /// This event fires after the form is created but just before it is displayed.
        /// It is the most reliable place to apply custom styling.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e); // It's good practice to call the base method.

            // 1. Get the primary screen's working area.
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            // 2. Calculate the top-left point for our form.
            int x = workingArea.Right - this.Width;
            int y = workingArea.Bottom - this.Height;

            // 3. Set the form's location.
            this.Location = new Point(x, y);
            
        }

        private void InitializeFormDefaults()
        {
            comboRole.Items.AddRange(new string[] { "Coordinator", "Responsible", "Supervisor", "Manager" });
            comboRole.SelectedIndex = 0;

            dtpStartTime.Format = DateTimePickerFormat.Custom;
            dtpStartTime.CustomFormat = "HH:mm";
            dtpStartTime.ShowUpDown = true;
            dtpStartTime.Value = DateTime.Today.AddHours(8);

            dtpEndTime.Format = DateTimePickerFormat.Custom;
            dtpEndTime.CustomFormat = "HH:mm";
            dtpEndTime.ShowUpDown = true;
            dtpEndTime.Value = DateTime.Today.AddHours(17);

            breakStartTime.Format = DateTimePickerFormat.Custom;
            breakStartTime.CustomFormat = "HH:mm";
            breakStartTime.ShowUpDown = true;
            breakStartTime.Value = DateTime.Today.AddHours(12.5);

            breakEndTime.Format = DateTimePickerFormat.Custom;
            breakEndTime.CustomFormat = "HH:mm";
            breakEndTime.ShowUpDown = true;
            breakEndTime.Value = DateTime.Today.AddHours(13);

            numStartTolerance.Minimum = 0;
            numStartTolerance.Maximum = 60;
            numStartTolerance.Value = 10;

            numEndTolerance.Minimum = 0;
            numEndTolerance.Maximum = 60;
            numEndTolerance.Value = 10;

            recipientEmail.Text = "omar.temsamani@yazaki-europe.com"; // Default empty, user can set it later
            userName.Text = Environment.UserName; // Default to current Windows user name
            senderEmail.Text = "OverTimeReport@yazaki-europe.com"; // Default empty, user can set it later
            smtpServer.Text = "smtp.yazaki-europe.com"; // Default empty, user can set it later
            dtpNotificationTime.Value = DateTime.Today.AddHours(15); // Set default to 15:00
            txtNotificationRecipient.Text = "HR@yazaki.com";
            txtNotificationSubject.Text = "Home Office Status for {UserName}";
            txtNotificationBody.Text = "This is an automated notification to confirm that {UserName} is working from home today, {Date}.";

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ShowPage(2);
        }



        // These are the empty stubs to fix the designer errors.
        private void label1_Click(object sender, EventArgs e) { }

        #region Draggable Borderless Form Logic
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

        private void labelStartTime_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            bool isInitialSetup = _existingConfig == null;

            if (isInitialSetup)
            {
                // Ask the user for confirmation before quitting. This prevents accidental exits.
                DialogResult confirmation = MessageBox.Show(
                    "Are you sure you want to cancel the setup? The application cannot run without being configured and will now exit.",
                    "Confirm Exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // Check the user's response.
                if (confirmation == DialogResult.Yes)
                {
                    // If they confirmed, set the form's DialogResult to Cancel.
                    // This signals to Program.cs that the setup was not completed successfully.
                    this.DialogResult = DialogResult.Cancel;

                    // Close the setup form. Program.cs will then handle the exit.
                    this.Close();
                }
            }
            else
            {
                // If this is a reconfiguration, just close the form without saving.
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        /// <summary>
        /// Controls which "page" (panel) of the wizard is currently visible.
        /// </summary>
        private void ShowPage(int pageNumber)
        {
            if (pageNumber == 1)
            {
                // Show Core Config Page
                pnlCoreConfig.Visible = true;
                pnlNotifications.Visible = false;

                btnNext.Visible = true;
                btnCancel.Visible = true;
                btnBack.Visible = false;
                btnSave.Visible = false;
            }
            else if (pageNumber == 2)
            {
                // Show Notifications Page
                pnlCoreConfig.Visible = false;
                pnlNotifications.Visible = true;

                btnNext.Visible = false;
                btnCancel.Visible = true; // Cancel is always available
                btnBack.Visible = true;
                btnSave.Visible = true;
            }
        }


    

        private void breakStartTime_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void SetupForm_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            // ... validation ...

            // Create a new ConfigurationSet object from the form's controls
            var newConfig = new ConfigurationSet
            {
                UserRole = comboRole.SelectedItem.ToString(),
                UserName = userName.Text.Trim(),
                WorkStartTime = dtpStartTime.Value.TimeOfDay,
                WorkEndTime = dtpEndTime.Value.TimeOfDay,
                BreakStartTime = breakStartTime.Value.TimeOfDay,
                BreakEndTime = breakEndTime.Value.TimeOfDay,
                StartTimeToleranceMinutes = (int)numStartTolerance.Value,
                EndTimeToleranceMinutes = (int)numEndTolerance.Value,
                RecipientEmail = recipientEmail.Text.Trim(),
                SenderEmail = senderEmail.Text.Trim(),
                SmtpServer = smtpServer.Text.Trim(),
                // From Panel 2
                NotificationSendTime = dtpNotificationTime.Value.TimeOfDay,
                NotificationRecipient = txtRecipientEmail.Text.Trim(),
                NotificationSubject = txtNotificationSubject.Text.Trim(),
                NotificationBody = txtNotificationBody.Text
            };

            try
            {
                // Save the entire set. This handles deactivating the old one and inserting the new one.
                ConfigurationManager.SaveNewConfiguration(newConfig);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save new configuration: {ex.Message}", "Error");
            }
        }

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            ShowPage(1);
        }
    }
}