﻿using Domain.Image;
using Domain.Post;
using Domain.Tag;
using Infrastructure.Config._Settings;
using Infrastructure.Helpers;
using Infrastructure.Mapping;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebUi.Controllers;
using WebUi.Models.Blog;
using WebUi.Models.RavenDB;

namespace WebUi.Areas.Admin.Controllers
{
    [Authorize]
    public class PostController : RavenController
    {
        IImageService imageService;

        public PostController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings,
            IImageService imageService)
            : base(logger, mapper, appSettings)
        {
            this.imageService = imageService;
        }

        //private WebUiContext db = new WebUiContext();

        public async Task<ViewResult> Index()
        {
            var data = await RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .OrderByDescending(m => m.PostedOn)
                .Where(m => m.Active == true)
                .ToListAsync();
            var viewModel = Mapper.Map<Post, PostListItemViewModel>(data);
            return View(viewModel);
        }

        public async Task<ViewResult> Create()
        {
            CreatePostViewModel viewModel = new CreatePostViewModel();

            var dataTagCount = await RavenSession.Query<TagCountIndex.ReduceResult, TagCountIndex>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .ToListAsync();
            viewModel.FillAllTags(dataTagCount.Select(m => new TagViewModel() { Name = m.Name, UrlSlug = m.UrlSlug }));

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Title,ShortDescription,Description,Tags")] CreatePostViewModel postviewmodel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                postviewmodel.Tags = handleRecivedTags(postviewmodel.Tags);
                var data = Mapper.Map<CreatePostViewModel, Post>(postviewmodel);
                data.PostedOn = DateTime.Now;
                data.UrlSlug = URLHelper.ToUniqueFriendlyUrl(data.Title);

                try
                {
                    data.User = ControllerContext.HttpContext.User.Identity.Name;
                }
                catch (Exception){ }
                
                
                if (file != null)
                {
                    var id = Guid.NewGuid().ToString();
                    imageService.StoreImage(id, file.InputStream);
                    data.ImageId = id;
                }


                await RavenSession.StoreAsync(data);
                await SaveAsync();
                return RedirectToAction("Index");
            }

            var dataTagCount = await RavenSession.Query<TagCountIndex.ReduceResult, TagCountIndex>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .ToListAsync();
            postviewmodel.FillAllTags(dataTagCount.Select(m => new TagViewModel() { Name = m.Name, UrlSlug = m.UrlSlug }));

            return View(postviewmodel);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get Post Data
            var postData = await RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstOrDefaultAsync(m => m.UrlSlug == id);
            var postviewmodel = Mapper.Map<Post, EditPostViewModel>(postData);
            
            if (postviewmodel == null)
            {
                return HttpNotFound();
            }
            //Get All Tags
            var dataTagCount = await RavenSession.Query<TagCountIndex.ReduceResult, TagCountIndex>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .ToListAsync();
            postviewmodel.FillAllTags(dataTagCount.Select(m => new TagViewModel() { Name = m.Name, UrlSlug = m.UrlSlug }));


            return View(postviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Title,ShortDescription,Description,UrlSlug,Tags")] EditPostViewModel postviewmodel)
        {
            if (ModelState.IsValid)
            {
                postviewmodel.Tags = handleRecivedTags(postviewmodel.Tags);
                var data = await RavenSession.Query<Post>().FirstAsync(m => m.UrlSlug == postviewmodel.UrlSlug);
                var editData = Mapper.Map<EditPostViewModel, Post>(postviewmodel, data);
                await RavenSession.StoreAsync(editData);
                await SaveAsync();
                return RedirectToAction("Index");
            }
            //Get All Tags
            var dataTagCount = await RavenSession.Query<TagCountIndex.ReduceResult, TagCountIndex>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .ToListAsync();
            postviewmodel.FillAllTags(dataTagCount.Select(m => new TagViewModel() { Name = m.Name, UrlSlug = m.UrlSlug }));
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


        public async Task<JsonResult> GetAllTags()
        {
            var dataTagCount = await RavenSession.Query<TagCountIndex.ReduceResult, TagCountIndex>()
               .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
               .ToListAsync();
            var tags = Mapper.Map<TagCountIndex.ReduceResult, TagCountViewModel>(dataTagCount)
                .Select(m => m.Name);

            return Json(tags, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<string> handleRecivedTags(IEnumerable<string> tags)
        {
            List<string> result = new List<string>();

            if (tags == null || tags.Count() == 0)
                return result;   

            if (tags.Count()>1 || !tags.First().Contains(","))
            {
                return tags;
            }

            var splitTags = tags.First().Split(',');
            result = splitTags.ToList();

            return result;
        }
    }
}