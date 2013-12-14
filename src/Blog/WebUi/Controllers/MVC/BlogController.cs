using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebUi.Models;
using Domain.Post;
using Infrastructure.Config._Settings;
using Infrastructure.Mapping;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Client.Linq;
using Raven.Client;
using WebUi.Models.Blog;
namespace WebUi.Controllers.MVC
{
    [RoutePrefix("Blog")]
    public class BlogController : RavenController
    {

        public BlogController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings)
            : base(logger, mapper, appSettings)
        {

        }


        public async Task<ViewResult> Index()
        {
            var data = await RavenSession.Query<Post>().ToListAsync();
            var viewModel = Mapper.Map<Post, PostViewModel>(data);
            return View(viewModel);
        }


        [Route("{id}", Name = "BlogDetails")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var data = await RavenSession.Query<Post>().FirstOrDefaultAsync(m => m.UrlSlug == id);
            PostViewModel postviewmodel = Mapper.Map<Post, PostViewModel>(data);
            if (postviewmodel == null)
            {
                return HttpNotFound();
            }
            return View(postviewmodel);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (RavenSession!=null)
                {
                    RavenSession.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
