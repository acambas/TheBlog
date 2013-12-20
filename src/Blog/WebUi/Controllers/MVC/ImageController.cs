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
using System.Web;
using System.Web.Mvc;
using WebUi.Models.Blog;
using WebUi.Models.RavenDB;

namespace WebUi.Controllers.MVC
{
    public class ImageController:RavenController
    {
        IImageService imageService;

        public ImageController(Infrastructure.Logging.ILogger logger,
            IMapper mapper,
            IApplicationSettings appSettings,
            IImageService imageService)
            : base(logger, mapper, appSettings)
        {
            this.imageService = imageService;
        }


        [OutputCache(Duration = 15, VaryByParam = "id")]
        public async Task<FileContentResult> Image(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException();
            try
            {
                var imageData = await imageService.GetImageData(id);

                if (imageData == null || imageData.ImageBinaryData == null)
                    throw new Exception();

                return File(imageData.ImageBinaryData, "jpg");
            }
            catch (Exception)
            {
                //Gets some base image from img folder
                string basePath = Server.MapPath("~/img");
                var files = Directory.GetFiles(basePath);
                var blogImagePath = files.First(m => m.Contains("blog-2.jpg"));
                var data = System.IO.File.ReadAllBytes(blogImagePath);
                return File(data, "jpg");
            }

        }

        [HttpPost]
        public async Task Upload(string id, HttpPostedFileBase file)
        {
            if (file != null && file.InputStream != null && !string.IsNullOrEmpty(id))
            {
                AppImage image = new AppImage();
                image.Id = id;

                using (MemoryStream stream = new MemoryStream())
                {
                    file.InputStream.CopyTo(stream);
                    image.ImageBinaryData = stream.ToArray();
                }

                await imageService.StoreImage(image);
            }
        }


        public async Task<ViewResult> Gallery()
        {
            var images = await imageService.GetAllImageIds();
            return View(images);
        }

    }
}