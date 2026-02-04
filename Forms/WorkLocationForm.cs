using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WOTTracker.Forms
{
    // In Forms/WorkLocationForm.cs
    public partial class WorkLocationForm : Form
    {
        public WorkLocationForm()
        {
            InitializeComponent();
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            

            // Position the form
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(workingArea.Right - this.Width, workingArea.Bottom - this.Height);
        }

        private void btnHome_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes; // Let's use 'Yes' to mean Home Office
            this.Close();
        }

        private void btnOffice_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No; // And 'No' to mean At The Office
            this.Close();
        }
    }
}
