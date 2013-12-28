using Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace WebUi.App_Start
{
    public class TraceLog:Infrastructure.Logging.ILogger
    {
        public void Log(string message)
        {
            Trace.WriteLine(string.Format("{0} : {1}",DateTime.Now.ToString(),message));
        }

        public void Error(string message)
        {
            Trace.Fail(string.Format("{0} : {1}", DateTime.Now.ToString(), message));
        }

        public void Error(string message, Exception e)
        {
            if (e == null)
            {
                Error(message);
            }
            
            Trace.Fail(string.Format("{0} : {1}", DateTime.Now.ToString(), e.Message));
        }
    }
}