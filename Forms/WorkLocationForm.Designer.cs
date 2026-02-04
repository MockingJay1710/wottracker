namespace WOTTracker.Forms
{
    partial class WorkLocationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelQuestion = new System.Windows.Forms.Label();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnOffice = new System.Windows.Forms.Button();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            this.SuspendLayout();
            // 
            // labelQuestion
            // 
            this.labelQuestion.AutoSize = true;
            this.labelQuestion.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.labelQuestion.ForeColor = System.Drawing.SystemColors.Window;
            this.labelQuestion.Location = new System.Drawing.Point(77, 129);
            this.labelQuestion.Name = "labelQuestion";
            this.labelQuestion.Size = new System.Drawing.Size(319, 25);
            this.labelQuestion.TabIndex = 1;
            this.labelQuestion.Text = "Where are you working from today?";
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnHome.ForeColor = System.Drawing.Color.Aqua;
            this.btnHome.Location = new System.Drawing.Point(92, 200);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(116, 65);
            this.btnHome.TabIndex = 4;
            this.btnHome.Text = "Working from Home";
            this.btnHome.UseVisualStyleBackColor = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click_1);
            // 
            // btnOffice
            // 
            this.btnOffice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnOffice.FlatAppearance.BorderSize = 0;
            this.btnOffice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOffice.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOffice.ForeColor = System.Drawing.Color.Aqua;
            this.btnOffice.Location = new System.Drawing.Point(271, 200);
            this.btnOffice.Name = "btnOffice";
            this.btnOffice.Size = new System.Drawing.Size(116, 65);
            this.btnOffice.TabIndex = 5;
            this.btnOffice.Text = "Working from Office";
            this.btnOffice.UseVisualStyleBackColor = false;
            this.btnOffice.Click += new System.EventHandler(this.btnOffice_Click_1);
            // 
            // pictureBox9
            // 
            this.pictureBox9.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox9.Image = global::WOTTracker.Properties.Resources.work;
            this.pictureBox9.Location = new System.Drawing.Point(184, 28);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(102, 78);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox9.TabIndex = 7;
            this.pictureBox9.TabStop = false;
            // 
            // WorkLocationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(471, 318);
            this.Controls.Add(this.pictureBox9);
            this.Controls.Add(this.btnOffice);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.labelQuestion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WorkLocationForm";
            this.Text = "WorkLocationForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelQuestion;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnOffice;
        private System.Windows.Forms.PictureBox pictureBox9;
    }
}