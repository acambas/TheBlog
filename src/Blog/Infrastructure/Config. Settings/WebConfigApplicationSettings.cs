using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config._Settings
{
    public class WebConfigApplicationSettings : IApplicationSettings
    {
     
        public string ConnectionStringName
        {
            get { return ConfigurationManager.AppSettings["DefaultConnection"]; }
        }

    }
}
