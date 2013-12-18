using System.Web.Mvc;

namespace WebUi.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/Admin/
        public ActionResult Index()
        {
            return View();
        }
    }
}