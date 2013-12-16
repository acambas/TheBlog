using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config._Settings
{
    public interface IApplicationSettings
    {
        string ConnectionStringName { get; }
        string AdminPassword { get; }
    }
}
