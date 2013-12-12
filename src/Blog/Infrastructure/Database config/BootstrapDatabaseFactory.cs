using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database_config
{
    public class BootstrapDatabaseFactory
    {
        private static IBootstrapDatabase _bootstrapDatabase;

        public static void InitializeBootstrapDatabase(IBootstrapDatabase bootstrapDatabase)
        {
            _bootstrapDatabase = bootstrapDatabase;
        }

        public static IBootstrapDatabase Get()
        {
            if (_bootstrapDatabase == null)
            {
                _bootstrapDatabase = new NullBootstrapDatabase();
            }
            return _bootstrapDatabase;
        }

    }
}
