namespace WOTTracker.Forms
{
    partial class DowntimeDecisionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DowntimeDecisionForm));
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.rbConge = new System.Windows.Forms.RadioButton();
            this.rbPermission = new System.Windows.Forms.RadioButton();
            this.rbCompensate = new System.Windows.Forms.RadioButton();
            this.lblOptionsTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelQuestion = new System.Windows.Forms.Label();
            this.btnAcknowledge = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.listBoxDowntimeChunks = new System.Windows.Forms.ListBox();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(28)))), ((int)(((byte)(38)))));
            this.panelTop.Controls.Add(this.pictureBox9);
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Controls.Add(this.labelTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(466, 45);
            this.panelTop.TabIndex = 6;
            // 
            // pictureBox9
            // 
            this.pictureBox9.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox9.Image = global::WOTTracker.Properties.Resources.yazaki;
            this.pictureBox9.Location = new System.Drawing.Point(44, 4);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(80, 41);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox9.TabIndex = 4;
            this.pictureBox9.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::WOTTracker.Properties.Resources.looogo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 42);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(159, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(184, 25);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Downtime Recovery";
            // 
            // pnlOptions
            // 
            this.pnlOptions.AutoSize = true;
            this.pnlOptions.Controls.Add(this.btnCancel);
            this.pnlOptions.Controls.Add(this.btnApply);
            this.pnlOptions.Controls.Add(this.rbConge);
            this.pnlOptions.Controls.Add(this.rbPermission);
            this.pnlOptions.Controls.Add(this.rbCompensate);
            this.pnlOptions.Controls.Add(this.lblOptionsTitle);
            this.pnlOptions.Location = new System.Drawing.Point(0, 257);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(466, 242);
            this.pnlOptions.TabIndex = 12;
            this.pnlOptions.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Aqua;
            this.btnCancel.Location = new System.Drawing.Point(272, 186);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 31);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnApply.FlatAppearance.BorderSize = 0;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnApply.ForeColor = System.Drawing.Color.Aqua;
            this.btnApply.Location = new System.Drawing.Point(107, 186);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 31);
            this.btnApply.TabIndex = 14;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Visible = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // rbConge
            // 
            this.rbConge.AutoSize = true;
            this.rbConge.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.rbConge.ForeColor = System.Drawing.SystemColors.Window;
            this.rbConge.Location = new System.Drawing.Point(71, 55);
            this.rbConge.Name = "rbConge";
            this.rbConge.Size = new System.Drawing.Size(158, 29);
            this.rbConge.TabIndex = 13;
            this.rbConge.TabStop = true;
            this.rbConge.Text = "Justify as Congé";
            this.rbConge.UseVisualStyleBackColor = true;
            // 
            // rbPermission
            // 
            this.rbPermission.AutoSize = true;
            this.rbPermission.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.rbPermission.ForeColor = System.Drawing.SystemColors.Window;
            this.rbPermission.Location = new System.Drawing.Point(71, 131);
            this.rbPermission.Name = "rbPermission";
            this.rbPermission.Size = new System.Drawing.Size(207, 29);
            this.rbPermission.TabIndex = 12;
            this.rbPermission.TabStop = true;
            this.rbPermission.Text = "Justify as a permission";
            this.rbPermission.UseVisualStyleBackColor = true;
            // 
            // rbCompensate
            // 
            this.rbCompensate.AutoSize = true;
            this.rbCompensate.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.rbCompensate.ForeColor = System.Drawing.SystemColors.Window;
            this.rbCompensate.Location = new System.Drawing.Point(71, 93);
            this.rbCompensate.Name = "rbCompensate";
            this.rbCompensate.Size = new System.Drawing.Size(243, 29);
            this.rbCompensate.TabIndex = 11;
            this.rbCompensate.TabStop = true;
            this.rbCompensate.Text = "Compensate with overtime";
            this.rbCompensate.UseVisualStyleBackColor = true;
            this.rbCompensate.CheckedChanged += new System.EventHandler(this.rbCompensate_CheckedChanged);
            // 
            // lblOptionsTitle
            // 
            this.lblOptionsTitle.AutoSize = true;
            this.lblOptionsTitle.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.lblOptionsTitle.ForeColor = System.Drawing.Color.White;
            this.lblOptionsTitle.Location = new System.Drawing.Point(138, 11);
            this.lblOptionsTitle.Name = "lblOptionsTitle";
            this.lblOptionsTitle.Size = new System.Drawing.Size(191, 25);
            this.lblOptionsTitle.TabIndex = 10;
            this.lblOptionsTitle.Text = "Please select a reason :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 8;
            // 
            // labelQuestion
            // 
            this.labelQuestion.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelQuestion.ForeColor = System.Drawing.Color.White;
            this.labelQuestion.Location = new System.Drawing.Point(95, 253);
            this.labelQuestion.Name = "labelQuestion";
            this.labelQuestion.Size = new System.Drawing.Size(277, 60);
            this.labelQuestion.TabIndex = 9;
            this.labelQuestion.Text = "Do you want to resolve this now?";
            // 
            // btnAcknowledge
            // 
            this.btnAcknowledge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnAcknowledge.FlatAppearance.BorderSize = 0;
            this.btnAcknowledge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAcknowledge.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAcknowledge.ForeColor = System.Drawing.Color.Aqua;
            this.btnAcknowledge.Location = new System.Drawing.Point(220, 362);
            this.btnAcknowledge.Name = "btnAcknowledge";
            this.btnAcknowledge.Size = new System.Drawing.Size(130, 82);
            this.btnAcknowledge.TabIndex = 10;
            this.btnAcknowledge.Text = "Acknowledge and Continue";
            this.btnAcknowledge.UseVisualStyleBackColor = false;
            this.btnAcknowledge.Click += new System.EventHandler(this.btnAcknowledge_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNext.ForeColor = System.Drawing.Color.Aqua;
            this.btnNext.Location = new System.Drawing.Point(113, 387);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(66, 32);
            this.btnNext.TabIndex = 11;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelInfo.ForeColor = System.Drawing.SystemColors.Window;
            this.labelInfo.Location = new System.Drawing.Point(65, 61);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(35, 20);
            this.labelInfo.TabIndex = 13;
            this.labelInfo.Text = "Info";
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDate.ForeColor = System.Drawing.SystemColors.Window;
            this.labelDate.Location = new System.Drawing.Point(126, 107);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(53, 25);
            this.labelDate.TabIndex = 14;
            this.labelDate.Text = "Date";
            // 
            // listBoxDowntimeChunks
            // 
            this.listBoxDowntimeChunks.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.listBoxDowntimeChunks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxDowntimeChunks.Font = new System.Drawing.Font("Consolas", 11F);
            this.listBoxDowntimeChunks.ForeColor = System.Drawing.SystemColors.Window;
            this.listBoxDowntimeChunks.FormattingEnabled = true;
            this.listBoxDowntimeChunks.ItemHeight = 18;
            this.listBoxDowntimeChunks.Location = new System.Drawing.Point(100, 162);
            this.listBoxDowntimeChunks.Name = "listBoxDowntimeChunks";
            this.listBoxDowntimeChunks.Size = new System.Drawing.Size(261, 56);
            this.listBoxDowntimeChunks.TabIndex = 15;
            // 
            // DowntimeDecisionForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(466, 537);
            this.Controls.Add(this.listBoxDowntimeChunks);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.labelDate);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnAcknowledge);
            this.Controls.Add(this.labelQuestion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DowntimeDecisionForm";
            this.Text = "DowntimeDecisionForm";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelQuestion;
        private System.Windows.Forms.Button btnAcknowledge;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Label lblOptionsTitle;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.RadioButton rbConge;
        private System.Windows.Forms.RadioButton rbPermission;
        private System.Windows.Forms.RadioButton rbCompensate;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.ListBox listBoxDowntimeChunks;
    }
}