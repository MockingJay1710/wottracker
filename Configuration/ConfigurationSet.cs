using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOTTracker.Configuration
{
    public class ConfigurationSet
    {
        public int ID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public string UserRole { get; set; }
        public string UserName { get; set; }
        public TimeSpan WorkStartTime { get; set; }
        public TimeSpan WorkEndTime { get; set; }
        public TimeSpan BreakStartTime { get; set; }
        public TimeSpan BreakEndTime { get; set; }
        public int WorkDurationMinutes { get; set; }
        public int StartTimeToleranceMinutes { get; set; } // Tolerance for downtime
        public int EndTimeToleranceMinutes { get; set; }   // Tolerance for overtime
        public string RecipientEmail { get; set; } // Optional email for notifications
        public string SenderEmail { get; set; } // Optional sender email for notifications
        public string SmtpServer { get; set; } // SMTP server for sending emails
        public string LogFilePath { get; set; }
        public string FilePath { get; set; }
        public TimeSpan NotificationSendTime { get; set; } // Time to send notifications
        public string NotificationRecipient { get; set; } 
        public string NotificationSubject { get; set; } 
        public string NotificationBody { get; set; } // Body of the notification email


        /// <summary>
        /// Compares this configuration set with another to see if their rule values are the same.
        /// It ignores properties like ID, EffectiveDate, and IsActive.
        /// </summary>
        public bool HasSameRules(ConfigurationSet other)
        {
            if (other == null) return false;

            return this.UserRole == other.UserRole &&
                   this.UserName == other.UserName &&
                   this.WorkStartTime == other.WorkStartTime &&
                   this.WorkEndTime == other.WorkEndTime &&
                   this.BreakStartTime == other.BreakStartTime &&
                     this.BreakEndTime == other.BreakEndTime &&
                     this.WorkDurationMinutes == other.WorkDurationMinutes &&
                   this.StartTimeToleranceMinutes == other.StartTimeToleranceMinutes &&
                   this.EndTimeToleranceMinutes == other.EndTimeToleranceMinutes &&
                   this.RecipientEmail == other.RecipientEmail &&
                   this.SenderEmail == other.SenderEmail &&
                   this.SmtpServer == other.SmtpServer &&
                    this.LogFilePath == other.LogFilePath &&
                   this.FilePath == other.FilePath &&
                     this.NotificationSendTime == other.NotificationSendTime &&
                     this.NotificationRecipient == other.NotificationRecipient &&
                        this.NotificationSubject == other.NotificationSubject &&
                        this.NotificationBody == other.NotificationBody;
            // Add any other rule-based properties to the comparison here.
        }

    }
}
