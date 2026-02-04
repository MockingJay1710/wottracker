namespace WOTTracker
{
    partial class SetupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.labelUserName = new System.Windows.Forms.Label();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.labelEndTime = new System.Windows.Forms.Label();
            this.comboRole = new System.Windows.Forms.ComboBox();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.btnNext = new System.Windows.Forms.Button();
            this.numStartTolerance = new System.Windows.Forms.NumericUpDown();
            this.labelStartTolerance = new System.Windows.Forms.Label();
            this.labelEndTolerance = new System.Windows.Forms.Label();
            this.numEndTolerance = new System.Windows.Forms.NumericUpDown();
            this.userName = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelBreakStartTime = new System.Windows.Forms.Label();
            this.labelBreakEndTime = new System.Windows.Forms.Label();
            this.breakStartTime = new System.Windows.Forms.DateTimePicker();
            this.breakEndTime = new System.Windows.Forms.DateTimePicker();
            this.recipientEmailLabel = new System.Windows.Forms.Label();
            this.senderEmailLabel = new System.Windows.Forms.Label();
            this.smtpServerLabel = new System.Windows.Forms.Label();
            this.recipientEmail = new System.Windows.Forms.TextBox();
            this.senderEmail = new System.Windows.Forms.TextBox();
            this.smtpServer = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlNotifications = new System.Windows.Forms.Panel();
            this.txtNotificationBody = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNotificationSubject = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRecipientEmail = new System.Windows.Forms.Label();
            this.dtpNotificationTime = new System.Windows.Forms.DateTimePicker();
            this.txtNotificationRecipient = new System.Windows.Forms.TextBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlCoreConfig = new System.Windows.Forms.Panel();
            this.labelRole = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numStartTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEndTolerance)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlNotifications.SuspendLayout();
            this.pnlCoreConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelUserName.ForeColor = System.Drawing.Color.LightGray;
            this.labelUserName.Location = new System.Drawing.Point(270, 21);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(99, 25);
            this.labelUserName.TabIndex = 1;
            this.labelUserName.Text = "User Name";
            // 
            // labelStartTime
            // 
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.BackColor = System.Drawing.Color.Transparent;
            this.labelStartTime.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelStartTime.ForeColor = System.Drawing.Color.LightGray;
            this.labelStartTime.Location = new System.Drawing.Point(25, 99);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(138, 25);
            this.labelStartTime.TabIndex = 2;
            this.labelStartTime.Text = "Work Start Time";
            this.labelStartTime.Click += new System.EventHandler(this.labelStartTime_Click);
            // 
            // labelEndTime
            // 
            this.labelEndTime.AutoSize = true;
            this.labelEndTime.BackColor = System.Drawing.Color.Transparent;
            this.labelEndTime.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelEndTime.ForeColor = System.Drawing.Color.LightGray;
            this.labelEndTime.Location = new System.Drawing.Point(269, 99);
            this.labelEndTime.Name = "labelEndTime";
            this.labelEndTime.Size = new System.Drawing.Size(132, 25);
            this.labelEndTime.TabIndex = 3;
            this.labelEndTime.Text = "Work End Time";
            // 
            // comboRole
            // 
            this.comboRole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.comboRole.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.comboRole.ForeColor = System.Drawing.Color.White;
            this.comboRole.FormattingEnabled = true;
            this.comboRole.Location = new System.Drawing.Point(30, 55);
            this.comboRole.Name = "comboRole";
            this.comboRole.Size = new System.Drawing.Size(200, 25);
            this.comboRole.TabIndex = 4;
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CalendarFont = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpStartTime.CalendarForeColor = System.Drawing.SystemColors.Window;
            this.dtpStartTime.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.dtpStartTime.Location = new System.Drawing.Point(30, 133);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.Size = new System.Drawing.Size(200, 20);
            this.dtpStartTime.TabIndex = 5;
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.CalendarFont = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpEndTime.CalendarForeColor = System.Drawing.SystemColors.Window;
            this.dtpEndTime.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.dtpEndTime.Location = new System.Drawing.Point(275, 133);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.Size = new System.Drawing.Size(200, 20);
            this.dtpEndTime.TabIndex = 6;
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnNext.ForeColor = System.Drawing.Color.Aqua;
            this.btnNext.Location = new System.Drawing.Point(272, 510);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 35);
            this.btnNext.TabIndex = 7;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // numStartTolerance
            // 
            this.numStartTolerance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.numStartTolerance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numStartTolerance.ForeColor = System.Drawing.Color.White;
            this.numStartTolerance.Location = new System.Drawing.Point(30, 285);
            this.numStartTolerance.Name = "numStartTolerance";
            this.numStartTolerance.Size = new System.Drawing.Size(200, 25);
            this.numStartTolerance.TabIndex = 8;
            // 
            // labelStartTolerance
            // 
            this.labelStartTolerance.AutoSize = true;
            this.labelStartTolerance.BackColor = System.Drawing.Color.Transparent;
            this.labelStartTolerance.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelStartTolerance.ForeColor = System.Drawing.Color.LightGray;
            this.labelStartTolerance.Location = new System.Drawing.Point(23, 252);
            this.labelStartTolerance.Name = "labelStartTolerance";
            this.labelStartTolerance.Size = new System.Drawing.Size(169, 25);
            this.labelStartTolerance.TabIndex = 9;
            this.labelStartTolerance.Text = "Start Time Tolerance";
            // 
            // labelEndTolerance
            // 
            this.labelEndTolerance.AutoSize = true;
            this.labelEndTolerance.BackColor = System.Drawing.Color.Transparent;
            this.labelEndTolerance.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelEndTolerance.ForeColor = System.Drawing.Color.LightGray;
            this.labelEndTolerance.Location = new System.Drawing.Point(268, 252);
            this.labelEndTolerance.Name = "labelEndTolerance";
            this.labelEndTolerance.Size = new System.Drawing.Size(163, 25);
            this.labelEndTolerance.TabIndex = 10;
            this.labelEndTolerance.Text = "End Time Tolerance";
            // 
            // numEndTolerance
            // 
            this.numEndTolerance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.numEndTolerance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numEndTolerance.ForeColor = System.Drawing.Color.White;
            this.numEndTolerance.Location = new System.Drawing.Point(275, 285);
            this.numEndTolerance.Name = "numEndTolerance";
            this.numEndTolerance.Size = new System.Drawing.Size(200, 25);
            this.numEndTolerance.TabIndex = 11;
            // 
            // userName
            // 
            this.userName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.userName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.userName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.userName.ForeColor = System.Drawing.Color.White;
            this.userName.Location = new System.Drawing.Point(275, 55);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(200, 25);
            this.userName.TabIndex = 12;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Aqua;
            this.btnCancel.Location = new System.Drawing.Point(150, 510);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 35);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.cancelButton_Click_1);
            // 
            // labelBreakStartTime
            // 
            this.labelBreakStartTime.AutoSize = true;
            this.labelBreakStartTime.BackColor = System.Drawing.Color.Transparent;
            this.labelBreakStartTime.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelBreakStartTime.ForeColor = System.Drawing.Color.LightGray;
            this.labelBreakStartTime.Location = new System.Drawing.Point(24, 174);
            this.labelBreakStartTime.Name = "labelBreakStartTime";
            this.labelBreakStartTime.Size = new System.Drawing.Size(139, 25);
            this.labelBreakStartTime.TabIndex = 15;
            this.labelBreakStartTime.Text = "Break Start Time";
            this.labelBreakStartTime.Click += new System.EventHandler(this.breakStartTime_Click);
            // 
            // labelBreakEndTime
            // 
            this.labelBreakEndTime.AutoSize = true;
            this.labelBreakEndTime.BackColor = System.Drawing.Color.Transparent;
            this.labelBreakEndTime.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelBreakEndTime.ForeColor = System.Drawing.Color.LightGray;
            this.labelBreakEndTime.Location = new System.Drawing.Point(270, 171);
            this.labelBreakEndTime.Name = "labelBreakEndTime";
            this.labelBreakEndTime.Size = new System.Drawing.Size(123, 25);
            this.labelBreakEndTime.TabIndex = 16;
            this.labelBreakEndTime.Text = "BreakEndTime";
            // 
            // breakStartTime
            // 
            this.breakStartTime.CalendarFont = new System.Drawing.Font("Segoe UI", 10F);
            this.breakStartTime.CalendarForeColor = System.Drawing.SystemColors.Window;
            this.breakStartTime.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.breakStartTime.Location = new System.Drawing.Point(30, 209);
            this.breakStartTime.Name = "breakStartTime";
            this.breakStartTime.Size = new System.Drawing.Size(200, 20);
            this.breakStartTime.TabIndex = 17;
            // 
            // breakEndTime
            // 
            this.breakEndTime.CalendarFont = new System.Drawing.Font("Segoe UI", 10F);
            this.breakEndTime.CalendarForeColor = System.Drawing.SystemColors.Window;
            this.breakEndTime.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.breakEndTime.Location = new System.Drawing.Point(275, 209);
            this.breakEndTime.Name = "breakEndTime";
            this.breakEndTime.Size = new System.Drawing.Size(200, 20);
            this.breakEndTime.TabIndex = 18;
            // 
            // recipientEmailLabel
            // 
            this.recipientEmailLabel.AutoSize = true;
            this.recipientEmailLabel.BackColor = System.Drawing.Color.Transparent;
            this.recipientEmailLabel.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.recipientEmailLabel.ForeColor = System.Drawing.Color.LightGray;
            this.recipientEmailLabel.Location = new System.Drawing.Point(24, 333);
            this.recipientEmailLabel.Name = "recipientEmailLabel";
            this.recipientEmailLabel.Size = new System.Drawing.Size(130, 25);
            this.recipientEmailLabel.TabIndex = 19;
            this.recipientEmailLabel.Text = "Recipient Email";
            this.recipientEmailLabel.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // senderEmailLabel
            // 
            this.senderEmailLabel.AutoSize = true;
            this.senderEmailLabel.BackColor = System.Drawing.Color.Transparent;
            this.senderEmailLabel.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.senderEmailLabel.ForeColor = System.Drawing.Color.LightGray;
            this.senderEmailLabel.Location = new System.Drawing.Point(270, 331);
            this.senderEmailLabel.Name = "senderEmailLabel";
            this.senderEmailLabel.Size = new System.Drawing.Size(114, 25);
            this.senderEmailLabel.TabIndex = 20;
            this.senderEmailLabel.Text = "Sender Email";
            // 
            // smtpServerLabel
            // 
            this.smtpServerLabel.AutoSize = true;
            this.smtpServerLabel.BackColor = System.Drawing.Color.Transparent;
            this.smtpServerLabel.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.smtpServerLabel.ForeColor = System.Drawing.Color.LightGray;
            this.smtpServerLabel.Location = new System.Drawing.Point(26, 408);
            this.smtpServerLabel.Name = "smtpServerLabel";
            this.smtpServerLabel.Size = new System.Drawing.Size(111, 25);
            this.smtpServerLabel.TabIndex = 21;
            this.smtpServerLabel.Text = "SMTP Server";
            // 
            // recipientEmail
            // 
            this.recipientEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.recipientEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.recipientEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.recipientEmail.ForeColor = System.Drawing.Color.White;
            this.recipientEmail.Location = new System.Drawing.Point(30, 369);
            this.recipientEmail.Name = "recipientEmail";
            this.recipientEmail.Size = new System.Drawing.Size(200, 25);
            this.recipientEmail.TabIndex = 22;
            // 
            // senderEmail
            // 
            this.senderEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.senderEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.senderEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.senderEmail.ForeColor = System.Drawing.Color.White;
            this.senderEmail.Location = new System.Drawing.Point(275, 369);
            this.senderEmail.Name = "senderEmail";
            this.senderEmail.Size = new System.Drawing.Size(200, 25);
            this.senderEmail.TabIndex = 23;
            // 
            // smtpServer
            // 
            this.smtpServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.smtpServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.smtpServer.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.smtpServer.ForeColor = System.Drawing.Color.White;
            this.smtpServer.Location = new System.Drawing.Point(31, 442);
            this.smtpServer.Name = "smtpServer";
            this.smtpServer.Size = new System.Drawing.Size(199, 25);
            this.smtpServer.TabIndex = 24;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(28)))), ((int)(((byte)(38)))));
            this.panel1.Controls.Add(this.pictureBox9);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(523, 59);
            this.panel1.TabIndex = 25;
            // 
            // pictureBox9
            // 
            this.pictureBox9.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox9.Image = global::WOTTracker.Properties.Resources.yazaki;
            this.pictureBox9.Location = new System.Drawing.Point(44, 9);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(80, 41);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox9.TabIndex = 6;
            this.pictureBox9.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::WOTTracker.Properties.Resources.looogo;
            this.pictureBox1.Location = new System.Drawing.Point(14, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 42);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(169, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Initial Application Setup";
            // 
            // pnlNotifications
            // 
            this.pnlNotifications.Controls.Add(this.txtNotificationBody);
            this.pnlNotifications.Controls.Add(this.label5);
            this.pnlNotifications.Controls.Add(this.label4);
            this.pnlNotifications.Controls.Add(this.txtNotificationSubject);
            this.pnlNotifications.Controls.Add(this.label3);
            this.pnlNotifications.Controls.Add(this.txtRecipientEmail);
            this.pnlNotifications.Controls.Add(this.dtpNotificationTime);
            this.pnlNotifications.Controls.Add(this.txtNotificationRecipient);
            this.pnlNotifications.Controls.Add(this.btnBack);
            this.pnlNotifications.Controls.Add(this.btnSave);
            this.pnlNotifications.Location = new System.Drawing.Point(0, 63);
            this.pnlNotifications.Name = "pnlNotifications";
            this.pnlNotifications.Size = new System.Drawing.Size(496, 573);
            this.pnlNotifications.TabIndex = 26;
            this.pnlNotifications.Visible = false;
            // 
            // txtNotificationBody
            // 
            this.txtNotificationBody.AcceptsReturn = true;
            this.txtNotificationBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.txtNotificationBody.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNotificationBody.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNotificationBody.ForeColor = System.Drawing.SystemColors.Window;
            this.txtNotificationBody.Location = new System.Drawing.Point(27, 254);
            this.txtNotificationBody.Multiline = true;
            this.txtNotificationBody.Name = "txtNotificationBody";
            this.txtNotificationBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotificationBody.Size = new System.Drawing.Size(444, 250);
            this.txtNotificationBody.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label5.ForeColor = System.Drawing.Color.LightGray;
            this.label5.Location = new System.Drawing.Point(23, 215);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(155, 21);
            this.label5.TabIndex = 19;
            this.label5.Text = "Notification mail Text";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label4.ForeColor = System.Drawing.Color.LightGray;
            this.label4.Location = new System.Drawing.Point(23, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(180, 21);
            this.label4.TabIndex = 18;
            this.label4.Text = "Notification mail Subject";
            // 
            // txtNotificationSubject
            // 
            this.txtNotificationSubject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.txtNotificationSubject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNotificationSubject.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNotificationSubject.ForeColor = System.Drawing.Color.White;
            this.txtNotificationSubject.Location = new System.Drawing.Point(27, 176);
            this.txtNotificationSubject.Name = "txtNotificationSubject";
            this.txtNotificationSubject.Size = new System.Drawing.Size(200, 25);
            this.txtNotificationSubject.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label3.ForeColor = System.Drawing.Color.LightGray;
            this.label3.Location = new System.Drawing.Point(267, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 21);
            this.label3.TabIndex = 16;
            this.label3.Text = "Notification Send Time";
            // 
            // txtRecipientEmail
            // 
            this.txtRecipientEmail.AutoSize = true;
            this.txtRecipientEmail.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtRecipientEmail.ForeColor = System.Drawing.Color.LightGray;
            this.txtRecipientEmail.Location = new System.Drawing.Point(23, 62);
            this.txtRecipientEmail.Name = "txtRecipientEmail";
            this.txtRecipientEmail.Size = new System.Drawing.Size(201, 21);
            this.txtRecipientEmail.TabIndex = 15;
            this.txtRecipientEmail.Text = "Notification Recipient Email";
            this.txtRecipientEmail.Click += new System.EventHandler(this.label2_Click);
            // 
            // dtpNotificationTime
            // 
            this.dtpNotificationTime.CalendarFont = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpNotificationTime.CalendarForeColor = System.Drawing.SystemColors.Window;
            this.dtpNotificationTime.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.dtpNotificationTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpNotificationTime.Location = new System.Drawing.Point(271, 108);
            this.dtpNotificationTime.Name = "dtpNotificationTime";
            this.dtpNotificationTime.ShowUpDown = true;
            this.dtpNotificationTime.Size = new System.Drawing.Size(200, 20);
            this.dtpNotificationTime.TabIndex = 14;
            // 
            // txtNotificationRecipient
            // 
            this.txtNotificationRecipient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.txtNotificationRecipient.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNotificationRecipient.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNotificationRecipient.ForeColor = System.Drawing.Color.White;
            this.txtNotificationRecipient.Location = new System.Drawing.Point(27, 103);
            this.txtNotificationRecipient.Name = "txtNotificationRecipient";
            this.txtNotificationRecipient.Size = new System.Drawing.Size(200, 25);
            this.txtNotificationRecipient.TabIndex = 13;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.Aqua;
            this.btnBack.Location = new System.Drawing.Point(133, 525);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 35);
            this.btnBack.TabIndex = 9;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Visible = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click_1);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.Aqua;
            this.btnSave.Location = new System.Drawing.Point(284, 525);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 35);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // pnlCoreConfig
            // 
            this.pnlCoreConfig.Controls.Add(this.labelRole);
            this.pnlCoreConfig.Controls.Add(this.labelStartTime);
            this.pnlCoreConfig.Controls.Add(this.comboRole);
            this.pnlCoreConfig.Controls.Add(this.senderEmail);
            this.pnlCoreConfig.Controls.Add(this.smtpServer);
            this.pnlCoreConfig.Controls.Add(this.senderEmailLabel);
            this.pnlCoreConfig.Controls.Add(this.dtpStartTime);
            this.pnlCoreConfig.Controls.Add(this.breakEndTime);
            this.pnlCoreConfig.Controls.Add(this.numStartTolerance);
            this.pnlCoreConfig.Controls.Add(this.labelBreakEndTime);
            this.pnlCoreConfig.Controls.Add(this.recipientEmail);
            this.pnlCoreConfig.Controls.Add(this.userName);
            this.pnlCoreConfig.Controls.Add(this.labelStartTolerance);
            this.pnlCoreConfig.Controls.Add(this.numEndTolerance);
            this.pnlCoreConfig.Controls.Add(this.smtpServerLabel);
            this.pnlCoreConfig.Controls.Add(this.labelEndTolerance);
            this.pnlCoreConfig.Controls.Add(this.btnCancel);
            this.pnlCoreConfig.Controls.Add(this.btnNext);
            this.pnlCoreConfig.Controls.Add(this.labelBreakStartTime);
            this.pnlCoreConfig.Controls.Add(this.dtpEndTime);
            this.pnlCoreConfig.Controls.Add(this.recipientEmailLabel);
            this.pnlCoreConfig.Controls.Add(this.labelEndTime);
            this.pnlCoreConfig.Controls.Add(this.breakStartTime);
            this.pnlCoreConfig.Controls.Add(this.labelUserName);
            this.pnlCoreConfig.Location = new System.Drawing.Point(0, 61);
            this.pnlCoreConfig.Name = "pnlCoreConfig";
            this.pnlCoreConfig.Size = new System.Drawing.Size(488, 570);
            this.pnlCoreConfig.TabIndex = 27;
            // 
            // labelRole
            // 
            this.labelRole.AutoSize = true;
            this.labelRole.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.labelRole.ForeColor = System.Drawing.Color.LightGray;
            this.labelRole.Location = new System.Drawing.Point(26, 21);
            this.labelRole.Name = "labelRole";
            this.labelRole.Size = new System.Drawing.Size(86, 25);
            this.labelRole.TabIndex = 27;
            this.labelRole.Text = "User Role";
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(500, 649);
            this.Controls.Add(this.pnlNotifications);
            this.Controls.Add(this.pnlCoreConfig);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetupForm";
            this.Text = "SetupForm";
            this.Load += new System.EventHandler(this.SetupForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numStartTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEndTolerance)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlNotifications.ResumeLayout(false);
            this.pnlNotifications.PerformLayout();
            this.pnlCoreConfig.ResumeLayout(false);
            this.pnlCoreConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Label labelStartTime;
        private System.Windows.Forms.Label labelEndTime;
        private System.Windows.Forms.ComboBox comboRole;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.NumericUpDown numStartTolerance;
        private System.Windows.Forms.Label labelStartTolerance;
        private System.Windows.Forms.Label labelEndTolerance;
        private System.Windows.Forms.NumericUpDown numEndTolerance;
        private System.Windows.Forms.TextBox userName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelBreakStartTime;
        private System.Windows.Forms.Label labelBreakEndTime;
        private System.Windows.Forms.DateTimePicker breakStartTime;
        private System.Windows.Forms.DateTimePicker breakEndTime;
        private System.Windows.Forms.Label recipientEmailLabel;
        private System.Windows.Forms.Label senderEmailLabel;
        private System.Windows.Forms.Label smtpServerLabel;
        private System.Windows.Forms.TextBox recipientEmail;
        private System.Windows.Forms.TextBox senderEmail;
        private System.Windows.Forms.TextBox smtpServer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlNotifications;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label txtRecipientEmail;
        private System.Windows.Forms.DateTimePicker dtpNotificationTime;
        private System.Windows.Forms.TextBox txtNotificationRecipient;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNotificationSubject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNotificationBody;
        private System.Windows.Forms.Panel pnlCoreConfig;
        private System.Windows.Forms.Label labelRole;
    }
}