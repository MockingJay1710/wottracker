using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WOTTracker.Configuration;

namespace WOTTracker.Forms
{
    public enum NotificationType
    {
        Welcome,
        Warning,
        Error,
        Success
    }

    public partial class NotificationForm : Form
    {
        // The constructor now accepts the content to display.
        public NotificationForm(string message, NotificationType type)
        {
            InitializeComponent();

            // Set the content
            labelMessage.Text = message;

            // Set the icon based on the type
            // You can add custom icons to your project's Resources for this.
            switch (type)
            {
                case NotificationType.Success:
                    pictureBox9.Image = Properties.Resources.sign;
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case NotificationType.Welcome:
                    pictureBox9.Image = Properties.Resources.sign;
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case NotificationType.Warning:
                    pictureBox9.Image = Properties.Resources.alert;
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
                case NotificationType.Error:
                    pictureBox9.Image = Properties.Resources.cross;
                    System.Media.SystemSounds.Hand.Play();
                    break;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Position the form
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(workingArea.Right - this.Width, workingArea.Bottom - this.Height);
        }


        private void btnOk_Click_1(object sender, EventArgs e)
        {
            // Set the result and close the form.
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void labelMessage_Click(object sender, EventArgs e)
        {

        }
    }
}
