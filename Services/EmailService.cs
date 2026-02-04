using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using WOTTracker.Configuration;

namespace WOTTracker.Services
{
    public class EmailService
    {
        private readonly ConfigurationSet _config;

        /// <summary>
        /// The service is initialized with the configuration it needs to operate.
        /// </summary>
        public EmailService(ConfigurationSet activeConfig)
        {
            _config = activeConfig;
        }

        /// <summary>
        /// Sends the "Working from Home" notification email.
        /// </summary>
        public void SendHomeOfficeNotification()
        {
            Log.Information("Preparing to send home office notification email.");

            // Use a try-catch block to handle potential network or configuration errors.
            try
            {
                // Validate that the necessary configuration is present.
                if (string.IsNullOrEmpty(_config.NotificationRecipient) ||
                    string.IsNullOrEmpty(_config.SenderEmail) ||
                    string.IsNullOrEmpty(_config.SmtpServer))
                {
                    Log.Error("Cannot send notification email. Email settings (Recipient, Sender, SmtpServer) are not fully configured.");
                    return;
                }

                // Personalize the email body.
                string body = _config.NotificationBody
                    .Replace("{UserName}", _config.UserName)
                    .Replace("{Date}", DateTime.Today.ToLongDateString());

                // Create the mail message and the SMTP client.
                using (var mail = new MailMessage(_config.SenderEmail, _config.NotificationRecipient, _config.NotificationSubject, body))
                using (var smtp = new SmtpClient(_config.SmtpServer))
                {
                    // Note: You might need to configure Port, SSL, and Credentials here
                    // depending on your email provider. These could also be added to the config.
                    // Example:
                    // smtp.Port = 587;
                    // smtp.EnableSsl = true;
                    // smtp.Credentials = new System.Net.NetworkCredential("user@example.com", "password");

                    smtp.Send(mail);
                    Log.Information("Home office email sent successfully to {Recipient}.", _config.NotificationRecipient);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "A critical error occurred while trying to send the home office email.");
            }
        }

        //public void SendHomeOfficeNotification()
        //{
        //    Log.Information("Preparing to send home office notification email via Gmail.");

        //    try
        //    {
        //        Log.Debug("Checking configuration values...");

        //        if (string.IsNullOrEmpty(_config.NotificationRecipient) ||
        //            string.IsNullOrEmpty(_config.SenderEmail) ||
        //            string.IsNullOrEmpty(_config.SmtpServer))
        //        {
        //            Log.Error("Cannot send notification email. Email settings are not fully configured.");
        //            Log.Debug("Recipient: {Recipient}, Sender: {Sender}, SMTP: {SmtpServer}",
        //                      _config.NotificationRecipient, _config.SenderEmail, _config.SmtpServer);
        //            return;
        //        }

        //        string body = _config.NotificationBody
        //            .Replace("{UserName}", _config.UserName)
        //            .Replace("{Date}", DateTime.Today.ToLongDateString());

        //        Log.Debug("Email body constructed:\n{Body}", body);

        //        var senderEmail = "elmejbarotmane@gmail.com";
        //        var recipientEmail = "otayjoker123@gmail.com";
        //        var subject = _config.NotificationSubject;
        //        var password = ""; // ❗Replace with app password securely

        //        Log.Debug("Creating MailMessage from {From} to {To}...", senderEmail, recipientEmail);
        //        using (var mail = new MailMessage(senderEmail, recipientEmail, subject, body))
        //        using (var smtp = new SmtpClient("smtp.gmail.com", 587))
        //        {
        //            smtp.EnableSsl = true;
        //            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            smtp.UseDefaultCredentials = false;
        //            smtp.Credentials = new System.Net.NetworkCredential(senderEmail, password);

        //            Log.Debug("SMTP client configured. Server: smtp.gmail.com, Port: 587, SSL: true");

        //            Log.Information("Attempting to send email to {Recipient}", recipientEmail);
        //            smtp.Send(mail);
        //            Log.Information("Email sent successfully.");
        //        }
        //    }
        //    catch (SmtpException smtpEx)
        //    {
        //        Log.Error(smtpEx, "SMTP error occurred while trying to send the email.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "General error occurred while trying to send the email.");
        //    }
        //}


    }
}
