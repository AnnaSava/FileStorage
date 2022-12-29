using FileStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage
{
    public interface IImageService
    {
        Task SaveImage(byte[] content);

        IEnumerable<ImageModel> GetImages(int page = 1, int count = 24);

        ImageModel GetImage(string id);
    }
}
