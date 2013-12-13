using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebInfrastructure.IOC;
using WebInfrastructure.Db;
using Raven.Client.Embedded;
namespace WebUi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Ioc initialize
            Bootstrapper.Initialise();

            //Handle RavenDB
            RavenDBInit();
            //RavenBootstrap.Instance.BootstrapDatabase();
        }

        public static DocumentStore Store;

        private static void RavenDBInit()
        {
            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName("RavenDB");
            parser.Parse();

            

#if DEBUG
            //Store = new EmbeddableDocumentStore() { RunInMemory = true };
            Store = new EmbeddableDocumentStore {  ConnectionStringName = "RavenDBEmbedded" };
#else
            Store = new DocumentStore { ConnectionStringName = "RavenDB" };
#endif
            
 
            Store.Initialize();
            var asembly = Assembly.GetCallingAssembly();
            IndexCreation.CreateIndexes(asembly, Store);
        }
    }
}
