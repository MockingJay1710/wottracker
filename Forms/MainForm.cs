using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Serilog;
using WOTTracker.Configuration;
using WOTTracker.Data;
using WOTTracker.Forms;
using WOTTracker.Services;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading.Tasks;


namespace WOTTracker
{
    public partial class MainForm : Form
    {
        // --- Core State & Configuration ---
        private ConfigurationSet _activeConfig;
        private DateTime? _currentSessionStartTime;
        private System.Threading.Timer _uiUpdateTimer;


        // --- Bootstrap/Environmental Variables from appsettings.json ---
        private string _databasePath;


        // --- In-Memory State Cache for Real-Time UI Performance ---
        // This dictionary will hold the historical overtime totals for each category.
        private Dictionary<string, double> _historicalOvertimeTotals = new Dictionary<string, double>();
        // this will hold the total unresolved downtime minutes for the current session.
        private double _totalUnresolvedDowntimeMinutes = 0;

        // --- UI and System Tray Components ---
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        private ToolStripMenuItem _exportMenuItem;
        private ToolStripMenuItem _reconfigureMenuItem;
        private ToolStripMenuItem _clearDatabaseMenuItem;

        // --- NEW: Variable to hold the scheduled email time ---
        private DateTime? _notificationSendTime = null;
        // --- Add a variable for our new service ---
        private readonly EmailService _emailService;
        // --- State variables for notification scheduling ---
        private bool _isHomeOfficeEmailScheduledToday = false;
        private DateTime _lastKnownDate = DateTime.MinValue;



        public MainForm(ConfigurationSet activeConfig)
        {

            Log.Information("Application starting up.");
            // --- Phase 1: Bootstrap & Load All Config ---
            _activeConfig = activeConfig;
            _databasePath = ConfigurationManager._databasePath;
            // --- Instantiate the service in the constructor ---
            _emailService = new EmailService(_activeConfig);
            ConfigureLogging();


            InitializeComponent();

            

            // --- Phase 2: Initialize UI and System Components ---
            InitializeTrayIcon();
            StartupManager.SetStartup(true);

            

            // --- Phase 3: Run Recovery and Start Live Tracking ---
            RunGrandReconciliation();

            // --- Phase 4: resolve downtime ---

            RunDowntimeResolveProcess(); // This method will now handle the initial downtime resolution.



            InitializeLiveSessionState(); // Cache historical data for UI performance
            SystemEvents.PowerModeChanged += OnPowerModeChanged;

            // --- Phase 4: Start Real-Time UI Updates ---
            // The callback method (UpdateLiveDisplay) will be executed on a background thread.
            // The state object (null) is for passing data, which we don't need here.
            // dueTime: 0 means start immediately.
            // period: 1000 means repeat every 1000ms (1 second).
            _uiUpdateTimer = new System.Threading.Timer(
                callback: _ => UpdateLiveDisplay(),
                state: null,
                dueTime: 1000,
                period: 1000);

            // Subscribe to Form events
            this.FormClosing += MainForm_FormClosing;
            this.Resize += MainForm_Resize;

            this.TopMost = true;
            System.Drawing.Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new System.Drawing.Point(workingArea.Right - this.Width, workingArea.Bottom - this.Height);
        }




        /// <summary>
        /// Resolve downtime for special reason (Conge, Permission).
        /// </summary>
        /// 

        public void ResolveSpecialDowntime(Downtime downtime, string reason)
        {
            if (downtime == null) return;
            Log.Information("Compensating downtime from {Start} to {End} ({Duration} mins) for reason: {Reason}",
                downtime.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), downtime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), Math.Round(downtime.DurationMinutes, 2), reason);

            using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                connection.Open();
                try
                {
                    using (var cmd = new SqlCeCommand("INSERT INTO Compensations (DowntimeID, ActivitySessionID, CompensatedMinutes, Reason) VALUES (@DowntimeID, @ActivitySessionID, @CompensatedMinutes, @Reason)", connection))
                    {
                        cmd.Parameters.AddWithValue("@DowntimeID", downtime.ID);
                        cmd.Parameters.AddWithValue("@ActivitySessionID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@CompensatedMinutes", downtime.DurationMinutes);
                        cmd.Parameters.AddWithValue("@Reason", reason); //"Permission", "Conge"
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error: Could not insert compensation record.\n\n{ex.Message}");
                }

            }
        }

        /// <summary>
        /// Compensate dowtime from an overtime.
        /// </summary>
        ///


        /// <summary>
        /// Applies available overtime sources to a day's downtime records and RETURNS the total amount compensated.
        /// This method mutates the state of the passed-in overtimeSources list.
        /// </summary>
        /// <param name="daySummary">The summary of the day's downtime to be resolved.</param>
        /// <param name="overtimeSources">The list of available overtime credits. This list WILL BE MODIFIED.</param>
        /// <returns>The total number of minutes successfully compensated in this run.</returns>
        private double CompensateDowntimeWithOvertime(DowntimeDaySummary daySummary, List<OvertimeSourceSummary> overtimeSources)
        {
            Log.Information("Attempting to compensate downtime for {Date} with {overtimeCount} overtime sources.",
                daySummary.Date.ToShortDateString(), overtimeSources.Count);

            // This variable will track the total amount we successfully compensate in this transaction.
            double totalCompensatedInThisRun = 0;

            // Use a single transaction for the entire compensation process for this day.
            using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // We use the DowntimeRecords from the summary, not the whole object.
                        foreach (var downtimeRecord in daySummary.DowntimeRecords)
                        {
                            // Check how much is still owed for this specific downtime record before this run.
                            double stillOwedOnThisDowntime = downtimeRecord.DurationMinutes - GetCompensatedAmount(downtimeRecord.ID, connection, transaction);
                            if (stillOwedOnThisDowntime <= 0) continue;

                            // Loop through available overtime sources to pay the bill.
                            foreach (var overtimeSource in overtimeSources)
                            {
                                if (overtimeSource.AvailableOvertimeMinutes <= 0) continue;

                                double paymentAmount = Math.Min(stillOwedOnThisDowntime, overtimeSource.AvailableOvertimeMinutes);

                                if (paymentAmount > 0.1) // Use a small threshold for floating point math
                                {
                                    // Create the linking record in the Compensations table.
                                    using (var cmd = new SqlCeCommand("INSERT INTO Compensations (DowntimeID, ActivitySessionID, CompensatedMinutes, Reason) VALUES (@DowntimeID, @ActivitySessionID, @CompensatedMinutes, 'Compensation')", connection, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@DowntimeID", downtimeRecord.ID);
                                        cmd.Parameters.AddWithValue("@ActivitySessionID", overtimeSource.ActivitySessionID);
                                        cmd.Parameters.AddWithValue("@CompensatedMinutes", paymentAmount);
                                        cmd.ExecuteNonQuery();
                                    }
                                    Log.Information("Compensated {amount} mins for DowntimeID {dID} using OvertimeID {oID}",
                                        Math.Round(paymentAmount, 2), downtimeRecord.ID, overtimeSource.ActivitySessionID);

                                    // --- STATE MUTATION ---
                                    // Update the in-memory state of our objects to reflect the transaction.
                                    stillOwedOnThisDowntime -= paymentAmount;
                                    overtimeSource.AvailableOvertimeMinutes -= paymentAmount;

                                    // Add the amount paid to our running total for this method call.
                                    totalCompensatedInThisRun += paymentAmount;
                                }

                                if (stillOwedOnThisDowntime <= 0) break; // This downtime is fully paid, move to the next one.
                            }
                        }

                        transaction.Commit();
                        Log.Information("Compensation transaction for {Date} committed successfully.", daySummary.Date.ToShortDateString());
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log.Error(ex, "Compensation transaction failed. All changes were rolled back.");
                        return 0; // If the transaction fails, nothing was compensated.
                    }
                }
            }

            // --- RETURN THE TOTAL AMOUNT PAID ---
            return totalCompensatedInThisRun;
        }

        private double GetCompensatedAmount(int downtimeId, SqlCeConnection connection, SqlCeTransaction transaction)
        {
            using (var cmd = new SqlCeCommand("SELECT SUM(CompensatedMinutes) FROM Compensations WHERE DowntimeID = @ID", connection, transaction))
            {
                cmd.Parameters.AddWithValue("@ID", downtimeId);
                if (cmd.ExecuteScalar() is double total) return total;
            }
            return 0;
        }

        private List<DowntimeDaySummary> GetUnresolvedDowntimeSummaries()
        {
            var dailyDowntime = new Dictionary<DateTime, DowntimeDaySummary>();
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                connection.Open();

                string query = @"
            SELECT d.ID, d.StartTime, d.EndTime, 
                   d.DurationMinutes - COALESCE(c.TotalCompensated, 0) AS DurationMinutes
            FROM Downtime d
            LEFT JOIN (
                SELECT DowntimeID, SUM(CompensatedMinutes) as TotalCompensated
                FROM Compensations
                GROUP BY DowntimeID
            ) c ON d.ID = c.DowntimeID
            WHERE d.DurationMinutes > COALESCE(c.TotalCompensated, 0)";

                using (var cmd = new SqlCeCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var downtimeRecord = new Downtime
                        {
                            ID = (int)reader["ID"],
                            StartTime = (DateTime)reader["StartTime"],
                            EndTime = (DateTime)reader["EndTime"],
                            DurationMinutes = (double)reader["DurationMinutes"]
                        };

                        // Only keep records from the current month and year
                        if (downtimeRecord.StartTime.Month != currentMonth || downtimeRecord.StartTime.Year != currentYear)
                            continue;

                        DateTime dateKey = downtimeRecord.StartTime.Date;

                        if (!dailyDowntime.ContainsKey(dateKey))
                        {
                            dailyDowntime[dateKey] = new DowntimeDaySummary { Date = dateKey };
                        }

                        dailyDowntime[dateKey].DowntimeRecords.Add(downtimeRecord);
                        dailyDowntime[dateKey].TotalDowntimeMinutes += downtimeRecord.DurationMinutes;
                    }
                }
            }

            return dailyDowntime.Values.OrderBy(d => d.Date).ToList();
        }


        private List<OvertimeSourceSummary> GetAvailableOvertimeSummaries()
        {
            var overtimeSources = new List<OvertimeSourceSummary>();

            using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                connection.Open();

                string query = @"
        SELECT 
            a.ID,
            a.StartTime,
            a.OvertimeMinutes,
            COALESCE(c.TotalSpent, 0) AS TotalSpent
        FROM 
            ActivitySessions a
        LEFT JOIN (
            SELECT ActivitySessionID, SUM(CompensatedMinutes) as TotalSpent
            FROM Compensations
            GROUP BY ActivitySessionID
        ) c ON a.ID = c.ActivitySessionID
        WHERE 
            a.OvertimeMinutes > 0";

                using (var cmd = new SqlCeCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    DateTime now = DateTime.Now;
                    DateTime expirationThreshold = now.AddDays(-30); // or .AddMonths(-1) if you prefer

                    while (reader.Read())
                    {
                        DateTime startTime = ((DateTime)reader["StartTime"]).Date;

                        // Skip expired overtime (older than 30 days)
                        if (startTime < expirationThreshold)
                            continue;

                        double totalCredit = Math.Round((double)reader["OvertimeMinutes"], 2);
                        double totalSpent = Math.Round((double)reader["TotalSpent"], 2);
                        double availableCredit = totalCredit - totalSpent;

                        int availableMinutes = (int)Math.Floor(availableCredit);

                        if (availableMinutes > 0)
                        {
                            overtimeSources.Add(new OvertimeSourceSummary
                            {
                                ActivitySessionID = (int)reader["ID"],
                                Date = startTime,
                                AvailableOvertimeMinutes = availableMinutes
                            });
                        }
                    }
                }
            }

            return overtimeSources.OrderBy(s => s.Date).ToList();
        }





        

        

        public void RunDowntimeResolveProcess()
        {
            Log.Information("--- Starting Downtime Resolution Process ---");
            List<DowntimeDaySummary> unresolvedDays = GetUnresolvedDowntimeSummaries();
            if (!unresolvedDays.Any())
            {
                Log.Information("No unresolved downtime found. Process finished.");
                return;
            }

            List<OvertimeSourceSummary> availableOvertimeSources = GetAvailableOvertimeSummaries();
            string userRole = _activeConfig.UserRole;

            foreach (var daySummary in unresolvedDays)
            {
                double? amountPaidInLastAction = null; // Start with null
                bool isDayFullyResolved = false;
                while (!isDayFullyResolved)
                {
                    // Always get the most current overtime total before showing the form.
                    double availableOvertimeTotal = availableOvertimeSources.Sum(s => s.AvailableOvertimeMinutes);

                    Log.Information("--> Processing Day: {Date} | Remaining Downtime: {Minutes} mins | Available OT: {OT} mins",
                        daySummary.Date.ToShortDateString(),
                        Math.Round(daySummary.TotalDowntimeMinutes, 2),
                        Math.Round(availableOvertimeTotal));

                    // ... (logging for individual chunks) ...

                    using (var decisionForm = new DowntimeDecisionForm(daySummary, availableOvertimeTotal, userRole, amountPaidInLastAction))
                    {
                        DialogResult result = decisionForm.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            string reason = decisionForm.SelectedReason;
                            if (reason == "Compensation")
                            {
                                // --- THIS IS THE NEW LOGIC ---

                                // 1. Call the compensation method and get the amount that was actually paid.
                                double amountPaid = CompensateDowntimeWithOvertime(daySummary, availableOvertimeSources);

                                // 2. Update the in-memory state of the daySummary for the next loop iteration.
                                daySummary.TotalDowntimeMinutes -= amountPaid;
                                amountPaidInLastAction = amountPaid;

                                // 3. Check if the day is now fully resolved.
                                if (daySummary.TotalDowntimeMinutes <= 0.1) // Use a small threshold for float math
                                {
                                    isDayFullyResolved = true;
                                }
                            }
                            else if (reason == "Permission" || reason == "Congé")
                            {
                                
                                // The user chose a special reason. Resolve all downtime for that day.
                                foreach (var downtimeRecord in daySummary.DowntimeRecords)
                                {
                                    ResolveSpecialDowntime(downtimeRecord, reason);
                                }
                                isDayFullyResolved = true;
                            }
                            else if (reason == "Acknowledge")
                            {
                                Log.Information("User acknowledged remaining downtime for {Date}. Moving to next day.", daySummary.Date.ToShortDateString());
                                break; // Exit the inner 'while' loop.
                            }
                        }
                        else // User cancelled
                        {
                            Log.Warning("User cancelled the downtime resolution process.");
                            return; // Exit the entire method.
                        }
                    }
                }
            }
            Log.Information("--- Downtime Resolution Process Finished ---");
        }












        #region NEW - Database-Driven Session Management




        /// <summary>
        /// Shows the WorkLocationForm and returns the user's choice.
        /// </summary>
        /// <returns>"HomeOffice", "Office", or "Cancelled".</returns>
        private string PromptForWorkLocation()
        {

            using (var locationForm = new WorkLocationForm())
            {
                DialogResult result = locationForm.ShowDialog(this);
                if (result == DialogResult.Yes) return "HomeOffice";
                if (result == DialogResult.No) return "Office";
            }
            return "Cancelled"; // If user closes the form with 'X'
        }

        /// <summary>
        /// Creates a new, open-ended session record in the database, but ONLY if one doesn't already exist.
        /// It also sets the in-memory _currentSessionStartTime to begin live tracking.
        /// </summary>
        /// 
        private void CreateNewActiveSession(DateTime startTime)
        {
            
            try
            {
                using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
                {
                    conn.Open();

                    // --- NEW DUPLICATION CHECK ---
                    // First, check if there is already an open-ended session.
                    using (var checkCmd = new SqlCeCommand("SELECT COUNT(*) FROM ActivitySessions WHERE EndTime IS NULL", conn))
                    {
                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            Log.Warning("CreateNewActiveSession called, but an active session already exists. Aborting creation.");
                            // If an active session already exists, we should find it and set our
                            // in-memory start time to match it.
                            using (var getCmd = new SqlCeCommand("SELECT StartTime FROM ActivitySessions WHERE EndTime IS NULL", conn))
                            {
                                _currentSessionStartTime = (DateTime)getCmd.ExecuteScalar();
                            }
                            return; // Exit the method to prevent creating a duplicate.
                        }
                    }

                    string location = PromptForWorkLocation();
                    if (location == "HomeOffice")
                    {
                        ScheduleHomeOfficeEmail();
                    }

                    // --- If we get here, no active session exists, so we create one. ---
                    _currentSessionStartTime = startTime;
                    var newSession = new ActivitySession
                    {
                        ConfigurationID = _activeConfig.ID,
                        StartTime = _currentSessionStartTime.Value,
                        EndTime = null, // IMPORTANT: EndTime is NULL
                        DurationMinutes = 0,
                        SessionType = GetSessionType(startTime),
                        Notes = "Live",
                        WorkLocation = location
                    };

                    using (var cmd = new SqlCeCommand("INSERT INTO ActivitySessions (ConfigurationID, StartTime, EndTime, DurationMinutes, SessionType, Notes, WorkLocation) VALUES (@ConfigID, @Start, NULL, 0, @Type, @Notes, @WorkLocation)", conn))
                    {
                        cmd.Parameters.AddWithValue("@ConfigID", newSession.ConfigurationID);
                        cmd.Parameters.AddWithValue("@Start", newSession.StartTime);
                        cmd.Parameters.AddWithValue("@Type", newSession.SessionType);
                        cmd.Parameters.AddWithValue("@Notes", newSession.Notes);
                        cmd.Parameters.AddWithValue("@WorkLocation", newSession.WorkLocation);
                        cmd.ExecuteNonQuery();
                    }
                    Log.Information("New live session created in database starting at {StartTime}", newSession.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create or verify new active session record.");
                _currentSessionStartTime = null;
            }
        }
        #endregion









        #region Startup and Configuration Loading


        private void ConfigureLogging()
        {
            // Get the log file path from your configuration.
            string logPath = _activeConfig.LogFilePath;

            if (string.IsNullOrEmpty(logPath))
            {
                // Fallback in case the path isn't configured, though it should be.
                logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wot_tracker_log.txt");
                MessageBox.Show($"LogFilePath not found in configuration. Logging to: {logPath}");
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // It's good to set a minimum level
                .WriteTo.Console()
                .WriteTo.File(
                    path: logPath, // The single file to write to

                    // This prevents the single file from growing infinitely.
                    // It will create log.txt, then log_001.txt, log_002.txt, etc., as backups.
                    rollOnFileSizeLimit: true, // Start a new file when the size limit is reached
                    fileSizeLimitBytes: 10 * 1024 * 1024, // Set a limit, e.g., 10 MB
                    retainedFileCountLimit: 5, // Keep the main log file and the last 5 backups

                    // A good output template for debugging
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            Log.Information("--- Logging Initialized ---");
        }

        #endregion

        #region Core Tracking, Recovery, and Session Management
        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                Log.Information("System is suspending. Closing active session.");
                CloseActiveSession();
            }
            else if (e.Mode == PowerModes.Resume)
            {
                Log.Information("System is resuming. Creating new active session.");
                // When we wake up, the old session is closed, so we create a new one.
                CreateNewActiveSession(DateTime.Now);
                InitializeLiveSessionState();
            }
        }

        /// <summary>
        /// Finds the active session in the database (EndTime IS NULL) and updates it
        /// with an EndTime and final duration.
        /// </summary>
        private void CloseActiveSession()
        {
            // We get the start time from our in-memory variable, which was set on startup.
            if (!_currentSessionStartTime.HasValue) return;

            var sessionToClose = new ActivitySession
            {
                StartTime = _currentSessionStartTime.Value,
                EndTime = DateTime.Now,
                ConfigurationID = _activeConfig.ID,
                Notes = "Live"
            };

            _currentSessionStartTime = null; // Session is over.

            // The splitter logic is still essential.
            List<ActivitySession> chunks = SplitSessionByDay(sessionToClose);

            // --- TRANSACTION LOGIC STARTS HERE ---
            SqlCeConnection conn = null;
            SqlCeTransaction transaction = null;
            try
            {
                conn = new SqlCeConnection($"Data Source={_databasePath}");
                conn.Open();
                transaction = conn.BeginTransaction(); // Start the transaction

                // 1. DELETE the original open-ended session record, using the transaction.
                using (var cmd = new SqlCeCommand("DELETE FROM ActivitySessions WHERE EndTime IS NULL", conn, transaction))
                {
                    cmd.ExecuteNonQuery();
                }

                // 2. INSERT the new, finalized chunks, passing the connection and transaction.
                BatchLogSessions(chunks, conn, transaction);

                // 3. If everything was successful, commit all the changes.
                transaction.Commit();
                Log.Information("Successfully closed and committed live session.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to close active session. Rolling back transaction.");
                try
                {
                    // If anything failed, roll back all changes. The original open-ended
                    // session will be restored in the database.
                    transaction?.Rollback();
                }
                catch (Exception rbEx)
                {
                    Log.Error(rbEx, "Failed to rollback transaction.");
                }
            }
            finally
            {
                // Always ensure the connection is closed.
                conn?.Close();
            }
        }


        /// <summary>
        /// This is the central method for ending a session, splitting it by day,
        /// and committing it to the database.
        /// </summary>


        internal List<ActivitySession> SplitSessionByDay(ActivitySession session)
        {
            var splitSessions = new List<ActivitySession>();
            if (!session.EndTime.HasValue)
            {
                Log.Error("Attempted to split a session with no EndTime.");
                return splitSessions;
            }

            var endTime = session.EndTime.Value;
            var cursor = session.StartTime;

            while (cursor < endTime)
            {
                var endOfThisDay = cursor.Date.AddDays(1);
                var chunkEnd = (endTime < endOfThisDay) ? endTime : endOfThisDay;

                // --- NEW OVERTIME CALCULATION LOGIC IS HERE ---

                double overtimeForThisChunk = 0;
                string sessionType = GetSessionType(cursor);

                if (sessionType != "Normal")
                {
                    // If the day is a weekend or holiday, the entire duration of the chunk is raw overtime.
                    overtimeForThisChunk = (chunkEnd - cursor).TotalMinutes;
                }
                else // It's a normal workday, calculate based on work hours.
                {
                    // Get the work interval for the day of the chunk.
                    DateTime workStart = cursor.Date + _activeConfig.WorkStartTime;
                    DateTime workEnd = cursor.Date + _activeConfig.WorkEndTime;

                    // There are three parts to a chunk: before work, during work, and after work.
                    // Overtime is the sum of the parts before and after.

                    // 1. Calculate time worked BEFORE the official start time.
                    if (cursor < workStart)
                    {
                        // The amount of time is from the cursor up to the work start,
                        // but not exceeding the chunk's actual end time.
                        DateTime endOfEarlyPart = (chunkEnd < workStart) ? chunkEnd : workStart;
                        overtimeForThisChunk += (endOfEarlyPart - cursor).TotalMinutes;
                    }

                    // 2. Calculate time worked AFTER the official end time.
                    if (chunkEnd > workEnd)
                    {
                        // The start of this part is the work end time, unless the whole chunk
                        // started after the work end time.
                        DateTime startOfLatePart = (cursor > workEnd) ? cursor : workEnd;
                        overtimeForThisChunk += (startOfLatePart == workEnd && (chunkEnd - startOfLatePart).TotalMinutes <= _activeConfig.EndTimeToleranceMinutes) ? 0 : (chunkEnd - startOfLatePart).TotalMinutes;
                    }
                }

                var chunk = new ActivitySession
                {
                    ConfigurationID = session.ConfigurationID,
                    StartTime = cursor,
                    EndTime = chunkEnd,
                    DurationMinutes = (chunkEnd - cursor).TotalMinutes,
                    SessionType = sessionType,
                    OvertimeMinutes = overtimeForThisChunk, // Assign the calculated value
                    Notes = session.Notes
                };

                splitSessions.Add(chunk);
                cursor = chunkEnd;
            }

            return splitSessions;
        }


        private string GetSessionType(DateTime date)
        {
            if (IsPublicHoliday(date)) return "Public Holiday";
            if (date.DayOfWeek == DayOfWeek.Saturday) return "Saturday";
            if (date.DayOfWeek == DayOfWeek.Sunday) return "Sunday";
            return "Normal";
        }

        private bool IsPublicHoliday(DateTime date)
        {
            // This method now queries the database tables.
            using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                connection.Open();
                // Check fixed holidays
                using (var cmd = new SqlCeCommand("SELECT COUNT(*) FROM FixedPublicHolidays WHERE Month = @Month AND Day = @Day", connection))
                {
                    cmd.Parameters.AddWithValue("@Month", date.Month);
                    cmd.Parameters.AddWithValue("@Day", date.Day);
                    if ((int)cmd.ExecuteScalar() > 0) return true;
                }
                // Check movable holidays
                using (var cmd = new SqlCeCommand("SELECT COUNT(*) FROM MovablePublicHolidays WHERE Month = @Month AND Day = @Day", connection))
                {
                    cmd.Parameters.AddWithValue("@Month", date.Month);
                    cmd.Parameters.AddWithValue("@Day", date.Day);
                    if ((int)cmd.ExecuteScalar() > 0) return true;
                }
            }
            return false;
        }

        private void BatchLogSessions(List<ActivitySession> sessions)
        {
            if (sessions == null || !sessions.Any()) return;

            try
            {
                using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
                {
                    connection.Open();
                    foreach (var entry in sessions)
                    {
                        if (entry.DurationMinutes < 0.1) continue; //minimum 1

                        // The query is now much simpler. We only insert the raw facts.
                        using (var cmd = new SqlCeCommand("INSERT INTO ActivitySessions (ConfigurationID, StartTime, EndTime, DurationMinutes, SessionType, OvertimeMinutes, Notes) VALUES (@ConfigID, @Start, @End, @Duration, @Type, @Overtime, @Notes)", connection))
                        {
                            cmd.Parameters.AddWithValue("@ConfigID", entry.ConfigurationID);
                            cmd.Parameters.AddWithValue("@Start", entry.StartTime);
                            cmd.Parameters.AddWithValue("@End", entry.EndTime);
                            cmd.Parameters.AddWithValue("@Duration", entry.DurationMinutes);
                            cmd.Parameters.AddWithValue("@Type", entry.SessionType);
                            cmd.Parameters.AddWithValue("@Overtime", entry.OvertimeMinutes);
                            cmd.Parameters.AddWithValue("@Notes", (object)entry.Notes ?? DBNull.Value); // Handle null notes
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                Log.Information("Committed {Count} raw session chunks to database.", sessions.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to log sessions to database.");
                // We would need a robust retry mechanism here. For now, we log the error.
            }
        }

        /// <summary>
        /// Inserts a list of session chunks using a provided, open connection and an optional transaction.
        /// </summary>
        private void BatchLogSessions(List<ActivitySession> sessions, SqlCeConnection connection, SqlCeTransaction transaction)
        {
            if (sessions == null || !sessions.Any()) return;

            // We assume the connection is already open.
            foreach (var chunk in sessions.OrderBy(s => s.StartTime))
            {
                if (chunk.DurationMinutes < 0.1) continue;

                using (var cmdInsert = new SqlCeCommand("INSERT INTO ActivitySessions (ConfigurationID, StartTime, EndTime, DurationMinutes, SessionType, OvertimeMinutes, Notes) VALUES (@ConfigID, @Start, @End, @Duration, @Type, @Overtime, @Notes)", connection, transaction))
                {
                    cmdInsert.Parameters.AddWithValue("@ConfigID", chunk.ConfigurationID);
                    cmdInsert.Parameters.AddWithValue("@Start", chunk.StartTime);
                    cmdInsert.Parameters.AddWithValue("@End", chunk.EndTime);
                    cmdInsert.Parameters.AddWithValue("@Duration", chunk.DurationMinutes);
                    cmdInsert.Parameters.AddWithValue("@Type", chunk.SessionType);
                    cmdInsert.Parameters.AddWithValue("@Overtime", chunk.OvertimeMinutes);
                    cmdInsert.Parameters.AddWithValue("@Notes", (object)chunk.Notes ?? DBNull.Value);
                    cmdInsert.ExecuteNonQuery();
                }
            }
            // We don't log success here, as the calling method is responsible for the final commit.
        }
        #endregion




        //private double CalculateDailyDowntime(double totalActiveMinutes, DateTime date, bool useTolerance)
        //{
        //    string dayType = GetSessionType(date);
        //    double downtime = 0;

        //    if (dayType == "Normal")
        //    {
        //        int tolerance = useTolerance ? _activeConfig.StartTimeToleranceMinutes : 0;
        //        double downtimeThreshold = _activeConfig.WorkDurationMinutes - tolerance;
        //        downtime = Math.Max(0, downtimeThreshold - totalActiveMinutes);
        //    }

        //    return downtime;
        //}


        private void RunGrandReconciliation()
        {
            Log.Information("Grand Reconciliation process running...");

            // This list will collect all new sessions created during this process.
            var allRecoveredSessions = new List<ActivitySession>();

            // --- Étape 1: Récupération de la dernière session ---
            // This now uses our new method that directly queries the database.
            DateTime lastCheckpointTime = GetLastKnownTimeFromDatabase();



            // --- Étape 2: Récupération des événements chronologiques ---
            Log.Information("Scanning Event Logs since {CheckpointTime}", lastCheckpointTime.ToString("yyyy-MM-dd HH:mm:ss"));
            List<SystemEvent> events = GetSystemEventsSince(lastCheckpointTime);

            if (!events.Any())
            {
                Log.Information("No new system events found since last checkpoint. Reconciliation not needed.");
                _currentSessionStartTime = lastCheckpointTime;
                return;
            }
            // --- NEW DIAGNOSTIC LOGGING BLOCK ---
            Log.Information("--- Processing {Count} Chronological System Events ---", events.Count);
            foreach (var ev in events)
            {
                // Log each event in a structured way.
                Log.Information("  -> Event: {EventType,-10} at {Timestamp}", ev.EventType, ev.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            Log.Information("--------------------------------------------------");
            // --- END LOGGING BLOCK ---

            // The reconciliation process starts from our last known safe point.
            DateTime cursor = lastCheckpointTime;




            // --- Parcours des événements (Main Timeline Processing Loop) ---
            foreach (var ev in events)
            {
                // Process the segment of time BETWEEN the last event (or checkpoint) and this one.
                DateTime segmentStart = cursor;
                DateTime segmentEnd = ev.Timestamp;

                // Ignore tiny or negative time segments.
                if ((segmentEnd - segmentStart).TotalSeconds < 1)
                {
                    cursor = segmentEnd; // Still advance the cursor
                    continue;
                }

                // Determine if the machine was ACTIVE during this segment.

                bool wasActive = (ev.EventType == "Sleep" || ev.EventType == "Shutdown");

                if (wasActive)
                {
                    // --- Étape 3: Si 'event' est {sleep, shutdown}, insérer une session d'activité ---
                    Log.Information("Reconstructing ACTIVE session from {Start} to {End}", segmentStart.ToString("yyyy-MM-dd HH:mm:ss"), segmentEnd.ToString("yyyy-MM-dd HH:mm:ss"));
                    // Instead of just inserting, we try to close an existing open session first.
                    List<ActivitySession> wasCrashedSessionClosed = CloseAnyCrashedSession(segmentEnd);
                    if (wasCrashedSessionClosed != null) allRecoveredSessions.AddRange(wasCrashedSessionClosed);
                    // If we didn't just close a crashed session, it means this is a new segment
                    // that needs to be inserted.
                    if (wasCrashedSessionClosed == null)
                    {
                        var recoveredSession = new ActivitySession
                        {
                            ConfigurationID = _activeConfig.ID,
                            StartTime = segmentStart,
                            EndTime = segmentEnd,
                            Notes = "Recovered"
                        };
                        List<ActivitySession> chunksToCommit = SplitSessionByDay(recoveredSession);
                        allRecoveredSessions.AddRange(chunksToCommit);
                        BatchLogSessions(chunksToCommit);
                    }
                }
                else // The machine was INACTIVE (asleep or off)
                {
                    // --- Étape 4: Vérifier si le segment est Downtime ---
                    CheckForDowntime(segmentStart, segmentEnd);
                }

                // Avancer le curseur
                cursor = segmentEnd;
            }

            // --- Post-traitement: Gérer le dernier segment jusqu'à maintenant ---
            // The time from the very last event until the app started now is an active session.
            if (cursor < DateTime.Now)
            {
                Log.Information("Reconstructing FINAL active session from {Start} ", cursor.ToString("yyyy-MM-dd HH:mm:ss"));

                CreateNewActiveSession(cursor);
            }

            if (allRecoveredSessions == null || !allRecoveredSessions.Any())
            {
                return;
            }
            Log.Information("Total recovered sessions to be displayed: {Count}", allRecoveredSessions.Count);

            // --- THIS IS THE KEY PART ---
            // Create an instance of your new form, passing the data to its constructor.
            using (var summaryForm = new RecoverySummaryForm(allRecoveredSessions))
            {
                var result = summaryForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Log.Information("User confirmed recovery summary.");
                }
            }

            Log.Information("Grand Reconciliation process finished.");
        }

        /// <summary>
        /// Inserts a single Downtime record into the database.
        /// </summary>
        private void InsertDowntimeRecord(Downtime downtime)
        {
            if (downtime == null || downtime.DurationMinutes < 1) return;

            Log.Information("Logging new downtime record from {Start} to {End} ({Duration} mins)",
                downtime.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), downtime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), Math.Round(downtime.DurationMinutes, 2));

            try
            {
                using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
                {
                    conn.Open();
                    using (var cmd = new SqlCeCommand("INSERT INTO Downtime (ConfigurationID, StartTime, EndTime, DurationMinutes) VALUES (@ConfigID, @Start, @End, @Duration)", conn))
                    {
                        cmd.Parameters.AddWithValue("@ConfigID", downtime.ConfigurationID);
                        cmd.Parameters.AddWithValue("@Start", downtime.StartTime);
                        cmd.Parameters.AddWithValue("@End", downtime.EndTime);
                        cmd.Parameters.AddWithValue("@Duration", downtime.DurationMinutes);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to insert downtime record into database.");
            }
        }

        /// <summary>
        /// Finds an open-ended session in the database and closes it with the provided end time.
        /// This is used by the reconciliation to close sessions that were left open by a crash.
        /// </summary>
        /// <returns>True if a session was found and closed, otherwise false.</returns>
        private List<ActivitySession> CloseAnyCrashedSession(DateTime endTime)
        {
            ActivitySession crashedSession = null;
            try
            {
                // Find the open session
                using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
                {
                    conn.Open();
                    using (var cmd = new SqlCeCommand("SELECT * FROM ActivitySessions WHERE EndTime IS NULL", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            crashedSession = new ActivitySession {
                                ConfigurationID = (int)reader["ConfigurationID"],
                                StartTime = (DateTime)reader["StartTime"] };
                        }
                    }

                    if (crashedSession != null)
                    {
                        // We found one. Now close it.
                        // First, DELETE the open-ended record to prevent duplicates.
                        using (var delCmd = new SqlCeCommand("DELETE FROM ActivitySessions WHERE EndTime IS NULL", conn))
                        {
                            delCmd.ExecuteNonQuery();
                        }

                        // Now, create the proper, closed version of the session.
                        crashedSession.EndTime = endTime;
                        crashedSession.Notes = "Recovered";
                        Log.Warning("Found and closing a crashed session from {Start} to {End}", crashedSession.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), crashedSession.EndTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");

                        // Split and log it.
                        List<ActivitySession> chunks = SplitSessionByDay(crashedSession);
                        BatchLogSessions(chunks);
                        return chunks; // We successfully closed a session.
                    }
                }
            }
            catch (Exception ex) { Log.Error(ex, "Error during crashed session cleanup."); }

            return null; // No crashed session was found.
        }


        /// <summary>
        /// Finds the last known timestamp from the database to use as the starting
        /// point for the reconciliation process, as per the defined logic.
        /// </summary>
        private DateTime GetLastKnownTimeFromDatabase()
        {
            DateTime lastKnownTime = DateTime.MinValue;

            try
            {
                using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
                {
                    conn.Open();
                    DateTime? maxEndTime = null;
                    DateTime? openSessionStartTime = null;

                    // 1. Get the latest EndTime from any closed session.
                    using (var cmd = new SqlCeCommand("SELECT MAX(EndTime) FROM ActivitySessions WHERE EndTime IS NOT NULL", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        // This check is correct because MAX() can return DBNull if the table is empty.
                        if (result != DBNull.Value && result != null)
                        {
                            maxEndTime = (DateTime)result;
                        }
                    }

                    // 2. Get the StartTime of any open session.
                    using (var cmd = new SqlCeCommand("SELECT StartTime FROM ActivitySessions WHERE EndTime IS NULL", conn))
                    {
                        var result = cmd.ExecuteScalar();

                        // --- FIX IS HERE ---
                        // Check for null, which indicates NO ROWS were found.
                        // Also check for DBNull just in case, which is good practice.
                        if (result != null && result != DBNull.Value)
                        {
                            openSessionStartTime = (DateTime)result;
                        }
                    }

                    // 3. Decide which time to use based on the logic.
                    if (maxEndTime.HasValue && openSessionStartTime.HasValue)
                    {
                        // This case is unlikely but possible. Use the later of the two.
                        lastKnownTime = maxEndTime.Value > openSessionStartTime.Value ? maxEndTime.Value : openSessionStartTime.Value;
                    }
                    else if (maxEndTime.HasValue)
                    {
                        lastKnownTime = maxEndTime.Value;
                    }
                    else if (openSessionStartTime.HasValue)
                    {
                        lastKnownTime = openSessionStartTime.Value;
                    }
                    else
                    {
                        // Database is completely empty (first run).
                        // The starting point for reconciliation is the beginning of today.
                        Log.Information("No previous sessions found. Setting first-run checkpoint to the start of today.");
                        lastKnownTime = DateTime.Today; // e.g., 2024-05-19 00:00:00
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get last known time from database. Defaulting to 7 days ago.");
                // If the DB query fails, fall back to a safe, recent default.
                lastKnownTime = DateTime.Now.AddDays(-7);
            }

            // Final safety check to prevent overflow errors with SQL CE's date range.
            if (lastKnownTime.Year < 1754)
            {
                return new DateTime(1754, 1, 1);
            }

            return lastKnownTime;
        }




        private List<SystemEvent> GetSystemEventsSince(DateTime startTime)
        {
            Log.Information("Querying system events starting from {StartTime}", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var events = new List<SystemEvent>();

            try
            {
                // Define the Event IDs to look for: 1=Resume, 42=Sleep, 12=Startup, 13=Shutdown
                string query = $@"
            *[System[
                (EventID=1 or EventID=42 or EventID=12 or EventID=13)
                and TimeCreated[@SystemTime >= '{startTime.ToUniversalTime().ToString("o")}']
            ]]";

                var elq = new EventLogQuery("System", PathType.LogName, query);
                using (var reader = new EventLogReader(elq))
                {
                    for (EventRecord ev = reader.ReadEvent(); ev != null; ev = reader.ReadEvent())
                    {
                        if (ev.TimeCreated == null)
                            continue;

                        string type = null;

                        switch (ev.Id)
                        {
                            case 1:
                                if (ev.ProviderName == "Microsoft-Windows-Power-Troubleshooter")
                                    type = "Resume";
                                break;

                            case 42:
                                if (ev.ProviderName == "Microsoft-Windows-Kernel-Power")
                                    type = "Sleep";
                                break;

                            case 12:
                                if (ev.ProviderName == "Microsoft-Windows-Kernel-General")
                                    type = "Startup";
                                break;

                            case 13:
                                if (ev.ProviderName == "Microsoft-Windows-Kernel-General")
                                    type = "Shutdown";
                                break;
                        }

                        // Only add if type was successfully identified
                        if (type != null)
                        {
                            events.Add(new SystemEvent
                            {
                                Timestamp = ev.TimeCreated.Value,
                                EventType = type
                            });
                        }




                        Log.Information("EventID={Id}, Provider={Provider}, Time={Time}", ev.Id, ev.ProviderName, ev.TimeCreated?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");

                    }
                }

                Log.Information("Found {Count} system events.", events.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not access system event logs during query.");
                return new List<SystemEvent>();
            }

            //added the following part to ignore duplicate events and events that are too close together (usually because of failed sleep

            var orderedEvents = events
            .OrderBy(e => e.Timestamp)
            .ToList();

            var filteredEvents = new List<SystemEvent>();
            SystemEvent previous = null;

            foreach (var current in orderedEvents)
            {
                if (previous != null)
                {
                    // Ignore duplicate types back-to-back (e.g., Sleep -> Sleep)
                    if (previous.EventType == current.EventType)
                        continue;

                    // Ignore events happening too close together (e.g., < 1 sec apart)
                    var delta = current.Timestamp - previous.Timestamp;
                    if (delta.TotalSeconds < 1)
                        continue;
                }
                filteredEvents.Add(current);
                previous = current;
            }
            return filteredEvents;
        }



        /// <summary>
        /// Analyzes an inactive time segment and logs any part of it that falls within
        /// standard working hours on a normal workday to the Downtime table.
        /// </summary>
        private void CheckForDowntime(DateTime segmentStart, DateTime segmentEnd)
        {
            // The inactive segment might span multiple days (e.g., a long weekend).
            // We need to process each day within the segment individually.
            var cursorDate = segmentStart.Date;
            while (cursorDate <= segmentEnd.Date)
            {
                // 1. Check if this specific day is a normal workday.
                if (GetSessionType(cursorDate) == "Normal")
                {
                    // 2. Define the work interval for this specific day.
                    DateTime workStart = cursorDate.Date + _activeConfig.WorkStartTime;
                    DateTime workEnd = cursorDate.Date + _activeConfig.WorkEndTime;

                    // 3. Find the intersection (overlap) between the inactive segment and the work interval.
                    DateTime downtimeStart = (segmentStart > workStart) ? segmentStart : workStart;
                    DateTime downtimeEnd = (segmentEnd < workEnd) ? segmentEnd : workEnd;

                    // 4. If there is a valid, positive overlap, log it (after excluding break time).
                    if (downtimeEnd > downtimeStart &&
                        !(downtimeStart == workStart && (downtimeEnd - downtimeStart).TotalMinutes <= _activeConfig.StartTimeToleranceMinutes))
                    {
                        // Define break time interval for this day
                        DateTime breakStart = cursorDate.Date + _activeConfig.BreakStartTime;
                        DateTime breakEnd = cursorDate.Date + _activeConfig.BreakEndTime;

                        // Find overlap between downtime and break
                        DateTime breakOverlapStart = (downtimeStart > breakStart) ? downtimeStart : breakStart;
                        DateTime breakOverlapEnd = (downtimeEnd < breakEnd) ? downtimeEnd : breakEnd;

                        double breakOverlapMinutes = (breakOverlapEnd > breakOverlapStart)
                            ? (breakOverlapEnd - breakOverlapStart).TotalMinutes
                            : 0;

                        double effectiveDowntime = (downtimeEnd - downtimeStart).TotalMinutes - breakOverlapMinutes;

                        if (effectiveDowntime > 0)
                        {
                            var downtimeEntry = new Downtime
                            {
                                ConfigurationID = _activeConfig.ID,
                                StartTime = downtimeStart,
                                EndTime = downtimeEnd,
                                DurationMinutes = effectiveDowntime
                            };

                            // Insert this record into the database.
                            InsertDowntimeRecord(downtimeEntry);
                        }
                    }
                }

                // Move to the next day.
                cursorDate = cursorDate.AddDays(1);
            }
        }


        private void InitializeLiveSessionState()
        {
            // --- NEW LOGIC: Check if the day has changed ---
            if (DateTime.Today > _lastKnownDate.Date)
            {
                Log.Information("New day detected. Resetting daily state flags.");
                // It's a new day, so we reset the email scheduling flag.
                _isHomeOfficeEmailScheduledToday = false;
            }
            _lastKnownDate = DateTime.Now;

            Log.Information("Initializing live session state cache for UI.");
            _historicalOvertimeTotals = new Dictionary<string, double>
                                            {
                                                { "Normal", 0 },
                                                { "Saturday", 0 },
                                                { "Sunday", 0 },
                                                { "Public Holiday", 0 }
                                            };
            try
            {
                using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
                {
                    connection.Open();

                    // Define cutoff date for 30-day filtering
                    var cutoffDate = DateTime.Now.AddDays(-30);

                    // STEP 1: Load all sessions with potential compensation
                    string sessionQuery = @"
                SELECT 
                    a.ID,
                    a.SessionType, 
                    a.StartTime, 
                    a.OvertimeMinutes,
                    COALESCE(c.TotalSpent, 0) AS TotalSpent
                FROM 
                    ActivitySessions a
                LEFT JOIN (
                    SELECT ActivitySessionID, SUM(CompensatedMinutes) as TotalSpent
                    FROM Compensations
                    GROUP BY ActivitySessionID
                ) c ON a.ID = c.ActivitySessionID";

                    using (var cmd = new SqlCeCommand(sessionQuery, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime startTime = reader.GetDateTime(reader.GetOrdinal("StartTime"));
                            if (startTime < cutoffDate) continue; // skip old sessions

                            string type = reader.GetString(reader.GetOrdinal("SessionType"));
                            double overtime = Convert.ToDouble(reader["OvertimeMinutes"]);
                            double spent = Convert.ToDouble(reader["TotalSpent"]);
                            double net = overtime - spent;

                            if (_historicalOvertimeTotals.ContainsKey(type))
                                _historicalOvertimeTotals[type] += net;
                        }
                    }

                    Log.Information("Initialized historical overtime cache: Normal={Normal}, Saturday={Saturday}, Sunday={Sunday}, Holiday={Holiday}",
                        Math.Round(_historicalOvertimeTotals["Normal"], 2),
                        Math.Round(_historicalOvertimeTotals["Saturday"], 2),
                        Math.Round(_historicalOvertimeTotals["Sunday"], 2),
                        Math.Round(_historicalOvertimeTotals["Public Holiday"], 2));


                    DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

                    string downtimeQuery = @"
                    SELECT 
                        d.StartTime,
                        d.DurationMinutes, 
                        COALESCE(c.TotalCompensated, 0) AS TotalCompensated
                    FROM 
                        Downtime d
                    LEFT JOIN (
                        SELECT DowntimeID, SUM(CompensatedMinutes) as TotalCompensated
                        FROM Compensations
                        GROUP BY DowntimeID
                    ) c ON d.ID = c.DowntimeID";

                    using (var cmd = new SqlCeCommand(downtimeQuery, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            double totalUnresolved = 0;
                            int currentMonth = DateTime.Now.Month;
                            int currentYear = DateTime.Now.Year;

                            while (reader.Read())
                            {
                                DateTime startTime = Convert.ToDateTime(reader["StartTime"]);
                                if (startTime.Month != currentMonth || startTime.Year != currentYear)
                                    continue;

                                double totalDuration = Convert.ToDouble(reader["DurationMinutes"]);
                                double totalCompensated = Convert.ToDouble(reader["TotalCompensated"]);

                                double unresolvedAmount = totalDuration - totalCompensated;
                                if (unresolvedAmount > 0)
                                {
                                    totalUnresolved += unresolvedAmount;
                                }
                            }

                            _totalUnresolvedDowntimeMinutes = totalUnresolved;
                        }
                    }

                    Log.Information("Initialized unresolved downtime cache: {DowntimeMinutes} mins", Math.Round(_totalUnresolvedDowntimeMinutes, 2));

                }
            }
            catch (Exception ex) { Log.Error(ex, "Failed to initialize live session state."); }


        }

        #region Real-Time UI Update Logic



        /// <summary>
        /// Called every second by the UI timer to update all on-screen labels with real-time data.
        /// This method uses cached state and performs only in-memory calculations for high performance.
        /// </summary>


        private void UpdateLiveDisplay()
        {

            // --- NEW EMAIL SCHEDULER LOGIC ---
            // Check if a notification is scheduled and if the time has come.
            if (_notificationSendTime.HasValue && DateTime.Now >= _notificationSendTime.Value)
            {
                // We have a scheduled email to send.

                // To prevent this block from running again on the next tick,
                // we capture the send time and then immediately clear the schedule.
                DateTime scheduledTime = _notificationSendTime.Value;
                _notificationSendTime = null; // Clear the schedule *before* starting the task.

                Log.Information("Email send time reached at {Time}. Starting send task.", scheduledTime);

                // --- THE FIX IS HERE: Use Task.Run ---
                // This launches the email sending operation on a separate background thread
                // from the thread pool, ensuring it does not block our UI timer or the UI itself.
                Task.Run(() =>
                {
                    // This code block runs in the background.
                    _emailService.SendHomeOfficeNotification();
                });
            }


            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (this.InvokeRequired) { this.BeginInvoke(new Action(UpdateLiveDisplay)); return; }

            if (!_currentSessionStartTime.HasValue)
            {
                lbl_status.Text = "Inactive (Asleep)";
                // Optionally clear other labels here if you want.
                return;
            }

            lbl_status.Text = "Session Active";

            // --- Step 1: Calculate the overtime for the CURRENT LIVE SESSION ONLY ---

            double rawOvertimeThisSession = 0;
            var sessionStart = _currentSessionStartTime.Value;
            var sessionEnd = DateTime.Now;

            DateTime workStart = sessionStart.Date + _activeConfig.WorkStartTime;
            DateTime workEnd = sessionStart.Date + _activeConfig.WorkEndTime;

            // This is the per-chunk calculation logic we designed.
            string currentSessionType = GetSessionType(sessionStart);
            if (currentSessionType != "Normal")
            {
                rawOvertimeThisSession = (sessionEnd - sessionStart).TotalMinutes;
            }
            else
            {
                
                if (sessionStart < workStart)
                    rawOvertimeThisSession += ((sessionEnd < workStart ? sessionEnd : workStart) - sessionStart).TotalMinutes;
                if (sessionEnd > workEnd)
                    rawOvertimeThisSession += (sessionEnd - (sessionStart > workEnd ? sessionStart : workEnd)).TotalMinutes;
            }

            // --- Step 2: Calculate all the display values by combining historical and live data ---

            // Create temporary variables for today's totals by category.
            double normalToday = _historicalOvertimeTotals["Normal"];
            double saturdayToday = _historicalOvertimeTotals["Saturday"];
            double sundayToday = _historicalOvertimeTotals["Sunday"];
            double holidayToday = _historicalOvertimeTotals["Public Holiday"];



            // The grand total is the sum of all categories.
            double totalOvertime = normalToday + saturdayToday + sundayToday + holidayToday;
            // --- Step 3: Update label 3 to show total unresolved downtime ---
            label3.Text = FormatDuration(_totalUnresolvedDowntimeMinutes);
            // --- Step 4: Update all six UI Labels as requested ---

            // 1. LabelSessionTotal: Overtime of the CURRENT session
            
            DateTime now = DateTime.Now;
            LabelSessionTotal.Text = (now >=workStart && now <=workEnd) ? GetMotivationalMessage() : FormatCounter(rawOvertimeThisSession);

            // 2. lblTotalOvertime: The grand total overtime balance
            lblTotalOvertime.Text = FormatDuration(totalOvertime);

            // 3. lblOvertimeNormalDays: Total Normal overtime (historical + live)
            lblOvertimeNormalDays.Text = FormatDuration(normalToday);

            // 4. lblOvertimeSaturdays: Total Saturday overtime (historical + live)
            lblOvertimeSaturdays.Text = FormatDuration(saturdayToday);

            // 5. lblOvertimeSundays: Total Sunday overtime (historical + live)
            lblOvertimeSundays.Text = FormatDuration(sundayToday);

            // 6. lblOvertimePublicHolidays: Total Holiday overtime (historical + live)
            lblOvertimePublicHolidays.Text = FormatDuration(holidayToday);

            // Also update the helper labels to be descriptive
            labelStartSession.Text = $"Session Started At: {sessionStart.Hour}:{sessionStart.Minute}:{sessionStart.Second}";
            label2.Text = "Normal Days OT:";
            label9.Text = "Saturdays OT:";
            label7.Text = "Sundays OT:";
            label5.Text = "Holidays OT:";
        }

        private string FormatDuration(double minutes)
        {
            if (minutes < 1) return "0m";
            TimeSpan t = TimeSpan.FromMinutes(Math.Round(minutes));
            return $"{(int)t.TotalHours}h {t.Minutes}m";
        }

        private string FormatCounter(double totalMinutes)
        {
            TimeSpan time = TimeSpan.FromMinutes(totalMinutes);
            return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }


        private void SendHomeOfficeEmail()
        {
            // The MainForm's only job is to tell the service to do its work.
            // It doesn't care about the details anymore.
            _emailService.SendHomeOfficeNotification();
        }

        #endregion

        #region Form Events and System Tray

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Information("MainForm is closing.");
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
            _uiUpdateTimer?.Dispose();

            // --- CRITICAL CHANGE ---
            // If the user closes the form, the session remains "active" in the database
            // for the GrandReconciliation to find on the next startup.
            // We only update the checkpoint on a REAL system shutdown.
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                CloseActiveSession();
            }
        }

        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon { Icon = SystemIcons.Application, Visible = true, Text = "WOTTracker" };
            _contextMenu = new ContextMenuStrip();
            _reconfigureMenuItem = new ToolStripMenuItem("Reconfigure Application", null, Reconfigure_Click);
            _exportMenuItem = new ToolStripMenuItem("Export to Excel", null, ButtonExport_Click);
            _clearDatabaseMenuItem = new ToolStripMenuItem("Clear Database", null, ClearDatabase);
            _contextMenu.Items.AddRange(new ToolStripItem[] { _exportMenuItem, _reconfigureMenuItem, _clearDatabaseMenuItem });
            _notifyIcon.ContextMenuStrip = _contextMenu;
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon1.ContextMenuStrip = _contextMenu;
        }

        /// <summary>
        /// Handles the click event for the new "Reconfigure" menu item.
        /// </summary>
        private void Reconfigure_Click(object sender, EventArgs e)
        {
            Log.Information("User initiated reconfiguration process.");

            // Load the currently active configuration to pass to the form.
            var currentConfig = _activeConfig;
            if (currentConfig == null)
            {
                MessageBox.Show("Could not load current configuration to edit.", "Error");
                return;
            }

            this.TopMost = false;
            // Show the SetupForm, passing in the current settings.
            using (var setupForm = new SetupForm(currentConfig)) // Use the new constructor
            {
                DialogResult result = setupForm.ShowDialog(this);

                // If the user clicked "Save" in the setup form...
                if (result == DialogResult.OK)
                {
                    Log.Information("Configuration changed. Application will now restart to apply new settings.");
                    MessageBox.Show("Configuration has been saved. The application will now restart to apply the new settings.", "Restart Required", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Restart the application.
                    Application.Restart();
                    Environment.Exit(0); // Ensure the current instance closes cleanly.
                }
                else
                {
                    Log.Information("Reconfiguration was cancelled by the user.");
                }
            }
            this.TopMost = true;
        }

        // --- All other methods below are either stubs for the designer or can be implemented later ---
        private void MainForm_Resize(object sender, EventArgs e) { if (this.WindowState == FormWindowState.Minimized) { this.Hide(); notifyIcon1.Visible = true; } }
        private void NotifyIcon_DoubleClick(object sender, EventArgs e) { this.Show(); this.WindowState = FormWindowState.Normal; notifyIcon1.Visible = false; }
        private void ButtonClose_Click(object sender, EventArgs e) => this.Close();
        private void ButtonMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void ButtonExport_Click(object sender, EventArgs e)
        {
            // Check if the background worker is already running another report.
            if (backgroundWorker1.IsBusy)
            {
                MessageBox.Show("An export operation is already in progress. Please wait.", "Busy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Update the UI to show that the process has started.
            toolStripStatusLabel.Text = "Starting report generation...";
            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee; // An animated progress bar for unknown duration

            // Start the background worker, telling it which task to perform.
            backgroundWorker1.RunWorkerAsync("ExportToExcel");
        }
        private void ButtonSendSummary_Click(object sender, EventArgs e) {
            try
            {
                // Gather all the necessary data on the UI thread (can be slow if large).
                // For a real app, this would also be on a background worker.
                var overtimeByDay = GetOvertimeByDayOfWeek();
                var longestSessions = GetLongestSessions();
                var weeklyOvertime = GetWeeklyOvertimeForBurnoutCheck();

                // Create and show the dashboard form.
                using (var dashboard = new DashboardForm(overtimeByDay, longestSessions, weeklyOvertime))
                {
                    dashboard.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not generate dashboard data: {ex.Message}", "Error");
                Log.Error(ex, "Failed to create dashboard.");
            }
        }
        private void OpenSettings(object sender, EventArgs e) { /* Placeholder */ }
        private void ClearDatabase(object sender, EventArgs e) { /* Placeholder */ }
        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            NotifyIcon_DoubleClick(sender, e);
        }

        #region Stubs for Designer
        private void btn_ExportToExcel(object sender, EventArgs e) { ButtonExport_Click(sender, e); }
        private void pictureBox7_MouseHover(object sender, EventArgs e) { }
        private void pictureBox5_MouseHover(object sender, EventArgs e) { }
        private void PanelTop_MouseDown(object sender, MouseEventArgs e) { }
        private void BackgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) { }
        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // Hide the progress bar
            progressBar1.Visible = false;
            progressBar1.Style = ProgressBarStyle.Blocks; // Reset style

            // Check the e.Result to see what happened in the background thread.
            if (e.Error != null)
            {
                // An unhandled exception occurred.
                toolStripStatusLabel.Text = "Export failed with an unhandled error.";
                MessageBox.Show($"An unexpected error occurred during export: {e.Error.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Result is Exception ex)
            {
                // We caught an exception and passed it back.
                toolStripStatusLabel.Text = "Export failed. Check logs for details.";
                MessageBox.Show($"An error occurred during export: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Cancelled)
            {
                toolStripStatusLabel.Text = "Export cancelled.";
            }
            else
            {
                // Success! The e.Result is the string we set in DoWork.
                toolStripStatusLabel.Text = e.Result.ToString();
            }
        }
        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) { }
        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) { }

        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {

        }


        #region Comprehensive Reporting Engine

        /// <summary>
        /// The main engine that gathers and aggregates all historical data to build a comprehensive report.
        /// This is a potentially slow operation and should be run on a background thread.
        /// </summary>
        private ComprehensiveReport GenerateComprehensiveReportData()
        {
            var report = new ComprehensiveReport();

            using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                connection.Open();

                // --- 1. Fetch all necessary raw data tables at once ---
                this.Invoke(new Action(() => toolStripStatusLabel.Text = "Fetching historical data..."));

                // Load raw data tables. These will be displayed on their own sheets.
                using (var adapter = new SqlCeDataAdapter("SELECT ID, ConfigurationID, StartTime, EndTime, DurationMinutes, SessionType, OvertimeMinutes, Notes FROM ActivitySessions ORDER BY StartTime", connection))
                { report.RawActivityData = new DataTable("RawActivity"); adapter.Fill(report.RawActivityData); }

                using (var adapter = new SqlCeDataAdapter("SELECT * FROM Downtime ORDER BY StartTime", connection))
                { report.DowntimeData = new DataTable("Downtime"); adapter.Fill(report.DowntimeData); } // Assuming a DowntimeData table in your report class

                using (var adapter = new SqlCeDataAdapter("SELECT * FROM Compensations ORDER BY TransactionDate", connection))
                { report.CompensationData = new DataTable("Compensations"); adapter.Fill(report.CompensationData); }

                // --- 2. Process the raw data into a Daily Breakdown ---
                this.Invoke(new Action(() => toolStripStatusLabel.Text = "Aggregating daily totals..."));

                // Group the raw activity by date.
                var activityByDay = report.RawActivityData.AsEnumerable()
                    .GroupBy(row => row.Field<DateTime>("StartTime").Date);

                // Group the raw downtime by date.
                var downtimeByDay = report.DowntimeData.AsEnumerable()
                    .GroupBy(row => row.Field<DateTime>("StartTime").Date);

                // Get a set of all unique dates from both activity and downtime.
                var allDates = activityByDay.Select(g => g.Key).Union(downtimeByDay.Select(g => g.Key)).OrderBy(d => d);

                foreach (var currentDate in allDates)
                {
                    // Sum up the pre-calculated values for this day.
                    double totalActiveMinutes = activityByDay.FirstOrDefault(g => g.Key == currentDate)?.Sum(r => r.Field<double>("DurationMinutes")) ?? 0;
                    double officialOvertime = activityByDay.FirstOrDefault(g => g.Key == currentDate)?.Sum(r => r.Field<double>("OvertimeMinutes")) ?? 0;
                    double officialDowntime = downtimeByDay.FirstOrDefault(g => g.Key == currentDate)?.Sum(r => r.Field<double>("DurationMinutes")) ?? 0;

                    report.DailyBreakdownData.Rows.Add(
                        currentDate.ToShortDateString(),
                        GetSessionType(currentDate), // GetSessionType is still useful for classifying the day itself
                        FormatDuration(totalActiveMinutes),
                        FormatDuration(officialDowntime),
                        FormatDuration(officialOvertime)
                    );
                }

                // --- 3. Create the final summary data from the processed daily breakdown ---
                this.Invoke(new Action(() => toolStripStatusLabel.Text = "Generating summary..."));

                // Calculate total credits (earned overtime)
                double totalOvertimeCredit = report.RawActivityData.AsEnumerable().Sum(r => r.Field<double>("OvertimeMinutes"));

                // Calculate total debits (spent on compensation)
                double totalOvertimeSpent = report.CompensationData.AsEnumerable()
                    .Where(r => r.Field<string>("Reason") == "Compensation")
                    .Sum(r => r.Field<double>("CompensatedMinutes"));

                // The net balance is the difference.
                double netOvertimeBalance = totalOvertimeCredit - totalOvertimeSpent;

                // Create summary rows. This part now needs to aggregate from the DailyBreakdownData.
                var summaryByCategory = report.DailyBreakdownData.AsEnumerable()
                    .GroupBy(row => row.Field<string>("DayType"))
                    .Select(group => new {
                        Category = group.Key,
                        TotalOvertime = group.Sum(r => ParseDuration(r.Field<string>("OfficialOvertime")))
                    }).ToList();

                foreach (var item in summaryByCategory)
                {
                    report.SummaryData.Rows.Add(item.Category, FormatDuration(item.TotalOvertime));
                }
                report.SummaryData.Rows.Add("NET BALANCE", FormatDuration(netOvertimeBalance));
            }

            return report;
        }

        // You will need a helper to parse the "Xh Ym" format back to minutes for summing.
        private double ParseDuration(string duration)
        {
            if (string.IsNullOrEmpty(duration)) return 0;
            double totalMinutes = 0;
            var parts = duration.Split(' ');
            foreach (var part in parts)
            {
                if (part.EndsWith("h")) totalMinutes += int.Parse(part.Replace("h", "")) * 60;
                if (part.EndsWith("m")) totalMinutes += int.Parse(part.Replace("m", ""));
            }
            return totalMinutes;
        }


        // This method now just focuses on creating the Excel file from pre-processed data.
        private void ExportToExcel(ComprehensiveReport reportData)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    // --- Sheet 1: Summary ---
                    var summarySheet = workbook.Worksheets.Add("Summary");
                    summarySheet.Cell(1, 1).InsertTable(reportData.SummaryData);
                    summarySheet.Columns().AdjustToContents();

                    // --- Sheet 2: Daily Breakdown ---
                    var dailySheet = workbook.Worksheets.Add("Daily Breakdown");
                    dailySheet.Cell(1, 1).InsertTable(reportData.DailyBreakdownData);
                    dailySheet.Columns().AdjustToContents();

                    // --- Sheet 3: Raw Activity Log ---
                    var rawSheet = workbook.Worksheets.Add("Raw Activity Log");
                    rawSheet.Cell(1, 1).InsertTable(reportData.RawActivityData);
                    rawSheet.Columns().AdjustToContents();

                    // --- Sheet 4: Compensation Log ---
                    var compSheet = workbook.Worksheets.Add("Compensation Log");
                    compSheet.Cell(1, 1).InsertTable(reportData.CompensationData);
                    compSheet.Columns().AdjustToContents();

                    // Save and open the file
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string fileName = $"ActivityReport_{timestamp}.xlsx";
                    string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                    workbook.SaveAs(filePath);
                    Process.Start(filePath);
                }
                // Update status bar on UI thread
                this.Invoke(new Action(() => toolStripStatusLabel.Text = "Report exported successfully."));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating Excel file.");
                this.Invoke(new Action(() => toolStripStatusLabel.Text = "Error during export."));
            }
        }

        // The background worker is now much cleaner.
        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // Get the task name that was passed from the Click event.
            string task = e.Argument as string;

            if (task == "ExportToExcel")
            {
                try
                {
                    // 1. Run the potentially slow data gathering process.
                    // We use Invoke to safely update the UI from this background thread.
                    this.Invoke(new Action(() => toolStripStatusLabel.Text = "Step 1/2: Gathering and processing historical data..."));
                    ComprehensiveReport reportData = GenerateComprehensiveReportData();

                    // 2. Create the Excel file from the processed data.
                    this.Invoke(new Action(() => toolStripStatusLabel.Text = "Step 2/2: Creating Excel file..."));
                    ExportToExcel(reportData);

                    e.Result = "Export successful."; // Pass a success message to the RunWorkerCompleted event
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred during the ExportToExcel background task.");
                    // Pass the exception so the UI can report the error.
                    e.Result = ex;
                }
            }
            // You can add 'else if (task == "SendEmailSummary")' here later.
        }

        #region Daily Work Location Notification



        private void ScheduleHomeOfficeEmail()
        {
            // --- NEW LOGIC: Check if we've already scheduled an email for today ---
            if (_isHomeOfficeEmailScheduledToday)
            {
                Log.Information("Home office email has already been scheduled for today. No new notification will be set.");
                return; // Do nothing.
            }

            try
            {
                // Rule: Get the target time from the configuration.
                DateTime targetDateTime = DateTime.Today.Add(_activeConfig.NotificationSendTime);

                if (targetDateTime <= DateTime.Now)
                {
                    Log.Warning("Target notification time {TargetTime} has already passed. Email will not be sent.", targetDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    // Even if it passed, we mark it as "scheduled" to prevent re-prompting.
                    _isHomeOfficeEmailScheduledToday = true;
                    return;
                }

                // We will reuse the main UI timer to check the time.
                _notificationSendTime = targetDateTime;

                // --- NEW LOGIC: Set the flag to true after successful scheduling ---
                _isHomeOfficeEmailScheduledToday = true;

                Log.Information("Email notification scheduled for {TargetTime}",
    _notificationSendTime.HasValue
        ? _notificationSendTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
        : "N/A");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to schedule home office email notification.");
            }
        }

        #region Motivational Messages

        // We need a single Random instance for the whole class to avoid generating
        // the same "random" number if the methods are called in quick succession.
        private readonly Random _random = new Random();
        private bool _isWelcomeMessageShown = false;
        private string _cachedMotivationalMessage = null;
        private DateTime _lastMessageUpdateTime = DateTime.MinValue;

        private string GetMotivationalMessage()
        {
            var now = DateTime.Now;
            TimeSpan currentTimeOfDay = now.TimeOfDay;

            // Optional: Refresh once every 10 minutes or on specific triggers
            bool shouldUpdate = false;

            // If it's a new hour or more than 10 minutes have passed since last update
            if ((now - _lastMessageUpdateTime).TotalMinutes >= 10)
            {
                shouldUpdate = true;
            }

            // Or: Update if it's the first time (i.e., cached message is null)
            if (_cachedMotivationalMessage == null)
            {
                shouldUpdate = true;
            }

            if (!shouldUpdate)
            {
                return _cachedMotivationalMessage;
            }

            _lastMessageUpdateTime = now;

            // --- Check for Break Time First ---
            // Make sure your _activeConfig has BreakStartTime and BreakEndTime
            if (_activeConfig.BreakStartTime > TimeSpan.Zero &&
                currentTimeOfDay >= _activeConfig.BreakStartTime &&
                currentTimeOfDay <= _activeConfig.BreakEndTime)
            {
                var breakMessages = new List<string> {
        "Time for a break!",
        "Take a short break.",
        "Stretch and relax!",
        "Break time! Step away from your desk.",
        "Rest your eyes for a few minutes.",
        "Breathe deeply and relax.",
        "Enjoy a quick walk.",
        "Grab a healthy snack.",
        "Hydrate yourself!",
        "Take a moment to unwind.",
        "Pause and refresh your mind.",
        "Relax and recharge.",
        "Step outside for fresh air.",
        "Take a mental break.",
        "Give yourself a short rest.",
        "Take a few deep breaths.",
        "Clear your mind for a bit.",
        "Take a quick stretch.",
        "Enjoy a brief pause.",
        "Take a moment to relax.",
        "“A field that has rested gives a bountiful crop.”",
        "“Rest and be thankful.”",
        "“The most productive thing is to relax.”",
        "“A change of work is the best rest.”",
        "“Rest is not idleness.”",
        "“Rest is the sweet sauce of labor.”"
    };
                _cachedMotivationalMessage =  breakMessages[_random.Next(breakMessages.Count)];
                return _cachedMotivationalMessage;
            }

            // --- Logic for Normal Work Hours ---
            // Calculate remaining time in the workday
            TimeSpan workEndTime = _activeConfig.WorkEndTime;
            TimeSpan remainingTime = workEndTime - currentTimeOfDay;

            // Welcome message on first check of the day
            if (!_isWelcomeMessageShown)
            {
                _isWelcomeMessageShown = true;
                var welcomeMessages = new List<string> { "Welcome! Let's make today productive!",
        "Good to see you! Ready to tackle today's tasks?",
        "Hello! Let's get started on a great day!",
        "Welcome back! Let's achieve some goals today!",
        "Hi there! Ready to make progress?",
        "Good day! Let's get things done!",
        "Welcome! Let's have a productive session!",
        "Hello! Time to get to work!",
        "Welcome back! Let's make today count!",
        "Hi! Ready to dive into today's work?",
        "Good to see you! Let's get started!",
        "Welcome! Let's make today amazing!",
        "Hello! Ready to accomplish great things?",
        "Welcome back! Let's focus and succeed!",
        "Hi there! Let's start the day strong!",
        "Good day! Ready to be productive?",
        "Welcome! Let's achieve our goals!",
        "Hello! Time to get things done!",
        "Welcome back! Let's make progress!",
        "Hi! Ready to work hard and succeed?"
 };
                _cachedMotivationalMessage =  welcomeMessages[_random.Next(welcomeMessages.Count)];
                return _cachedMotivationalMessage;
            }

            // Different messages based on how much of the day is left
            if (remainingTime.TotalHours > 4)
            {
                var earlyMessages = new List<string> { "Within working hours.",
        "No overtime.",
        "Regular hours.",
        "On schedule!",
        "Great job!",
        "Keep it up!",
        "Regular hours in progress.",
        "Scheduled hours.",
        "Take breaks!",
        "Stay hydrated!",
        "Remember to stretch!",
        "You're doing great!",
        "Stay focused!",
        "Keep going!",
        "You're on track!",
        "Maintain your pace.",
        "Stay productive!",
        "Keep the momentum!",
        "You're making progress!",
        "“Do great work, love what you do.”",
        "“Success is not the key to happiness.”",
        "“Love your job, never work a day.”",
        "“Pleasure in the job puts perfection.”",
        "“Work hard, be kind, amazing things.”",
        "“Predict the future by creating it.”",
        "You're on fire!",
        "Keep the energy high!",
        "Stay on top of it!",
        "You're doing awesome!",
        "Keep the good work going!",
        "You're a star!",
        "Keep shining!",
        "You're rocking it!",
        "Stay awesome!",
        "Keep the pace steady!"};
                _cachedMotivationalMessage =  earlyMessages[_random.Next(earlyMessages.Count)];
                return _cachedMotivationalMessage;
            }
            else if (remainingTime.TotalHours > 1)
            {
                var midMessages = new List<string> { "Halfway there!",
        "Keep pushing!",
        "You're doing amazing!",
        "Stay strong!",
        "Keep up the great work!",
        "You're on track!",
        "Keep the momentum going!",
        "Stay motivated!",
        "You're making great progress!",
        "Keep your focus!",
        "You're halfway done!",
        "Stay determined!",
        "Keep your energy up!",
        "You're doing well!",
        "Stay on task!",
        "Keep your spirits high!",
        "You're almost there!",
        "Stay positive!",
        "Keep your head up!",
        "You're doing fantastic!",
        "“Hard work brings luck.”",
        "“Success is small efforts.”",
        "“Believe, you're halfway there.”",
        "“Perseverance is not a long race.”",
        "“Doubts limit our realization.”",
        "“Future depends on today.”",
        "You're halfway through, keep it up!",
        "Keep the momentum strong!",
        "You're doing great, stay focused!",
        "Keep pushing, you're doing fantastic!",
        "Stay strong, you're halfway there!",
        "Keep up the amazing work!",
        "You're making great progress, keep going!",
        "Stay motivated, you're doing well!",
        "Keep your spirits high, you're halfway done!",
        "You're doing fantastic, keep it up!"
 };
                _cachedMotivationalMessage =  midMessages[_random.Next(midMessages.Count)];
                return _cachedMotivationalMessage;
            }
            else
            {
                var lateMessages = new List<string> { $"Only {remainingTime.TotalHours} minutes left!",
        $"Just {remainingTime.TotalHours} minutes to go, you got this!",
        $"Hang in there, {remainingTime.TotalHours} minutes remaining!",
        $"Almost done, {remainingTime.TotalHours} minutes left!",
        "Finish strong!",
        "You're almost there!",
        "Great job, nearly finished!",
        "Final stretch, keep going!",
        "You're about to wrap up!",
        "Almost at the finish line!",
        "Keep pushing, almost done!",
        "You're nearly there!",
        "Just a bit more to go!",
        "Stay focused, almost done!",
        "You're on the home stretch!",
        "Keep your eyes on the prize!",
        "You're so close!",
        "Almost there, keep going!",
        "You're finishing strong!",
        "Just a few more minutes!",
        "“The best way out is through.”",
        "“It seems impossible until done.”",
        "“The last mile is the longest.”",
        "“Finish line is the beginning.”",
        "“Hard battle, sweet victory.”",
        "“Success is not final.”",
        "Almost there, finish strong!",
        "You're nearly done, keep going!",
        "Just a little more, you got this!",
        "You're on the final stretch, keep pushing!",
        "Almost at the finish line, stay focused!",
        "You're about to wrap up, great job!",
        "Keep going, you're almost there!",
        "You're finishing strong, keep it up!",
        "Just a few more minutes, stay strong!",
        "You're so close, keep pushing!" };
                // Personalize the late messages
                _cachedMotivationalMessage = lateMessages[_random.Next(lateMessages.Count)];
                return _cachedMotivationalMessage;
            }
        }
        #region Charting and Data Analysis

        // --- Metric 1: Most Common Overtime Days ---
        public Dictionary<DayOfWeek, double> GetOvertimeByDayOfWeek()
        {
            var overtimeByDay = new Dictionary<DayOfWeek, double>();
            using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                conn.Open();
                // DATEPART(dw, StartTime) returns a number (1=Sunday, 2=Monday, etc., depending on server settings)
                // We will do the mapping in C# to be safe.
                using (var cmd = new SqlCeCommand("SELECT StartTime, OvertimeMinutes FROM ActivitySessions WHERE OvertimeMinutes > 0", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var day = ((DateTime)reader["StartTime"]).DayOfWeek;
                        double overtime = (double)reader["OvertimeMinutes"];
                        if (!overtimeByDay.ContainsKey(day))
                        {
                            overtimeByDay[day] = 0;
                        }
                        overtimeByDay[day] += overtime;
                    }
                }
            }
            return overtimeByDay;
        }

        // --- Metric 2: Longest Sessions ---
        public DataTable GetLongestSessions()
        {
            var dt = new DataTable();
            using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                // Get the top 10 longest sessions
                string query = "SELECT TOP(10) StartTime, DurationMinutes FROM ActivitySessions ORDER BY DurationMinutes DESC";
                using (var adapter = new SqlCeDataAdapter(query, conn))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // --- Metric 3: Burnout Risk ---
        public Dictionary<string, double> GetWeeklyOvertimeForBurnoutCheck()
        {
            var weeklyTotals = new Dictionary<string, double>();
            using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                conn.Open();
                // This query groups overtime by the week number of the year.
                // DATEPART(wk, StartTime) is the key function here.
                string query = "SELECT DATEPART(yy, StartTime) AS Year, DATEPART(wk, StartTime) AS Week, SUM(OvertimeMinutes) AS TotalWeeklyOvertime FROM ActivitySessions GROUP BY DATEPART(yy, StartTime), DATEPART(wk, StartTime) ORDER BY Year, Week";
                using (var cmd = new SqlCeCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string weekLabel = $"{(int)reader["Year"]}-W{(int)reader["Week"]}";
                        weeklyTotals[weekLabel] = (double)reader["TotalWeeklyOvertime"];
                    }
                }
            }
            return weeklyTotals;
        }

        #endregion


        #endregion



        #endregion
        #endregion


        #endregion
    }
}