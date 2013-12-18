using Infrastructure.Config._Settings;
using Infrastructure.Mapping;
using System.Web.Mvc;

namespace WebUi.Controllers
{
    public class HomeController : RavenController
    {
        public HomeController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings)
            : base(logger, mapper, appSettings)
        { }

        //[ClaimsAuthorize(AppAuthorizationType.RoleAuth, AppRoles.Read)]
        [Route]
        public ActionResult Index()
        {
            return View();
        }

        //[ClaimsAuthorize(AppAuthorizationType.RoleAuth, AppRoles.Edit)]
        [Route("About")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //[ClaimsAuthorize(AppAuthorizationType.RoleAuth)]
        //[Authorize]
        [Route("Contact")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}