using Domain.Image;
using Raven.Client.Document;
using Raven.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebUi.App_Start
{
    public class StoreOnServerImageService : IImageService
    {

        public void StoreImage(string id, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void StoreImage(string id, Stream data)
        {
            throw new NotImplementedException();
        }

        public string GetImageUrl(string id)
        {
            throw new NotImplementedException();
        }

        public AppImage GetImageData(string id)
        {
            throw new NotImplementedException();
        }

        public string GetImageSoureUrl()
        {
            throw new NotImplementedException();
        }
    }


    public class StoreOnRavenDBImageService : IImageService
    {
        DocumentStore store = WebUi.MvcApplication.Store;

        public void StoreImage(string id, byte[] data)
        {
            using (var session = store.OpenSession())
            {
                var dbCommands = session.Advanced.DocumentStore.DatabaseCommands;
                var optionalMetaData = new RavenJObject();
                optionalMetaData["Format"] = "JPG";
                using (Stream stream = new MemoryStream(data))
                {
                    dbCommands.PutAttachment(id, Guid.NewGuid(),
                    stream, optionalMetaData);
                }
            }
        }

        public void StoreImage(string id, Stream data)
        {
            using (var session = store.OpenSession())
            {
                var dbCommands = session.Advanced.DocumentStore.DatabaseCommands;
                var optionalMetaData = new RavenJObject();
                optionalMetaData["Format"] = "JPG";
                dbCommands.PutAttachment(id, null,data, optionalMetaData);
            }
        }

        public string GetImageUrl(string id)
        {
            throw new NotImplementedException();
        }

        public AppImage GetImageData(string id)
        {
            using (var session = store.OpenSession())
            {
                var dbCommands = session.Advanced.DocumentStore.DatabaseCommands;
                var attachement = dbCommands.GetAttachment(id);
                AppImage image = new AppImage() { Id = id };

                var input = attachement.Data.Invoke();

                if (input is MemoryStream)
                {
                    image.ImageBinaryData = ((MemoryStream)input).ToArray();
                }
                else
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        input.CopyTo(stream);
                        image.ImageBinaryData = stream.ToArray();
                    }
                }

                return image;
            }
        }

        public string GetImageSoureUrl()
        {
            throw new NotImplementedException();
        }
    }
}