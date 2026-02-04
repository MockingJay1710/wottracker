using System;
using System.Threading;
using System.Windows.Forms;
using Serilog;
using WOTTracker.Configuration;
using WOTTracker.Forms;


namespace WOTTracker
{
    internal static class Program
    {
        /// <summary>
        /// A unique identifier for the application's Mutex. 
        /// Using a GUID ensures this name will not conflict with any other application on the system.
        /// </summary>
        private const string AppMutexName = "WOTTracker-App-Mutex-C9A4A93E-2FDC-4E7A-A4E5-6F7E1A6E7D2B";
        private static Mutex appMutex;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // --- Step 1: Ensure only one instance of the application is running ---
            // We try to create a new Mutex. The 'createdNew' variable will tell us if we were the first.
            appMutex = new Mutex(true, AppMutexName, out bool createdNew);

            if (!createdNew)
            {
                // If we did not create the Mutex, it means another instance already holds it.
                // We should inform the user and exit immediately.
                MessageBox.Show(
                    "WOTTracker is already running.\n\nPlease check the system tray (next to the clock).",
                    "Application Already Running",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return; // Exit the Main method, terminating this new instance.
            }

            // Standard WinForms setup
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // --- Step 2: Ensure the core infrastructure (Database and Tables) exists ---
            // This method MUST be called before any part of the app attempts to query or save data.
            // It will create the .sdf file and all required tables if they are missing.
            ConfigurationManager.EnsureInfrastructureExists();

            // Try to load config from DB or JSON. If both fail, this returns null.
            ConfigurationSet activeConfig = ConfigurationManager.LoadOrCreateConfiguration();

            if (activeConfig == null)
            {
                bool hasOrphanedSession = ConfigurationManager.HasOrphanedActiveSession();

                if (hasOrphanedSession)
                {
                    // Scenario B: Config is lost, but data exists.
                    string message = "Oops! Could not find a valid configuration, but an active work session was found.\n\nPlease reconfigure the application to continue.";
                    using (var notificationForm = new NotificationForm(message, NotificationType.Warning))
                    {
                        notificationForm.ShowDialog();
                    }
                }
                else
                {
                    // Scenario A: Fresh install.
                    string message = "Welcome to WOTTracker! Let's get started by configuring the application.";
                    using (var notificationForm = new NotificationForm(message, NotificationType.Welcome))
                    {
                        notificationForm.ShowDialog();
                    }
                }
                // If not configured, we must show the setup wizard modally (blocking).
                using (var setupForm = new SetupForm())
                {
                    DialogResult result = setupForm.ShowDialog();

                    // If the user cancels the setup, the application cannot proceed.
                    if (result != DialogResult.OK)
                    {
                            // You could use your new form here too!
                            using (var finalMsg = new NotificationForm("Setup was not completed. The application will now exit.", NotificationType.Error))
                            {
                                finalMsg.ShowDialog();
                            }
                            return;
                    }
                        // After setup, we need to load the newly saved config.
                    activeConfig = ConfigurationManager.LoadActiveConfiguration();
                    if (activeConfig == null) {
                            using (var finalMsg = new NotificationForm("A critical error occurred after setup. The application will now exit.", NotificationType.Error))
                            {
                                finalMsg.ShowDialog();
                            }
                            return;
                    }
                    
                }
            }



            string welcomeMessage = $"Welcome {activeConfig.UserName}\nYour configuration has been successfully retrieved.\nThe application will now start tracking your activity.";
            using (var welcomeForm = new NotificationForm(welcomeMessage, NotificationType.Success))
            {
                welcomeForm.ShowDialog();
            }
            // If we reach this point, all checks have passed, and the app is ready to run.
            // We wrap the main application run in a try...finally block to guarantee the Mutex is released.
            try
            {
                Application.Run(new MainForm(activeConfig));
            }
            finally
            {
                // This code will run when MainForm closes, ensuring we release the Mutex
                // so the application can be started again later.
                appMutex.ReleaseMutex();
                appMutex.Close();
                appMutex = null;
            }
        }
        
        

    }
}