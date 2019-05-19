using JelaCoreLib.Extension;
using JelaCoreLib.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JelaCoreLib.Service
{
    /// <summary>
    /// This class is used to send emails.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// Name or IP address of the host used for SMTP transactions.
        /// </summary>
        private readonly string _smtpServer;
        /// <summary>
        /// Port that is used by the SMTP Server
        /// </summary>
        private readonly int _smtpPort;
        /// <summary>
        /// Specifies whether the SmtpClient uses SSL to encrypt the connection.
        /// </summary>
        private readonly bool _enableSsl;
        /// <summary>
        /// Email address of the sender.
        /// </summary>
        private readonly MailAddress _emailSender;
        /// <summary>
        /// The credentials used to authenticate the sender at SMTP.
        /// </summary>
        private readonly NetworkCredential _smtpCredential;

        /// <summary>
        /// Create new instance of the email sender.
        /// </summary>
        /// <param name="smtpServer">Name or IP address of the host used for SMTP transactions.</param>
        /// <param name="smtpPort">Port that is used by the SMTP Server.</param>
        /// <param name="smtpUser">Username that is used to authenticate the user on SMTP Server.</param>
        /// <param name="smtpPass">Passphrase that is used to authenticate the user on SMTP Server.</param>
        /// <param name="emailSender">Email address of the sender.</param>
        /// <param name="enableSsl">Specify whether the SmtpClient uses SSL to encrypt the connection.</param>
        public EmailSender(string smtpServer, int smtpPort, string smtpUser, string smtpPass, string emailSender, bool enableSsl)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _enableSsl = enableSsl;

            _emailSender = new MailAddress(emailSender);
            _smtpCredential = new NetworkCredential(smtpUser, smtpPass);
        }

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="emailRecipient">Email address of the email recipient.</param>
        /// <param name="subject">Subject that will be given to the message.</param>
        /// <param name="message">Body of the message.</param>
        /// <param name="ccList">Comma separated string that consists of CC recipients of the message.</param>
        /// <param name="bccList">Comma separated string that consists of BCC recipients of the message.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if SMTP Client cannot be linitialized with given details.</exception>
        /// <exception cref="ArgumentNullException">Thrown if required parameter is null value.</exception>
        /// <exception cref="FormatException">Thrown if email address of sender or recipient is invalid. (CC and BCC are skipped if error)</exception>
        /// <exception cref="SmtpException">Thrown if connection to the SMTP server failed, authentication failed or the operation timed out.</exception>
        /// <exception cref="SmtpFailedRecipientException">Thrown if the message could not be delivered to the recipients.</exception>
        /// <returns>Task with info of status of sending email.</returns>
        public Task SendEmailAsync(string emailRecipient, string subject, string message, string ccList = "", string bccList = "")
        {
            List<string> listCC = null;
            List<string> listBCC = null;

            //First convert to list.
            if (!string.IsNullOrWhiteSpace(ccList))
                listCC = ccList.Split(',').ToList();
            if (!string.IsNullOrWhiteSpace(bccList))
                listBCC = bccList.Split(',').ToList();

            //Then return from the actual implementation.
            return SendEmailAsync(emailRecipient, subject, message, listCC, listBCC);
        }

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="emailRecipient">Email address of the email recipient.</param>
        /// <param name="subject">Subject that will be given to the message.</param>
        /// <param name="message">Body of the message.</param>
        /// <param name="ccList">List of CC recipients of the message.</param>
        /// <param name="bccList">List of BCC recipients of the message.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if SMTP Client cannot be linitialized with given details.</exception>
        /// <exception cref="ArgumentNullException">Thrown if required parameter is null value.</exception>
        /// <exception cref="FormatException">Thrown if email address of sender or recipient is invalid. (CC and BCC are skipped if error)</exception>
        /// <exception cref="SmtpException">Thrown if connection to the SMTP server failed, authentication failed or the operation timed out.</exception>
        /// <exception cref="SmtpFailedRecipientException">Thrown if the message could not be delivered to the recipients.</exception>
        /// <returns>Task with info of status of sending email.</returns>
        public Task SendEmailAsync(string emailRecipient, string subject, string message, List<string> ccList = null, List<string> bccList = null)
        {
            //Initialize client.
            SmtpClient client = new SmtpClient(_smtpServer, _smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = _smtpCredential,
                EnableSsl = _enableSsl
            };

            //Initialize message.
            MailMessage mail = new MailMessage(_emailSender, new MailAddress(emailRecipient))
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            //Add CC's if any.
            foreach (var ccEmail in ccList.OrEmptyIfNull())
            {
                try
                {
                    mail.CC.Add(new MailAddress(ccEmail));
                }
                catch (Exception e)
                {
                    //Simply skip since the email wasnt valid.
                    Console.WriteLine(e.Message);
                    continue;
                }
            }

            //Add BCC's if any.
            foreach (var bccEmail in bccList.OrEmptyIfNull())
            {
                try
                {
                    mail.Bcc.Add(new MailAddress(bccEmail));
                }
                catch (Exception e)
                {
                    //Simply skip since the email wasnt valid.
                    Console.WriteLine(e.Message);
                    continue;
                }
            }

            //And send message with client.
            client.Send(mail);

            //Return info of completed task.
            return Task.CompletedTask;
        }
    }
}
