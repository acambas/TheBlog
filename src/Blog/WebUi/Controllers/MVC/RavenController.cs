using Infrastructure.Mapping;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebUi.Controllers
{
    public abstract class RavenController : Controller
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
            RavenSession = MvcApplication.Store.OpenAsyncSession();
        }

        protected virtual async Task SaveAsync()
        {
            using (RavenSession)
            {
                if (RavenSession != null)
                    await RavenSession.SaveChangesAsync();
            }
        }

        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    if (filterContext.IsChildAction)
        //        return;

        //    using (RavenSession)
        //    {
        //        if (filterContext.Exception != null)
        //            return;

        //        if (RavenSession != null)
        //            RavenSession.SaveChangesAsync();
        //    }

        //}
    }
}