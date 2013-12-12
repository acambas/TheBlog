using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config._Settings
{
    public interface IApplicationSettings
    {
        string LoggerName { get; }
        string ConnectionStringName { get; }
        string NumberOfResultsPerPage { get; }
    }
}
