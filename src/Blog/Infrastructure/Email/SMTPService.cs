using Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Email
{
    public class SMTPService : IEmailService
    {
        ILogger log;
        public SMTPService(ILogger log)
        {
            this.log = log;
        }

        public void SendMail(string from, string to, string subject, string body)
        {

            try
            {
                log.Log("email sending start");
                MailMessage message = new MailMessage();
                message.Subject = subject;
                message.Body = body;
                SmtpClient smtp = new SmtpClient();
                smtp.Send(message);
                log.Log(string.Format("email sent to:{0} from:{1}, subject:{2}",to,from,subject));
            }
            catch (EmailException e)
            {
                log.Error("Email sending error", e);
                throw e;
            }
        }
    }
}
