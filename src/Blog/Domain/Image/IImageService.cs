using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Image
{
    public interface IImageService
    {
        void StoreImage(string id, byte[] data);

        void StoreImage(string id, Stream data);

        string GetImageUrl(string id);

        AppImage GetImageData(string id);

        string GetImageSoureUrl();
    }
}
