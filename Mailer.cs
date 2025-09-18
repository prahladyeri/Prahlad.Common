/**
 * Mailer.cs
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Prahlad.Common
{
    public static class MailerConfig 
    {
        public static MailAddress From;
        public static string SmtpPassword;
        public static string SmtpHost;
        public static int SmtpPort = 587;
    }

    public static class Mailer
    {

        public static void SendEmail(IEnumerable<string> to,
            string subject, string htmlBody, List<string> attachments = null, 
            IEnumerable<string> cc = null, IEnumerable<string> bcc = null)
        {
            attachments = attachments ?? new List<string>();
            cc = cc ?? new List<string>();
            bcc = bcc ?? new List<string>();
            var fromAddress = MailerConfig.From;

            var smtp = new SmtpClient
            {
                Host = MailerConfig.SmtpHost,
                Port = MailerConfig.SmtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address,MailerConfig.SmtpPassword)
            };

            using (var message = new MailMessage())
            {
                message.From = fromAddress;
                foreach (string email in to)
                {
                    message.To.Add(new MailAddress(email));
                }
                foreach (string email in cc)
                {
                    message.CC.Add(new MailAddress(email));
                }
                foreach (string email in bcc)
                {
                    message.Bcc.Add(new MailAddress(email));
                }
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlBody;

                foreach (string attachmentPath in attachments)
                {
                    string mimeType = MimeTypeHelper.GetMimeType(attachmentPath);
                    Attachment attachment = new Attachment(attachmentPath, mimeType);
                    message.Attachments.Add(attachment);
                }

                smtp.Send(message);
                smtp.Dispose();
            }
        }
    }
}
