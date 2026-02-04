namespace WOTTracker.Forms
{
    partial class DashboardForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardForm));
            this.chartOvertimeDays = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartLongestSessions = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartBurnoutRisk = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartOvertimeDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartLongestSessions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartBurnoutRisk)).BeginInit();
            this.SuspendLayout();
            // 
            // chartOvertimeDays
            // 
            chartArea1.Name = "ChartArea1";
            this.chartOvertimeDays.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartOvertimeDays.Legends.Add(legend1);
            this.chartOvertimeDays.Location = new System.Drawing.Point(37, 18);
            this.chartOvertimeDays.Name = "chartOvertimeDays";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartOvertimeDays.Series.Add(series1);
            this.chartOvertimeDays.Size = new System.Drawing.Size(371, 322);
            this.chartOvertimeDays.TabIndex = 0;
            this.chartOvertimeDays.Text = "chart1";
            // 
            // chartLongestSessions
            // 
            chartArea2.Name = "ChartArea1";
            this.chartLongestSessions.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartLongestSessions.Legends.Add(legend2);
            this.chartLongestSessions.Location = new System.Drawing.Point(414, 18);
            this.chartLongestSessions.Name = "chartLongestSessions";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartLongestSessions.Series.Add(series2);
            this.chartLongestSessions.Size = new System.Drawing.Size(347, 322);
            this.chartLongestSessions.TabIndex = 1;
            this.chartLongestSessions.Text = "chart1";
            // 
            // chartBurnoutRisk
            // 
            chartArea3.Name = "ChartArea1";
            this.chartBurnoutRisk.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartBurnoutRisk.Legends.Add(legend3);
            this.chartBurnoutRisk.Location = new System.Drawing.Point(208, 348);
            this.chartBurnoutRisk.Name = "chartBurnoutRisk";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartBurnoutRisk.Series.Add(series3);
            this.chartBurnoutRisk.Size = new System.Drawing.Size(405, 268);
            this.chartBurnoutRisk.TabIndex = 2;
            this.chartBurnoutRisk.Text = "chart1";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(50)))), ((int)(((byte)(54)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.Aqua;
            this.btnClose.Location = new System.Drawing.Point(353, 656);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 33);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(800, 701);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chartBurnoutRisk);
            this.Controls.Add(this.chartLongestSessions);
            this.Controls.Add(this.chartOvertimeDays);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DashboardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DashboardForm";
            ((System.ComponentModel.ISupportInitialize)(this.chartOvertimeDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartLongestSessions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartBurnoutRisk)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartOvertimeDays;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartLongestSessions;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartBurnoutRisk;
        private System.Windows.Forms.Button btnClose;
    }
}