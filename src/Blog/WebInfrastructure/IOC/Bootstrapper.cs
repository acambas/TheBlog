using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using Infrastructure.Config._Settings;
using Infrastructure.Logging;
using Infrastructure.Mapping;

namespace WebInfrastructure.IOC
{
  public static class Bootstrapper
  {
      public static void Initialise()
      {
          var container = BuildUnityContainer();

          DependencyResolver.SetResolver(new UnityDependencyResolver(container));

      }

      private static IUnityContainer BuildUnityContainer()
      {
          var container = new UnityContainer();

          // register all your components with the container here
          // it is NOT necessary to register your controllers

          // e.g. container.RegisterType<ITestService, TestService>();    
          RegisterTypes(container);

          return container;
      }

    public static void RegisterTypes(IUnityContainer container)
    {
        container

        .RegisterType<IApplicationSettings, WebConfigApplicationSettings>(new ContainerControlledLifetimeManager())
        .RegisterType<ILogger, Log4NetAdapter>(new ContainerControlledLifetimeManager())
        .RegisterType<IMapper, AutoMapperAdapter>(new ContainerControlledLifetimeManager())
        ;
    }
  }
}