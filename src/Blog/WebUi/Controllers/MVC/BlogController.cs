using DevTrends.MvcDonutCaching;
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
using System.Web.UI;
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
        [DonutOutputCache(CacheProfile = "BlogPage")]
        public async Task<ViewResult> Index()
        {
            Logger.Log("Blog index start");

            IList<Post> data = await RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .OrderByDescending(m => m.PostedOn)
                .ToListAsync();

            Logger.Log("Blog page index data received");

            var viewModel = Mapper.Map<Post, PostViewModel>(data);

            Logger.Log("Blog page index data mapped to view model");
            Logger.Log("Blog page index sent");
            return View(viewModel);
        }

        [DonutOutputCache(CacheProfile = "BlogPost")]
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

        [DonutOutputCache(CacheProfile = "BlogPage")]
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

        [DonutOutputCache(CacheProfile = "BlogPage")]
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

        
        //Child actions---------------------------------------------
        [DonutOutputCache(CacheProfile = "BlogPageData")]
        [ChildActionOnly]
        [Authorize]
        public PartialViewResult BlogPageData()
        {
            Logger.Log("Blog page DATA start");
            BlogPageDataViewModel viewModel = new BlogPageDataViewModel();
            using (var session = MvcApplication.Store.OpenSession())
            {
                //Get last 5 post links
                var postLinks1 = session.Query<Post>()
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                    .OrderByDescending(m => m.PostedOn)
                    .Take(5)
                    .Select(m => new Post { Title = m.Title, UrlSlug = m.UrlSlug })
                    .ToList();

                var postLinks = postLinks1.Select(m => new Post { Title = m.Title, UrlSlug = m.UrlSlug });

                Logger.Log("Blog page DATA link data received");
                viewModel.RecentLinkList = Mapper.Map<Post, PostLinkViewModel>(postLinks);

                var dataTagCount = session.Query<TagCountIndex.ReduceResult, TagCountIndex>()
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5))).ToList();
                Logger.Log("Blog page DATA tag data received");

                viewModel.TagCountList = Mapper.Map<TagCountIndex.ReduceResult, TagCountViewModel>(dataTagCount);


                return PartialView("_BlogPageData", viewModel);
            }

        }

    }
}