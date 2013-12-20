using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUi.Models.RavenDB
{

    public class RavenLogMessage
    {
        public RavenLogMessage()
        {
            Time = DateTime.Now;
        }

        public DateTime Time { get; set; }
        public string Message { get; set; }
    }

    public class RavenLog : Infrastructure.Logging.ILogger
    {
        DocumentStore store = MvcApplication.Store;
        public void Log(string message)
        {
            using (var session = store.OpenSession())
            {
                RavenLogMessage log = new RavenLogMessage() { Message = message };
                session.Store(log);
                session.SaveChanges();
            }
        }

        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, Exception e)
        {
            throw new NotImplementedException();
        }
    }
}