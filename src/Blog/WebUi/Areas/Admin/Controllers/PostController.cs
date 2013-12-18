using Domain.Post;
using Infrastructure.Config._Settings;
using Infrastructure.Helpers;
using Infrastructure.Mapping;
using Raven.Client;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebUi.Controllers;
using WebUi.Models.Blog;

namespace WebUi.Areas.Admin.Controllers
{
    [Authorize]
    public class PostController : RavenController
    {
        public PostController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings)
            : base(logger, mapper, appSettings)
        { }

        //private WebUiContext db = new WebUiContext();

        public async Task<ViewResult> Index()
        {
            var data = await RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .Where(m => m.Active == true).ToListAsync();
            var viewModel = Mapper.Map<Post, PostListItemViewModel>(data);
            return View(viewModel);
        }

        public ViewResult Create()
        {
            PostViewModel viewModel = new PostViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Title,ShortDescription,Description,Tags")] PostViewModel postviewmodel)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<PostViewModel, Post>(postviewmodel);
                data.PostedOn = DateTime.Now;
                data.UrlSlug = URLHelper.ToUniqueFriendlyUrl(data.Title);
                await RavenSession.StoreAsync(data);
                await SaveAsync();
                return RedirectToAction("Index");
            }

            return View(postviewmodel);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var postData = await RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstOrDefaultAsync(m => m.UrlSlug == id);
            var postviewmodel = Mapper.Map<Post, PostViewModel>(postData);
            if (postviewmodel == null)
            {
                return HttpNotFound();
            }
            return View(postviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Title,ShortDescription,Description,UrlSlug")] PostViewModel postviewmodel)
        {
            if (ModelState.IsValid)
            {
                var data = await RavenSession.Query<Post>().FirstAsync(m => m.UrlSlug == postviewmodel.UrlSlug);
                var editData = Mapper.Map<PostViewModel, Post>(postviewmodel, data);
                await RavenSession.StoreAsync(editData);
                await SaveAsync();
                return RedirectToAction("Index");
            }
            return View(postviewmodel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var data = await RavenSession.Query<Post>().FirstAsync(m => m.UrlSlug == id);
            data.Active = false;
            await RavenSession.StoreAsync(data);
            await SaveAsync();
            return RedirectToAction("Index");
        }
    }
}