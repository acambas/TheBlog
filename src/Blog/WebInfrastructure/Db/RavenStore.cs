using Infrastructure.Database_config;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebInfrastructure.Db
{


    public sealed class RavenBootstrap : IBootstrapDatabase
    {
        private static readonly RavenBootstrap instance = new RavenBootstrap();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RavenBootstrap()
        {
        }

        private RavenBootstrap()
        {
        }

        public static RavenBootstrap Instance
        {
            get
            {
                return instance;
            }
        }

        public void BootstrapDatabase()
        {
            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName("RavenDB");
            parser.Parse();

            Store = new DocumentStore
            {
                ApiKey = parser.ConnectionStringOptions.ApiKey,
                Url = parser.ConnectionStringOptions.Url,
            };
            Store = new EmbeddableDocumentStore() { RunInMemory = true};
 
            Store.Initialize();
            var asembly = Assembly.GetCallingAssembly();
            IndexCreation.CreateIndexes(asembly, Store);
        }

        public DocumentStore Store;

    }
}
