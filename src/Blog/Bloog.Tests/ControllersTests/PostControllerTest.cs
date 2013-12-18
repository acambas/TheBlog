using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bloog.Tests.Base;
using Domain.Post;
using Domain.Tag;
using System.Collections.Generic;
using WebUi.Controllers.MVC;
using Infrastructure.Logging;
using Infrastructure.Mapping;
using Infrastructure.Config._Settings;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using System.Net;
using WebUi.App_Start;
using WebUi.Models.Blog;
using Raven.Client;
using WebUi.Models.RavenDB;
using Moq;
using WebUi.Areas.Admin.Controllers;
using WebUi;
namespace Bloog.Tests.ControllersTests
{
    [TestClass]
    public class PostControllerTest:RavenBaseControllerTest
    {
        //BlogController controller1;
        static string postTitle1 = "Post 1";
        static string postTitle2 = "Post 2";

        string postUrlSlug1 = Infrastructure.Helpers.URLHelper.ToUniqueFriendlyUrl(postTitle1);
        string postUrlSlug2 = Infrastructure.Helpers.URLHelper.ToUniqueFriendlyUrl(postTitle2);


        private async Task<PostController> CreateController()
        {

            PostController controller1 = new PostController(
                new Mock<ILogger>().Object,
                new AutoMapperAdapter(),
                new Mock<IApplicationSettings>().Object);
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
            await  RavenSession.StoreAsync(post2);
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
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<PostViewModel>));
            var viewModel = (IEnumerable<PostViewModel>)result.Model;
            Assert.IsTrue(viewModel.Count() == 2);
        }

        [TestMethod]
        public void Create_View()
        {
            //Arange
            var controller =  CreateController().Result;
            
            //Act
            var result = controller.Create();

            //Assert
            Assert.IsNotNull(result);

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(PostViewModel));
        }

        [TestMethod]
        public async Task Create_Post()
        {
            //Arange
            var controller = await CreateController();
            PostViewModel viewModel = new PostViewModel()
            {
                Title = "Post 3",
                ShortDescription = "asdsaf",
                Description = "asdasfss",
                Tags = new List<string> { "tag1", "tag2", "tag3"}
            };

            //Act
            await controller.Create(viewModel);
         
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
            PostViewModel viewModel = new PostViewModel()
            {

            };

            //Act
            var data = await controller.Create(viewModel);

            //Asert
            Assert.IsInstanceOfType(data, typeof(ViewResult));
            var viewResult = (ViewResult)data;
            Assert.IsInstanceOfType(viewResult.Model, typeof(PostViewModel));
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

            Assert.IsInstanceOfType(viewResult.Model, typeof(PostViewModel));
            var viewModel = (PostViewModel)viewResult.Model;
            Assert.IsTrue(viewModel.Title == postTitle1);
        }

        [TestMethod]
        public async Task Edit_Post()
        {
            //Arange
            var controller1 = await CreateController();

            var viewModel = new PostViewModel()
            {
                Title = "new Post 1",
                UrlSlug = postUrlSlug1,
                Description = "aa",
                ShortDescription = "a",
                Tags = new List<string>{ "tag 5","tag4"}
            };

            // Act
            ActionResult result = await controller1.Edit(viewModel);

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
