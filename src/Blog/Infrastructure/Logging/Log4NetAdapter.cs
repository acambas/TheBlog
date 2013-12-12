using Infrastructure.Config._Settings;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Logging
{
    public class Log4NetAdapter : ILogger
    {
        private readonly log4net.ILog _log;
        public Log4NetAdapter()
        {
            XmlConfigurator.Configure();
            _log = LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
        public void Log(string message)
        {
            _log.Info(message);
        }

        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Error(string message, Exception e)
        {
            _log.Error(message, e);
        }
    }
}
