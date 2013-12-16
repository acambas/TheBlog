using Infrastructure.Config._Settings;
using Infrastructure.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Authorization.Mvc;
using WebUi.App_Start;


namespace WebUi.Controllers
{
    public class HomeController : RavenController
    {

        public HomeController( Infrastructure.Logging.ILogger logger,
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