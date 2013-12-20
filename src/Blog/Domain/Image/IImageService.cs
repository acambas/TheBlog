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
        Task StoreImage(AppImage image);

        Task<AppImage> GetImageData(string id);

        Task<IEnumerable<string>> GetAllImageIds();
    }
}
