using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;


namespace WOTTracker
{
    public class StartupManager
    {
        public static void SetStartup(bool enable)
        {
            string appPath = Application.ExecutablePath;
            string appName = Path.GetFileNameWithoutExtension(appPath); // Automatically get the application's name

            try
            {
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (registryKey == null)
                    {
                        MessageBox.Show("Failed to open registry key.");
                        return;
                    }

                    if (enable)
                    {
                        string existingValue = registryKey.GetValue(appName) as string;
                        if (existingValue != appPath)
                        {
                            registryKey.SetValue(appName, appPath);
                        }
                    }
                    else
                    {
                        if (registryKey.GetValue(appName) != null)
                        {
                            registryKey.DeleteValue(appName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}



