using Infrastructure.Config._Settings;
using Infrastructure.Logging;
using Infrastructure.Mapping;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity.Mvc4;

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
            .RegisterType<Infrastructure.Logging.ILogger, Log4NetAdapter>(new ContainerControlledLifetimeManager())
            .RegisterType<IMapper, AutoMapperAdapter>(new ContainerControlledLifetimeManager())
            ;
        }
    }


}