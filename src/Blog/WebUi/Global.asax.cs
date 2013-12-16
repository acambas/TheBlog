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

using Raven.Client.Embedded;

using Domain;
using Domain.Post;
using Domain.Tag;
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
