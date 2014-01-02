using Autofac;
using Autofac.Integration.Mvc;
using Domain.Image;
using Infrastructure.Config._Settings;
using Infrastructure.Logging;
using Infrastructure.Mapping;
using System.Reflection;
using System.Web.Mvc;
using WebUi.Models.RavenDB;

namespace WebUi.App_Start
{
    public static class IocConfig
    {
        public static void Initialise()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<WebConfigApplicationSettings>().As<IApplicationSettings>().InstancePerHttpRequest();
            builder.RegisterType<TraceLog>().As<ILogger>().InstancePerHttpRequest();
            builder.RegisterType<StoreOnRavenDBImageService>().As<IImageService>().InstancePerHttpRequest();
            builder.RegisterType<AutoMapperAdapter>().As<IMapper>().InstancePerHttpRequest();
            builder.RegisterFilterProvider();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}