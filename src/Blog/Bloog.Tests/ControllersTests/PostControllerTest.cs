using Bloog.Tests.Base;
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
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebUi.Areas.Admin.Controllers;
using WebUi.Models.Blog;

namespace Bloog.Tests.ControllersTests
{
    [TestClass]
    public class PostControllerTest : RavenBaseControllerTest
    {
        private static string postTitle1 = "Post 1";
        private static string postTitle2 = "Post 2";

        private string postUrlSlug1 = Infrastructure.Helpers.URLHelper.ToUniqueFriendlyUrl(postTitle1);
        private string postUrlSlug2 = Infrastructure.Helpers.URLHelper.ToUniqueFriendlyUrl(postTitle2);

        private async Task<PostController> CreateController()
        {
            PostController controller1 = new PostController(
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
            var controller = CreateController().Result;

            // Act
            ViewResult result = await controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<PostListItemViewModel>));
            var viewModel = (IEnumerable<PostListItemViewModel>)result.Model;
            Assert.IsTrue(viewModel.Count() == 2);
        }

        [TestMethod]
        public async Task Create_View()
        {
            //Arange
            var controller = CreateController().Result;

            //Act
            var result = await controller.Create();

            //Assert
            Assert.IsNotNull(result);

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(CreatePostViewModel));
        }

        [TestMethod]
        public async Task Create_Post()
        {
            //Arange
            var controller = await CreateController();
            CreatePostViewModel viewModel = new CreatePostViewModel()
            {
                Title = "Post 3",
                ShortDescription = "asdsaf",
                Description = "asdasfss",
                Tags = new List<string> { "tag1", "tag2", "tag3" }
            };


            controller.ControllerContext = new ControllerContext(new Mock<HttpContextBase>().Object
                , new RouteData(), controller);

            //Act
            await controller.Create(viewModel, null);

            //Asert
            var findPost = await controller.RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .AnyAsync(m => m.Title == "Post 3");

            Assert.IsTrue(findPost);
        }

        [TestMethod]
        public async Task Create_Post_With_Not_Valid_Args()
        {
            //Arange
            var controller = await CreateController();
            controller.ModelState.AddModelError("key", "model is invalid");
            CreatePostViewModel viewModel = new CreatePostViewModel()
            {
            };

            //Act
            var data = await controller.Create(viewModel, null);

            //Asert
            Assert.IsInstanceOfType(data, typeof(ViewResult));
            var viewResult = (ViewResult)data;
            Assert.IsInstanceOfType(viewResult.Model, typeof(CreatePostViewModel));
        }

        [TestMethod]
        public async Task Edit_open_view_when_id_is_null()
        {
            //Arange
            var controller1 = await CreateController();
            // Act
            ActionResult result = await controller1.Edit("");

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            var viewResult = (HttpStatusCodeResult)result;

            var status = (HttpStatusCodeResult)result;
            Assert.IsTrue(status.StatusCode == (int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Edit_open_view_when_post_doesnt_exist()
        {
            //Arange
            var controller1 = await CreateController();
            // Act
            ActionResult result = await controller1.Edit("123");

            // Assert
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public async Task Edit_open_view_when_post_exists()
        {
            //Arange
            var controller1 = await CreateController();
            // Act
            ActionResult result = await controller1.Edit(postUrlSlug1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(EditPostViewModel));
            var viewModel = (EditPostViewModel)viewResult.Model;
            Assert.IsTrue(viewModel.Title == postTitle1);
        }

        [TestMethod]
        public async Task Edit_Post()
        {
            //Arange
            var controller1 = await CreateController();

            var viewModel = new EditPostViewModel()
            {
                Title = "new Post 1",
                UrlSlug = postUrlSlug1,
                Description = "aa",
                ShortDescription = "a",
                Tags = new List<string> { "tag 5", "tag4" }
            };

            // Act
            ActionResult result = await controller1.Edit(viewModel, null);

            // Assert
            var findEditPost = await controller1.RavenSession.Query<Post>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(5)))
                .FirstAsync(m => m.Title == "new Post 1");
            var count = await controller1.RavenSession.Query<Post>().CountAsync();
            Assert.IsTrue(count == 2);

            Assert.IsNotNull(findEditPost);
            Assert.IsTrue(findEditPost.Description == viewModel.Description);
        }
    }
}