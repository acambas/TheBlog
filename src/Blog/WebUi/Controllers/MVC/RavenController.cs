using DevTrends.MvcDonutCaching;
using DevTrends.MvcDonutCaching.Annotations;
using Infrastructure.Config._Settings;
using Infrastructure.Mapping;
using Raven.Client;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebUi.Controllers
{
    public abstract class RavenController : Controller
    {
        protected Infrastructure.Logging.ILogger Logger { get; set; }

        protected IMapper Mapper { get; set; }

        protected IApplicationSettings AppSettings { get; set; }

        public IAsyncDocumentSession RavenSession { get; set; }

        public RavenController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings)
        {
            Logger = logger;
            Mapper = mapper;
            AppSettings = appSettings;
            CacheManager = new OutputCacheManager();
        }


        protected OutputCacheManager CacheManager { get; set; }

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

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            using (RavenSession)
            {
                if (filterContext.Exception != null)
                    return;

                //if (RavenSession != null)
                //    AppTaskExtensions.RunAsyncAsSync(RavenSession.SaveChangesAsync);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (RavenSession != null)
                {
                    RavenSession.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}