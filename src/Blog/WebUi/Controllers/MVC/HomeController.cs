using Infrastructure.Config._Settings;
using Infrastructure.Mapping;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching.Annotations;
using DevTrends.MvcDonutCaching;
using WebUi.Models.RavenDB;
using System;
using Raven.Client;
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
        [OutputCache(CacheProfile = "StaticPage")]
        public ActionResult Index()
        {
            
            return View();
        }

        //[ClaimsAuthorize(AppAuthorizationType.RoleAuth, AppRoles.Edit)]
        [Route("About")]
        [DonutOutputCache(CacheProfile = "StaticPage")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }


        [Route("CV")]
        [DonutOutputCache(CacheProfile = "StaticPage")]
        public ActionResult CV()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        //[ClaimsAuthorize(AppAuthorizationType.RoleAuth)]
        //[Authorize]
        [DonutOutputCache(CacheProfile = "StaticPage")]
        [Route("Contact")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}