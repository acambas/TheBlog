using Raven.Client.Document;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using WebUi.App_Start;

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

            //Handle Mapping
            MapperConfig.ConfigureMappings();

            //Ioc initialize
            IocConfig.Initialise();

            //Handle RavenDB
            RavenDbConfig.RavenDBInit();
        }

        public static DocumentStore Store;
    }
}