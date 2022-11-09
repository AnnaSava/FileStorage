using FileStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage
{
    public interface IFileRepository
    {
        Task<string> UploadFileAsync(StoredFileModel file);

        Task<StoredFileModel> DownLoadFileAsync(string id);

        Task UpdateManySha1Async(IEnumerable<StoredFileModel> files);

        Task<bool> FindAnyByMd5(string md5nash);

        Task<IEnumerable<StoredFileModel>> FindAllByMd5(string md5hash);

        Task<bool> FindAnyBySha1(string sha1hash);
    }
}
