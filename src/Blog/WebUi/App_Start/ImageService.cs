using Domain.Image;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebUi.App_Start
{
    public class StoreOnServerImageService : IImageService
    {

        Task IImageService.StoreImage(AppImage image)
        {
            throw new NotImplementedException();
        }

        Task<AppImage> IImageService.GetImageData(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetAllImageIds()
        {
            throw new NotImplementedException();
        }
    }


    public class StoreOnRavenDBImageService : IImageService
    {
        DocumentStore store = WebUi.MvcApplication.Store;

        public async Task<AppImage> GetImageData(string id)
        {
            using (var session = store.OpenSession())
            {
                var dbCommands = session.Advanced.DocumentStore.DatabaseCommands;
                var attachement = dbCommands.GetAttachment(id);

                AppImage image = new AppImage() { Id = id };

                var dataStream = attachement.Data.Invoke();

                if (dataStream is MemoryStream)
                    image.ImageBinaryData = ((MemoryStream)dataStream).ToArray();
                else
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await dataStream.CopyToAsync(stream);
                        image.ImageBinaryData = stream.ToArray();
                    }
                }

                return image;
            }
        }

        public async Task StoreImage(AppImage image)
        {
            using (var session = store.OpenSession())
            {
                var dbCommands = session.Advanced.DocumentStore.DatabaseCommands;
                var optionalMetaData = new RavenJObject();
                optionalMetaData["Format"] = "JPG";

                //Put or update new image
                using (Stream stream = new MemoryStream(image.ImageBinaryData))
                {
                    await Task.Run(() => dbCommands.PutAttachment(image.Id, Guid.NewGuid(), stream, optionalMetaData));
                }
            }
        }

        public async Task<IEnumerable<string>> GetAllImageIds()
        {
            using (var session = store.OpenSession())
            {
                var dbCommands = session.Advanced.DocumentStore.DatabaseCommands;

                var attachement = await Task<IEnumerable<Attachment>>.Run(() => dbCommands.GetAttachmentHeadersStartingWith("", 0, 5000));

                var result = attachement.Select(m => m.Key);
                return result;
            }
        }

    }
}