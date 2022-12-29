using FileStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage
{
    public interface IImageRepository
    {
        Task<string> Create(ImageModel model);

        IEnumerable<ImageModel> GetImages(int page, int count);

        ImageModel GetImage(string id);
    }
}
