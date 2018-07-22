using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace AutomationTestServer.DataObjects
{
    public class EmailHelper
    {
        public static void SendEmail(string recipients, string header, string body, string text)
        {
            string[] results = recipients.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            MailMessage message = new MailMessage();
            foreach (var result in results)
            {
                message.Bcc.Add(result + "@testapp.com");
            }

            message.From = new MailAddress("testautomation@testapp.com");
            message.Subject = header;
            message.IsBodyHtml = false;
            message.Body = body;
            message.Attachments.Add(new Attachment(text));

            SmtpClient client = new SmtpClient("smtprelay.test.testapp");
            client.UseDefaultCredentials = true;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
