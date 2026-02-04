// In Forms/DashboardForm.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WOTTracker.Forms
{
    public partial class DashboardForm : Form
    {
        // Data passed from MainForm
        private readonly Dictionary<DayOfWeek, double> _overtimeByDay;
        private readonly DataTable _longestSessions;
        private readonly Dictionary<string, double> _weeklyOvertime;

        public DashboardForm(Dictionary<DayOfWeek, double> overtimeByDay, DataTable longestSessions, Dictionary<string, double> weeklyOvertime)
        {
            InitializeComponent();

            _overtimeByDay = overtimeByDay;
            _longestSessions = longestSessions;
            _weeklyOvertime = weeklyOvertime;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyStyling();
            PopulateCharts();
            // Position the form
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(workingArea.Right - this.Width - 461, workingArea.Bottom - this.Height);
        }
       

        private void ApplyStyling()
        {
            // Apply your dark theme to the form and controls
            this.BackColor = Color.FromArgb(33, 38, 45);
            // ... Style your charts (background, labels, etc.) ...
            var charts = new[] { chartOvertimeDays, chartLongestSessions, chartBurnoutRisk };
            foreach (var chart in charts)
            {
                chart.BackColor = Color.FromArgb(45, 50, 54);
                chart.ChartAreas[0].BackColor = Color.Transparent;
                chart.Legends[0].BackColor = Color.Transparent;
                chart.Legends[0].ForeColor = Color.White;
                chart.ChartAreas[0].AxisX.LineColor = Color.LightGray;
                chart.ChartAreas[0].AxisY.LineColor = Color.LightGray;
                chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;
                chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
                chart.Titles.Add("Title").ForeColor = Color.White;
            }
        }

        private void PopulateCharts()
        {
            PopulateOvertimeDaysChart();
            PopulateLongestSessionsChart();
            PopulateBurnoutRiskChart();
        }

        private void PopulateOvertimeDaysChart()
        {
            chartOvertimeDays.Series.Clear();
            chartOvertimeDays.Titles[0].Text = "Overtime Distribution by Day of Week";

            var series = new Series("Overtime (hrs)")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.DodgerBlue
            };

            // Order: Monday to Sunday
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                double minutes = _overtimeByDay.ContainsKey(day) ? _overtimeByDay[day] : 0;
                double hours = Math.Round(minutes / 60, 2);
                series.Points.AddXY(day.ToString(), hours);
            }

            chartOvertimeDays.Series.Add(series);
            chartOvertimeDays.ChartAreas[0].AxisX.Title = "Day of Week";
            chartOvertimeDays.ChartAreas[0].AxisY.Title = "Overtime Hours";
        }


        private void PopulateLongestSessionsChart()
        {
            chartLongestSessions.Series.Clear();
            chartLongestSessions.Titles[0].Text = "Top 10 Longest Sessions";

            var series = new Series("Duration (min)")
            {
                ChartType = SeriesChartType.Bar,
                Color = Color.MediumPurple
            };

            foreach (DataRow row in _longestSessions.Rows)
            {
                DateTime start = (DateTime)row["StartTime"];
                double minutes = (double)row["DurationMinutes"];
                string label = start.ToString("MMM dd - HH:mm");

                var point = new DataPoint();
                point.SetValueXY(label, minutes);
                point.Label = $"{minutes} min";

                series.Points.Add(point);
            }

            chartLongestSessions.Series.Add(series);
            chartLongestSessions.ChartAreas[0].AxisX.Title = "Session Start Time";
            chartLongestSessions.ChartAreas[0].AxisY.Title = "Duration (minutes)";
        }


        private void PopulateBurnoutRiskChart()
        {
            chartBurnoutRisk.Series.Clear();
            chartBurnoutRisk.Titles[0].Text = "Burnout Risk (Last 12 Weeks)";

            var series = new Series("Overtime (hrs)")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = Color.LimeGreen
            };

            var recentWeeks = _weeklyOvertime.Skip(Math.Max(0, _weeklyOvertime.Count - 12)).ToList();

            List<bool> burnoutFlags = new List<bool>();
            int index = 0;
            foreach (var week in recentWeeks)
            {
                double hours = Math.Round(week.Value / 60, 2);
                var point = new DataPoint(index++, hours)
                {
                    AxisLabel = week.Key
                };

                if (hours > 10)
                {
                    burnoutFlags.Add(true);
                    point.Color = Color.OrangeRed;
                }
                else
                {
                    burnoutFlags.Add(false);
                    point.Color = Color.SteelBlue;
                }

                series.Points.Add(point);
            }

            // Check for 3 consecutive burnout weeks
            for (int i = 0; i < burnoutFlags.Count - 2; i++)
            {
                if (burnoutFlags[i] && burnoutFlags[i + 1] && burnoutFlags[i + 2])
                {
                    chartBurnoutRisk.Titles[0].Text += " - ⚠️  Burnout Detected!";
                    chartBurnoutRisk.Titles[0].ForeColor = Color.Red;
                    break;
                }
            }

            chartBurnoutRisk.Series.Add(series);
            chartBurnoutRisk.ChartAreas[0].AxisX.Title = "Week";
            chartBurnoutRisk.ChartAreas[0].AxisY.Title = "Overtime Hours";
        }



        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}