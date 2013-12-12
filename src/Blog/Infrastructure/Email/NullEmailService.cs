
using Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Email
{
    public class NullEmailService : IEmailService
    {


        #region IEmailService Members

        public void SendMail(string from, string to, string subject, string body)
        {

        }

        #endregion
    }
}
