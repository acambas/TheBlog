using AspNet.Identity.RavenDB.Stores;
using Domain.Image;
using Domain.Post;
using Domain.Tag;
using Infrastructure.Config._Settings;
using Infrastructure.Helpers;
using Infrastructure.Mapping;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebUi.Controllers;
using WebUi.Models;
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
            [Bind(Include = "Title,ShortDescription,Description,Tags,ImageUrl")] CreatePostViewModel postviewmodel,
            HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                //Handle tahs
                postviewmodel.Tags = handleRecivedTags(postviewmodel.Tags);

                //Map to data object
                var data = Mapper.Map<CreatePostViewModel, Post>(postviewmodel);
                //Handle Date
                data.PostedOn = DateTime.Now;
                //data.UrlSlug = URLHelper.ToUniqueFriendlyUrl(data.Title);

                //Handle user name
                try
                {
                    var UserManager = new UserManager<ApplicationUser>
                            (new RavenUserStore<ApplicationUser>(RavenSession));
                    var user = UserManager.FindByName(ControllerContext.HttpContext.User.Identity.Name);
                    data.UserName = user.UserName;
                    data.WrittenBy = user.Name;
                }
                catch (Exception)
                {
                    data.UserName = "anonymus";
                }
                //Handle image
                if (!string.IsNullOrEmpty(postviewmodel.ImageUrl))
                {
                    data.ImageId = postviewmodel.ImageUrl;
                }
                else
                {
                    var imageId = await handleImageUpload(file);
                    if (!string.IsNullOrEmpty(imageId))
                        data.ImageId = imageId;
                }


                //Save post
                await RavenSession.StoreAsync(data);
                await SaveAsync();
                //Handle cache
                handleCache();

                return RedirectToAction("Index");
            }

            //Fill viewmodel with tags
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

            //Fill viewmodel with tags
            var dataTagCount = await RavenSession.Query<TagCountIndex.ReduceResult, TagCountIndex>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .ToListAsync();
            postviewmodel.FillAllTags(dataTagCount.Select(m => new TagViewModel() { Name = m.Name, UrlSlug = m.UrlSlug }));


            return View(postviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Title,ShortDescription,Description,UrlSlug,Tags,ImageUrl")] EditPostViewModel postviewmodel,
            HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                postviewmodel.Tags = handleRecivedTags(postviewmodel.Tags);
                var data = await RavenSession.Query<Post>().FirstAsync(m => m.UrlSlug == postviewmodel.UrlSlug);
                var oldImageId = data.ImageId;
                var editData = Mapper.Map<EditPostViewModel, Post>(postviewmodel, data);

                //Handle date
                editData.Modified = DateTime.Now;

                //Handle image
                if (file != null && file.InputStream != null && string.IsNullOrEmpty(postviewmodel.ImageUrl))
                {

                    var imageId = await handleImageUpload(file);
                    if (!string.IsNullOrEmpty(imageId))
                        data.ImageId = imageId;

                }
                else if (!string.IsNullOrEmpty(postviewmodel.ImageUrl))
                {
                    data.ImageId = postviewmodel.ImageUrl;
                }
                else
                    data.ImageId = oldImageId;

                await RavenSession.StoreAsync(editData);
                await SaveAsync();

                //Handle post cache
                handleCache(data.UrlSlug);

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
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var data = await RavenSession.Query<Post>().FirstAsync(m => m.UrlSlug == id);
            data.Active = false;
            await RavenSession.StoreAsync(data);
            await SaveAsync();

            //HandleCashe
            handleCache(id);

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

        //Private functions------------------------------------------------
        private IEnumerable<string> handleRecivedTags(IEnumerable<string> tags)
        {
            List<string> result = new List<string>();

            if (tags == null || tags.Count() == 0)
                return result;

            if (tags.Count() > 1 || !tags.First().Contains(","))
            {
                return tags;
            }

            var splitTags = tags.First().Split(',');
            result = splitTags.ToList();

            return result;
        }

        private void handleCache(string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                CacheManager.RemoveItem("Blog", "Details", new { id = id });
            }
            CacheManager.RemoveItem("Blog", "BlogPageData");
            CacheManager.RemoveItem("Blog", "Index");
            CacheManager.RemoveItems("Blog", "PostsByTag");
            CacheManager.RemoveItems("Blog", "PostsByTerm");
        }

        private async Task<string> handleImageUpload(HttpPostedFileBase file)
        {
            if (file != null && file.InputStream != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(file.FileName);

                var id = URLHelper.ToUniqueFriendlyUrl(fileName);

                AppImage image = new AppImage { Id = id };

                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    await file.InputStream.CopyToAsync(stream);
                    image.ImageBinaryData = stream.ToArray();
                }

                await imageService.StoreImage(image);
                var url = Url.Action("Image", "Image", new { Area = "", id = id });
                return url;
            }

            return null;
        }
    }
}