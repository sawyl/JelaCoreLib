using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JelaCoreLib.Service.Interface
{

    /// <summary>
    /// Interface for sending emails.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="emailRecipient">Email address of the email recipient.</param>
        /// <param name="subject">Subject that will be given to the message.</param>
        /// <param name="message">Body of the message.</param>
        /// <param name="ccList">Comma separated string that consists of CC recipients of the message.</param>
        /// <param name="bccList">Comma separated string that consists of BCC recipients of the message.</param>
        /// <returns>Task with info of status of sending email.</returns>
        Task SendEmailAsync(string emailRecipient, string subject, string message, string ccList = "", string bccList = "");

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="emailRecipient">Email address of the email recipient.</param>
        /// <param name="subject">Subject that will be given to the message.</param>
        /// <param name="message">Body of the message.</param>
        /// <param name="ccList">List of CC recipients of the message.</param>
        /// <param name="bccList">List of BCC recipients of the message.</param>
        /// <returns>Task with info of status of sending email.</returns>
        Task SendEmailAsync(string emailRecipient, string subject, string message, List<string> ccList = null, List<string> bccList = null);
    }
}
