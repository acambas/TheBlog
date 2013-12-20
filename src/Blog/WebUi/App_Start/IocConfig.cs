using Domain.Image;
using Infrastructure.Config._Settings;
using Infrastructure.Logging;
using Infrastructure.Mapping;
using Microsoft.Practices.Unity;
using System.Web.Mvc;
using Unity.Mvc4;
using WebUi.Models.RavenDB;

namespace WebUi.App_Start
{
    public static class IocConfig
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container
            .RegisterType<IApplicationSettings, WebConfigApplicationSettings>(new ContainerControlledLifetimeManager())
            .RegisterType<Infrastructure.Logging.ILogger, RavenLog>(new ContainerControlledLifetimeManager())
            .RegisterType<IImageService, StoreOnRavenDBImageService>(new ContainerControlledLifetimeManager())
            .RegisterType<IMapper, AutoMapperAdapter>(new ContainerControlledLifetimeManager())
            ;
        }
    }
}