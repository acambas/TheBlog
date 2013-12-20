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
        public async Task<ActionResult> Image(string id)
        {
            try
            {
                var imageData = await Task<byte[]>.Run(() => imageService.GetImageData(id));
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

        [HttpPost]
        public async Task Upload(string id, HttpPostedFileBase file)
        {
            if (file != null && !string.IsNullOrEmpty(id))
            {
                imageService.StoreImage(id, file.InputStream);
                await Task.Run(() => imageService.StoreImage(id, file.InputStream));
            }
        }

    }
}