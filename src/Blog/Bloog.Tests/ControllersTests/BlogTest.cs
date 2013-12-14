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
namespace Bloog.Tests.ControllersTests
{
    [TestClass]
    public class BlogTest:RavenBaseController
    {
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
            // Arrange
            ILogger log = new Infrastructure.Logging.Fakes.StubILogger() { };
            IMapper map = new Infrastructure.Mapping.AutoMapperAdapter();
            IApplicationSettings settings = new Infrastructure.Config._Settings.Fakes.StubIApplicationSettings();
            BlogController controller = new BlogController(log, map, settings);
            controller.RavenSession = RavenSession;

            MapperConfig.ConfigureMappings();
            var post = new Post()
            {
                Title = "Post 1",
                ShortDescription = "asdasfsa asfasf asf asd asf asf safsa fasfsa saf",
                Description = "asdasfsa asfasf asf asd asf asf safsa fasfsa saf asdasfsa asfasf asf asd asf asf safsa fasfs",
                UrlSlug = "Post-1__1__",
                LastEdit = DateTime.Now,
                PostedOn = DateTime.Now,
                Published = true,
                Tags = new List<Tag> { new Tag { Name = "mvc", UrlSlug = "mvc" } }
            };

            await RavenSession.StoreAsync(post);
            await RavenSession.SaveChangesAsync();
            controller.RavenSession = RavenSession;

            // Act
            ViewResult result = await controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<PostViewModel>));

            var viewModel = (IEnumerable<PostViewModel>)result.Model;
            Assert.IsTrue(viewModel.Count() == 1);
            Assert.IsTrue(viewModel.Any(m => m.Title == post.Title));
        }

        [TestMethod]
        public async Task Details_when_id_exists()
        {
            // Arrange
            ILogger log = new Infrastructure.Logging.Fakes.StubILogger() { };
            IMapper map = new Infrastructure.Mapping.AutoMapperAdapter();
            IApplicationSettings settings = new Infrastructure.Config._Settings.Fakes.StubIApplicationSettings();
            BlogController controller = new BlogController(log, map, settings);
            controller.RavenSession = RavenSession;

            MapperConfig.ConfigureMappings();
            var post = new Post()
            {
                Title = "Post 1",
                ShortDescription = "asdasfsa asfasf asf asd asf asf safsa fasfsa saf",
                Description = "asdasfsa asfasf asf asd asf asf safsa fasfsa saf asdasfsa asfasf asf asd asf asf safsa fasfs",
                UrlSlug = "Post-1__1__",
                LastEdit = DateTime.Now,
                PostedOn = DateTime.Now,
                Published = true,
                Tags = new List<Tag> { new Tag { Name = "mvc", UrlSlug = "mvc" } }
            };

            await RavenSession.StoreAsync(post);
            await RavenSession.SaveChangesAsync();
            controller.RavenSession = RavenSession;

            // Act
            ActionResult result = await controller.Details(post.UrlSlug);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(PostViewModel));

            var viewModel = (PostViewModel)viewResult.Model;
            
            Assert.IsTrue(viewModel.Title == post.Title);
        }

        [TestMethod]
        public async Task Details_when_id_doesnt_exists()
        {
            // Arrange
            ILogger log = new Infrastructure.Logging.Fakes.StubILogger() { };
            IMapper map = new Infrastructure.Mapping.AutoMapperAdapter();
            IApplicationSettings settings = new Infrastructure.Config._Settings.Fakes.StubIApplicationSettings();
            BlogController controller = new BlogController(log, map, settings);
            controller.RavenSession = RavenSession;

            // Act
            ActionResult result = await controller.Details("test");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }


        [TestMethod]
        public async Task Details_when_id_is_null()
        {
            // Arrange
            ILogger log = new Infrastructure.Logging.Fakes.StubILogger() { };
            IMapper map = new Infrastructure.Mapping.AutoMapperAdapter();
            IApplicationSettings settings = new Infrastructure.Config._Settings.Fakes.StubIApplicationSettings();
            BlogController controller = new BlogController(log, map, settings);
            controller.RavenSession = RavenSession;


            // Act
            ActionResult result = await controller.Details(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));

            var status = (HttpStatusCodeResult)result;
            Assert.IsTrue(status.StatusCode == (int)HttpStatusCode.BadRequest);
        }
    }
}
