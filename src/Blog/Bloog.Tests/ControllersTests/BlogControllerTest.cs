﻿using Bloog.Tests.Base;
using Domain.Image;
using Domain.Post;
using Domain.Tag;
using Infrastructure.Config._Settings;
using Infrastructure.Logging;
using Infrastructure.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebUi.Controllers.MVC;
using WebUi.Models.Blog;

namespace Bloog.Tests.ControllersTests
{
    [TestClass]
    public class BlogControllerTest : RavenBaseControllerTest
    {
        private static string postTitle1 = "Post 1";
        private static string postTitle2 = "Post 2";

        private string postUrlSlug1 = Infrastructure.Helpers.URLHelper.ToUniqueFriendlyUrl(postTitle1);
        private string postUrlSlug2 = Infrastructure.Helpers.URLHelper.ToUniqueFriendlyUrl(postTitle2);

        private async Task<BlogController> CreateController()
        {
            BlogController controller1 = new BlogController(
                new Mock<ILogger>().Object,
                new AutoMapperAdapter(),
                new Mock<IApplicationSettings>().Object,
                new Mock<IImageService>().Object);
            controller1.RavenSession = RavenSession;

            var post1 = new Post()
            {
                Title = postTitle1,
                ShortDescription = "asdsaf",
                Description = "asdasfss",
                UrlSlug = postUrlSlug1,
                LastEdit = DateTime.Now,
                PostedOn = DateTime.Now,
                Published = true,
                Tags = new List<Tag> { new Tag { Name = "mvc", UrlSlug = "mvc" } }
            };

            var post2 = new Post()
            {
                Title = postTitle2,
                ShortDescription = "asdaf",
                Description = "asdasfs",
                UrlSlug = postUrlSlug2,
                LastEdit = DateTime.Now,
                PostedOn = DateTime.Now,
                Published = true,
                Tags = new List<Tag> { new Tag { Name = "it", UrlSlug = "it" }, new Tag { Name = "mvc", UrlSlug = "mvc" } }
            };

            await RavenSession.StoreAsync(post1);
            await RavenSession.StoreAsync(post2);
            await RavenSession.SaveChangesAsync();
            controller1.RavenSession = RavenSession;

            return controller1;
        }

        [TestInitialize]
        public void SetUpDB()
        {
            SetUp();
        }

        [TestCleanup]
        public void CleanTest()
        {
            CleanUp();
        }

        [TestMethod]
        public async Task Index()
        {
            //Arange
            var controller1 = CreateController().Result;
            // Act
            ViewResult result = await controller1.Index();
            var data = await controller1.RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5))).ToListAsync();
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<PostViewModel>));
            var viewModel = (IEnumerable<PostViewModel>)result.Model;
            Assert.IsTrue(viewModel.Count() == 2);
        }



        [TestMethod]
        public async Task Details_when_id_exists()
        {
            //Arange
            var controller1 = CreateController().Result;
            // Act
            ActionResult result = await controller1.Details(postUrlSlug1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(PostViewModel));

            var viewModel = (PostViewModel)viewResult.Model;

            Assert.IsTrue(viewModel.Title == postTitle1);
        }

        [TestMethod]
        public async Task Details_when_id_doesnt_exists()
        {
            //Arange
            var controller1 = await CreateController();
            // Act
            ActionResult result = await controller1.Details("test");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public async Task Details_when_id_is_null()
        {
            //Arange
            var controller1 = await CreateController();
            // Act
            ActionResult result = await controller1.Details(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));

            var status = (HttpStatusCodeResult)result;
            Assert.IsTrue(status.StatusCode == (int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Search_By_Tag()
        {
            //Arange
            var controller1 = CreateController().Result;
            // Act
            ActionResult result1 = await controller1.PostsByTag("it");
            ActionResult result2 = await controller1.PostsByTag("mvc");
            // Assert

            Assert.IsInstanceOfType(result1, typeof(ViewResult));
            var viewResult = (ViewResult)result1;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<PostViewModel>));

            var viewModel1 = (IEnumerable<PostViewModel>)viewResult.Model;
            var viewModel2 = (IEnumerable<PostViewModel>)((ViewResult)result2).Model;
            Assert.IsTrue(viewModel1.FirstOrDefault(m => m.Title == postTitle2) != null);
            Assert.IsTrue(viewModel2.Count() == 2);
        }

        [TestMethod]
        public async Task Search_By_Tag_when_tag_is_null()
        {
            //Arange
            var controller1 = await CreateController();
            // Act
            ActionResult result = await controller1.PostsByTag(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            var viewResult = (HttpStatusCodeResult)result;

            var status = (HttpStatusCodeResult)result;
            Assert.IsTrue(status.StatusCode == (int)HttpStatusCode.BadRequest);
        }

        //[TestMethod]
        //public async Task Search_By_Term()
        //{
        //    //Arange
        //    var controller1 = await CreateController();
        //    // Act
        //    ActionResult result1 = await controller1.PostsByTerm("Pos");
        //    // Assert

        //    Assert.IsInstanceOfType(result1, typeof(ViewResult));
        //    var viewResult = (ViewResult)result1;
        //    Assert.IsNotNull(viewResult);
        //    Assert.IsNotNull(viewResult.Model);
        //    Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<PostViewModel>));

        //    var viewModel1 = (IEnumerable<PostViewModel>)viewResult.Model;
        //    Assert.IsTrue(viewModel1.FirstOrDefault(m => m.Title == postTitle1) != null);
        //}

        [TestMethod]
        public async Task Search_By_Tag_when_term_is_null()
        {
            //Arange
            var controller1 = await CreateController();
            // Act
            ActionResult result = await controller1.PostsByTerm(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            var viewResult = (HttpStatusCodeResult)result;

            var status = (HttpStatusCodeResult)result;
            Assert.IsTrue(status.StatusCode == (int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Get_Blog_Page_Data()
        {
            //Arange
            var controller1 = await CreateController();
            // Act

            PartialViewResult result = controller1.BlogPageData();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(BlogPageDataViewModel));
            var viewModel = (BlogPageDataViewModel)result.Model;

            // Assert
            Assert.IsTrue(viewModel.TagCountList.Count() == 2);

            Assert.IsTrue(viewModel.TagCountList.FirstOrDefault(m => m.Name == "mvc").Count == 2);
            Assert.IsTrue(viewModel.RecentLinkList.Count() == 2);
        }
    }
}