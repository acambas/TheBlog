using Domain.Image;
using Domain.Post;
using Infrastructure.Config._Settings;
using Infrastructure.Extensions;
using Infrastructure.Mapping;
using Raven.Client;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebUi.Models.Blog;
using WebUi.Models.RavenDB;

namespace WebUi.Controllers.MVC
{
    public class BlogController : RavenController
    {
        IImageService imageService;

        public BlogController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings,
            IImageService imageService)
            : base(logger, mapper, appSettings)
        {
            this.imageService = imageService;
        }

        [Route("Blog")]
        public async Task<ViewResult> Index()
        {
            IList<Post> data = await RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .OrderByDescending(m => m.PostedOn)
                .ToListAsync();
            var viewModel = Mapper.Map<Post, PostViewModel>(data);
            return View(viewModel);
        }

        [OutputCache(Duration = 15, VaryByParam = "id")]
        [Route("Blog/{id}", Name = "ViewPost")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var data = await RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .OrderByDescending(m => m.PostedOn)
                .FirstOrDefaultAsync(m => m.UrlSlug == id);
            PostViewModel postviewmodel = Mapper.Map<Post, PostViewModel>(data);

            if (postviewmodel == null)
            {
                return HttpNotFound();
            }

            return View(postviewmodel);
        }

        [Route("Blog/{tag}/Tag", Name = "PostsByTag")]
        public async Task<ActionResult> PostsByTag(string tag)
        {
            if (tag == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var data = await RavenSession.Query<Post>()
                .Where(m => m.Tags.Any(t => t.UrlSlug == tag))
                .OrderByDescending(m => m.PostedOn)
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .ToListAsync();

            var postviewmodel = Mapper.Map<Post, PostViewModel>(data);

            return View("Index", postviewmodel);
        }

        [Route("Blog/{term}/Term", Name = "PostsByTerm")]
        public async Task<ActionResult> PostsByTerm(string term)
        {
            if (term == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var data = await RavenSession
                .Query<Post, PostsByTitleIndex>()
                .Where(m => m.Title.StartsWith(term))
                .OrderByDescending(m => m.PostedOn)
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .ToListAsync();
            ;

            var postviewmodel = Mapper.Map<Post, PostViewModel>(data);

            return View("Index", postviewmodel);
        }

        [OutputCache(Duration = 15, VaryByParam = "id")]
        [Route("Blog/Image/{id}", Name = "BlogImage")]
        public async Task<ActionResult> Image(string id)
        {

            try
            {
                var imageData = imageService.GetImageData(id);
                if (imageData == null || imageData.ImageBinaryData == null)
                {
                    throw new Exception();
                }
                return File(imageData.ImageBinaryData, "jpg");
            }
            catch (Exception)
            {
                string basePath = Server.MapPath("~/img");
                var files = Directory.GetFiles(basePath);
                var blogImagePath = files.First(m => m.Contains("blog-2.jpg"));
                var data = System.IO.File.ReadAllBytes(blogImagePath);
                return File(data, "jpg");
            }

        }

        [OutputCache(Duration = 15, VaryByParam = "none")]
        [ChildActionOnly]
        public PartialViewResult BlogPageData()
        {
            BlogPageDataViewModel viewModel = new BlogPageDataViewModel();

            //Get Tag Count
            var queryTagCount = RavenSession.Query<TagCountIndex.ReduceResult, TagCountIndex>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)));
            var dataTagCount = AppTaskExtensions.RunAsyncAsSync<IList<TagCountIndex.ReduceResult>>(queryTagCount.ToListAsync);
            viewModel.TagCountList = Mapper.Map<TagCountIndex.ReduceResult, TagCountViewModel>(dataTagCount);

            //Get last 5 post links
            var lastPostsQuery = RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .OrderByDescending(m => m.PostedOn)
                .Select(m => new Post { Title = m.Title, UrlSlug = m.UrlSlug })
                .Take(5);
            var postLinks = AppTaskExtensions.RunAsyncAsSync<IList<Post>>(lastPostsQuery.ToListAsync);
            viewModel.RecentLinkList = Mapper.Map<Post, PostLinkViewModel>(postLinks);

            return PartialView("_BlogPageData", viewModel);
        }

        

    }
}