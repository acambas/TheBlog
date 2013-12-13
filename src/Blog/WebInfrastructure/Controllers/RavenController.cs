using Infrastructure.Mapping;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebInfrastructure.Db;

namespace WebInfrastructure.Controllers
{
    public class RavenController:Controller
    {
        protected Infrastructure.Logging.ILogger Logger { get; set; }
        protected IMapper Mapper { get; set; }
        public IAsyncDocumentSession RavenSession { get; protected set; }

        public RavenController(Infrastructure.Logging.ILogger logger,
            IMapper mapper)
        {
            Logger = logger;
            Mapper = mapper;
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = RavenBootstrap.Store.OpenAsyncSession();
        }

        protected virtual async Task SaveAsync()
        {
            using (RavenSession)
            {
                if (RavenSession != null)
                    await RavenSession.SaveChangesAsync();
            }
        }



    }
}
