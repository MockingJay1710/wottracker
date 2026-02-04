using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;
using Serilog;
using WOTTracker.Configuration;

namespace WOTTracker
{

    /// <summary>
    /// Manages all configuration and state persistence with the database.
    /// This is the single point of truth for application settings.
    /// </summary>
    public static class ConfigurationManager
    {
        public static readonly string _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wot_data.sdf");

        private static readonly string _jsonConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wot_tracker_log.txt");
        private static readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "overtime.xslx");

        private static void SaveConfigToJson(ConfigurationSet config)
        {
            try
            {
                // We wrap the config in a root object to match the JSON structure.
                var jsonObject = new { Configurations = config };
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_jsonConfigPath, jsonString);
            }
            catch (Exception ex)
            {
                // Log this error. It's not fatal, but it's important.
                Log.Warning("Warning: Could not save configuration backup to JSON file. " + ex.Message);
            }
        }

        private static ConfigurationSet LoadConfigFromJson()
        {
            if (!File.Exists(_jsonConfigPath)) return null;

            try
            {
                string jsonString = File.ReadAllText(_jsonConfigPath);
                // This uses an anonymous type to match the expected JSON structure.
                var tempObject = new { Configurations = new ConfigurationSet() };
                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(jsonString, tempObject);
                return deserialized?.Configurations;
            }
            catch
            {
                // If parsing fails, the file is corrupt.
                return null;
            }
        }


        public static void EnsureInfrastructureExists()
        {
            // Create the database file itself if it doesn't exist.
            if (!File.Exists(_databasePath))
            {
                try
                {
                    SqlCeEngine engine = new SqlCeEngine($"Data Source={_databasePath}");
                    engine.CreateDatabase();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fatal Error: Could not create the database file.\n\n{ex.Message}", "Infrastructure Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }

            // Now connect to the database and ensure all tables exist.
            using (var connection = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                connection.Open();

                Func<string, bool> tableExists = (tableName) =>
                {
                    using (var cmd = new SqlCeCommand($"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'", connection))
                    {
                        return cmd.ExecuteScalar() != null;
                    }
                };

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Configurations table remains the same
                        if (!tableExists("Configurations"))
                        {
                            using (var cmd = new SqlCeCommand(
                                "CREATE TABLE Configurations (" +
                                "ID INT PRIMARY KEY IDENTITY, " +
                                "EffectiveDate DATETIME, " +
                                "IsActive BIT, " +
                                "UserName NVARCHAR(50), " +
                                "UserRole NVARCHAR(50), " +
                                "WorkStartTime float, " +
                                "WorkEndTime float, " +
                                "BreakStartTime float, " +
                                "BreakEndTime float, " +
                                "WorkDurationMinutes INT, " +
                                "RecipientEmail NVARCHAR(100), " + // Also increased email size just in case
                                "SenderEmail NVARCHAR(100), " +   // Also increased email size just in case
                                "SmtpServer NVARCHAR(100), " +      // Also increased SMTP size
                                "StartTimeToleranceMinutes INT, " +
                                "EndTimeToleranceMinutes INT, " +
                                "LogFilePath NVARCHAR(260), " + 
                                "FilePath NVARCHAR(260), " + 
                                "InsertionDate DATETIME DEFAULT GETDATE()," + 
                                // --- NEW COLUMNS FOR NOTIFICATION ---
                                "NotificationSendTime float, " + // Storing as TimeSpan (float)
                                "NotificationRecipient NVARCHAR(255), " +
                                "NotificationSubject NVARCHAR(255), " +
                                "NotificationBody NVARCHAR(1000)" +

                                ")", // <-- FINAL CLOSING PARENTHESIS
                                connection, transaction))
                            {
                                cmd.ExecuteNonQuery();
                            }


                            // --- CHANGE 1: ActivitySessions table now needs OvertimeMinutes ---
                            if (!tableExists("ActivitySessions"))
                            {
                                using (var cmd = new SqlCeCommand("CREATE TABLE ActivitySessions (ID INT PRIMARY KEY IDENTITY, ConfigurationID INT, StartTime DATETIME, EndTime DATETIME, DurationMinutes FLOAT, SessionType NVARCHAR(50), OvertimeMinutes FLOAT DEFAULT 0, Notes NVARCHAR(255), WorkLocation NVARCHAR(50), InsertionDate DATETIME DEFAULT GETDATE())", connection, transaction))
                                    cmd.ExecuteNonQuery();
                            }

                            // --- CHANGE 2: Downtime table is now simpler ---
                            if (!tableExists("Downtime"))
                            {
                                // It no longer needs CompensatedMinutes or IsResolved columns.
                                using (var cmd = new SqlCeCommand("CREATE TABLE Downtime (ID INT PRIMARY KEY IDENTITY, ConfigurationID INT, StartTime DATETIME, EndTime DATETIME, DurationMinutes FLOAT, InsertionDate DATETIME DEFAULT GETDATE())", connection, transaction))
                                    cmd.ExecuteNonQuery();
                            }

                            // --- CHANGE 3: Add the new Compensations junction table ---
                            if (!tableExists("Compensations"))
                            {
                                using (var cmd = new SqlCeCommand("CREATE TABLE Compensations (ID INT PRIMARY KEY IDENTITY, DowntimeID INT, ActivitySessionID INT, CompensatedMinutes FLOAT, Reason NVARCHAR(50), TransactionDate DATETIME DEFAULT GETDATE())", connection, transaction))
                                    cmd.ExecuteNonQuery();

                                // Note: In a full SQL database, you would add Foreign Key constraints here.
                                // SQL CE has limited support, so we manage the relationship in code.
                            }
                            

                            if (!tableExists("FixedPublicHolidays"))
                            {
                                using (var cmd = new SqlCeCommand("CREATE TABLE FixedPublicHolidays (Month INT, Day INT, PRIMARY KEY (Month, Day))", connection, transaction))
                                    cmd.ExecuteNonQuery();
                            }

                            if (!tableExists("MovablePublicHolidays"))
                            {
                                using (var cmd = new SqlCeCommand("CREATE TABLE MovablePublicHolidays (Month INT, Day INT)", connection, transaction))
                                    cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Fatal Error: Could not create database tables.\n\n{ex.Message}", "Infrastructure Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }

                // Holiday population logic remains the same
                try
                {
                    

                    int holidayCount;
                    using (var cmd = new SqlCeCommand("SELECT COUNT(*) FROM FixedPublicHolidays", connection))
                    {
                        holidayCount = (int)cmd.ExecuteScalar();
                    }

                    if (holidayCount == 0)
                    {
                        var holidays = new[] { (1, 1), (1, 11), (5, 1), (7, 30), (8, 14), (8, 20), (8, 21), (11, 6), (11, 18) };
                        foreach (var holiday in holidays)
                        {
                            using (var cmd = new SqlCeCommand("INSERT INTO FixedPublicHolidays (Month, Day) VALUES (@Month, @Day)", connection))
                            {
                                cmd.Parameters.AddWithValue("@Month", holiday.Item1);
                                cmd.Parameters.AddWithValue("@Day", holiday.Item2);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: Could not populate public holidays table.\n\n{ex.Message}", "Infrastructure Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        

       



        public static ConfigurationSet LoadOrCreateConfiguration()
        {
            ConfigurationSet dbConfig = LoadActiveConfiguration();

            // 1. Try Secondary Source: JSON file
            ConfigurationSet jsonConfig = LoadConfigFromJson();
            if (jsonConfig != null)
            {
                
                // Success! Recovered from JSON. Self-heal the database.
                if (dbConfig != null && !dbConfig.HasSameRules(jsonConfig)) SaveNewConfiguration(jsonConfig);

                return jsonConfig;
            }
            // 2. Try Primary Source: Database
            if (dbConfig != null)
            {
                // Success! Self-heal the JSON file to keep it in sync.
                SaveConfigToJson(dbConfig);
                return dbConfig;
            }

            // 3. All sources failed. Trigger setup by returning null.
            return null;
        }



        

        // --- Configuration Set Management (Configurations Table) ---

        /// <summary>
        /// Loads the single currently active configuration set from the database.
        /// </summary>
        public static ConfigurationSet LoadActiveConfiguration()
        {
            try
            {
                using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
                {
                    conn.Open();
                    using (var cmd = new SqlCeCommand("SELECT * FROM Configurations WHERE IsActive = 1", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                double startTimeInDays = (double)reader["WorkStartTime"];
                                double endTimeInDays = (double)reader["WorkEndTime"];
                                double breakStartTimeInDays = (double)reader["BreakStartTime"];
                                double breakEndTimeInDays = (double)reader["BreakEndTime"];

                                return new ConfigurationSet
                                {
                                    ID = (int)reader["ID"],
                                    EffectiveDate = (DateTime)reader["EffectiveDate"],
                                    IsActive = (bool)reader["IsActive"],
                                    UserRole = reader["UserRole"].ToString(),
                                    UserName = reader["UserName"].ToString(),
                                    WorkStartTime = TimeSpan.FromDays(startTimeInDays),
                                    WorkEndTime = TimeSpan.FromDays(endTimeInDays),
                                    BreakStartTime = TimeSpan.FromDays(breakStartTimeInDays),
                                    BreakEndTime = TimeSpan.FromDays(breakEndTimeInDays),
                                    WorkDurationMinutes = (int)reader["WorkDurationMinutes"],
                                    RecipientEmail = reader["RecipientEmail"]?.ToString(),
                                    SenderEmail = reader["SenderEmail"]?.ToString(),
                                    SmtpServer = reader["SmtpServer"]?.ToString(),
                                    StartTimeToleranceMinutes = (int)reader["StartTimeToleranceMinutes"],
                                    EndTimeToleranceMinutes = (int)reader["EndTimeToleranceMinutes"],
                                    LogFilePath = _logFilePath,
                                    FilePath = _filePath
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In a real app, you'd want to log this critical error.
                // For now, we return null to indicate failure.
                Log.Error("CRITICAL: Could not load active configuration. " + ex.Message);
                return null;
            }

            return null; // No active configuration found
        }

        /// <summary>
        /// Saves a new configuration set, deactivating the old one. This creates a version history.
        /// </summary>
        public static void SaveNewConfiguration(ConfigurationSet newConfig)
        {
            newConfig.LogFilePath = _logFilePath;
            newConfig.FilePath = _filePath;
            if (newConfig.NotificationSubject == null)
            {
                newConfig.NotificationSubject = "WOTTracker Notification";
            }
            if (newConfig.NotificationBody == null)
            {
                newConfig.NotificationBody = "Hello {UserName},\n\nThis is a notification from WOTTracker.\n\nBest regards,\nWOTTracker Team";
            }
            if (newConfig.NotificationRecipient == null)
            {
                newConfig.NotificationRecipient = newConfig.RecipientEmail; // Default to RecipientEmail if not set
            }
            if (newConfig.RecipientEmail == null)
            {
                newConfig.RecipientEmail = ""; // Default to empty string if not set
            }
                using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
            {
                conn.Open();
                // Use a transaction to ensure both steps succeed or fail together.
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Deactivate the currently active configuration (if any).
                        using (var cmdDeactivate = new SqlCeCommand("UPDATE Configurations SET IsActive = 0 WHERE IsActive = 1", conn, transaction))
                        {
                            cmdDeactivate.ExecuteNonQuery();
                        }

                        // Step 2: Insert the new configuration as the active one.
                        string sql = @"INSERT INTO Configurations 
                                     (EffectiveDate, IsActive, UserRole, UserName, WorkStartTime, WorkEndTime, BreakStartTime, BreakEndTime, WorkDurationMinutes, RecipientEmail, SenderEmail, SmtpServer, StartTimeToleranceMinutes, EndTimeToleranceMinutes, LogFilePath, FilePath, NotificationSendTime, NotificationRecipient, NotificationSubject, NotificationBody)
                                     VALUES 
                                     (@EffectiveDate, 1, @UserRole, @UserName, @WorkStartTime, @WorkEndTime, @BreakStartTime, @BreakEndTime, @WorkDurationMinutes, @RecipientEmail, @SenderEmail, @SmtpServer, @StartTimeTolerance, @EndTimeTolerance, @LogFilePath, @FilePath, @NotificationSendTime, @NotificationRecipient, @NotificationSubject, @NotificationBody)";

                        using (var cmdInsert = new SqlCeCommand(sql, conn, transaction))
                        {
                            cmdInsert.Parameters.AddWithValue("@EffectiveDate", DateTime.Now);
                            cmdInsert.Parameters.AddWithValue("@UserRole", newConfig.UserRole);
                            cmdInsert.Parameters.AddWithValue("@UserName", newConfig.UserName);
                            cmdInsert.Parameters.AddWithValue("@WorkStartTime", newConfig.WorkStartTime.TotalDays);
                            cmdInsert.Parameters.AddWithValue("@WorkEndTime", newConfig.WorkEndTime.TotalDays);
                            cmdInsert.Parameters.AddWithValue("@BreakStartTime", newConfig.BreakStartTime.TotalDays);
                            cmdInsert.Parameters.AddWithValue("@BreakEndTime", newConfig.BreakEndTime.TotalDays);
                            cmdInsert.Parameters.AddWithValue("@WorkDurationMinutes", (newConfig.WorkEndTime - newConfig.WorkStartTime).TotalMinutes);
                            cmdInsert.Parameters.AddWithValue("@RecipientEmail", newConfig.RecipientEmail ?? (object)DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@SenderEmail", newConfig.SenderEmail ?? (object)DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@SmtpServer", newConfig.SmtpServer ?? (object)DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@StartTimeTolerance", newConfig.StartTimeToleranceMinutes);
                            cmdInsert.Parameters.AddWithValue("@EndTimeTolerance", newConfig.EndTimeToleranceMinutes);
                            cmdInsert.Parameters.AddWithValue("@LogFilePath", newConfig.LogFilePath);
                            cmdInsert.Parameters.AddWithValue("@FilePath", newConfig.FilePath);
                            cmdInsert.Parameters.AddWithValue("@NotificationSendTime", newConfig.NotificationSendTime.TotalDays);
                            cmdInsert.Parameters.AddWithValue("@NotificationRecipient", newConfig.NotificationRecipient ?? (object)DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@NotificationSubject", newConfig.NotificationSubject ?? (object)DBNull.Value);
                            cmdInsert.Parameters.AddWithValue("@NotificationBody", newConfig.NotificationBody ?? (object)DBNull.Value);
                            cmdInsert.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw; // Re-throw the exception so the UI knows something went wrong.
                    }
                    
                }
            }
            SaveConfigToJson(newConfig);
        }

        public static bool HasOrphanedActiveSession()
        {
            // If the database doesn't even exist, there can't be an orphaned session.
            if (!File.Exists(_databasePath)) return false;

            try
            {
                using (var conn = new SqlCeConnection($"Data Source={_databasePath}"))
                {
                    conn.Open();
                    // We just need to know if at least one such row exists.
                    using (var cmd = new SqlCeCommand("SELECT COUNT(*) FROM ActivitySessions WHERE EndTime IS NULL", conn))
                    {
                        return ((int)cmd.ExecuteScalar() > 0);
                    }
                }
            }
            catch
            {
                // If we can't query the table, assume no orphaned session for safety.
                return false;
            }
        }
    }
}