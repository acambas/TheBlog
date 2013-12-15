﻿using System;
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
using WebUi.Models.RavenDB;
using Infrastructure.Extensions;


namespace WebUi.Controllers.MVC
{

    public class BlogController : RavenController
    {

        public BlogController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings)
            : base(logger, mapper, appSettings)
        { }

        [Route("Blog")]
        public async Task<ViewResult> Index()
        {
            IList<Post> data = await RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5))).ToListAsync();
            var viewModel = Mapper.Map<Post, PostViewModel>(data);
            return View(viewModel);
        }


        [Route("Blog/{id}", Name = "ViewPost")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var data = await RavenSession.Query<Post>()
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
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .ToListAsync();
                ;

            var postviewmodel = Mapper.Map<Post, PostViewModel>(data);

            return View("Index", postviewmodel);
        }

        [ChildActionOnly]
        public PartialViewResult BlogPageData()
        {
            BlogPageDataViewModel viewModel = new BlogPageDataViewModel();

            //Get Tag Count
            var queryTagCount =  RavenSession.Query<TagCountIndex.ReduceResult, TagCountIndex>()
            .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)));
            var dataTagCount = AppTaskExtensions.RunAsyncAsSync<IList<TagCountIndex.ReduceResult>>(queryTagCount.ToListAsync);
            viewModel.TagCountList = Mapper.Map<TagCountIndex.ReduceResult, TagCountViewModel>(dataTagCount);

            //Get last 5 post links
            var lastPostsQuery = RavenSession.Query<Post>()
            .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
            .Select(m => new Post { Title = m.Title, UrlSlug = m.UrlSlug})
            .Take(5);
            var postLinks = AppTaskExtensions.RunAsyncAsSync<IList<Post>>(lastPostsQuery.ToListAsync);
            viewModel.RecentLinkList = Mapper.Map<Post, PostLinkViewModel>(postLinks);

            return PartialView("_BlogPageData", viewModel);
        }

    }
}
