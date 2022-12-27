using FileStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public class FileProcessingService
    {
        private readonly IFileRepository _fileRepository;

        public FileProcessingService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<StoredFileModel> UploadFilePreventDuplicate(StoredFileModel fileModel)
        {
            //// Checking for duplicates, step 1
            //fileModel.IsDuplicateMd5 = await _fileRepository.FindAnyByMd5(fileModel.Md5);

            //StoredFileModel duplicateFileModel = null;

            //if (fileModel.IsDuplicateMd5)
            //{
            //    // Checking for duplicates, step 2
            //    var duplicateMda5Files = await _fileRepository.FindAllByMd5(fileModel.Md5);

            //    foreach (var df in duplicateMda5Files)
            //    {
            //        if (fileModel.Sha1 == df.Sha1)
            //        {
            //            fileModel.IsDuplicateSha1 = true;
            //            duplicateFileModel = df;
            //            //_logger.Log($"duplicate file {filePath}\n");
            //            break;
            //        }
            //    }
            //}

            //if (fileModel.IsDuplicateSha1)
            //    return duplicateFileModel;

            var fileId = await _fileRepository.UploadFileAsync(fileModel);

            return await _fileRepository.DownLoadFileAsync(fileId);
        }
    }
}
