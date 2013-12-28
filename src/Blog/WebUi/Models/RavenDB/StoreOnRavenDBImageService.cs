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

namespace WebUi.Models.RavenDB
{
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
                    await Task.Run(() => dbCommands.PutAttachment(image.Id, null, stream, optionalMetaData));
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



        public async Task DeleteImage(string id)
        {
            using (var session = store.OpenSession())
            {
                var dbCommands = session.Advanced.DocumentStore.DatabaseCommands;
                //Put or update new image
                await Task.Run(() => dbCommands.DeleteAttachment(id,null));
            }
        }
    }
}