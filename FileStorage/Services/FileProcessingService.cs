using Athn.Helpers.MimeType;
using FileStorage.Helpers;
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
        private readonly MimeTypeChecker _mimeTypeChecker;
        private readonly HashHelper _hashHelper;

        public FileProcessingService(IFileRepository fileRepository,
            MimeTypeChecker mimeTypeChecker,
            HashHelper hashHelper)
        {
            _fileRepository = fileRepository;
            _mimeTypeChecker = mimeTypeChecker;
            _hashHelper = hashHelper;
        }

        public async Task<StoredFileModel> UploadFilePreventDuplicate(byte[] content)
        {
            var fileModel = MakeFileModelToUpload(content);

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

        private StoredFileModel MakeFileModelToUpload(byte[] content)
        {
            var mimeType = _mimeTypeChecker.GetMimeType(content);

            var model = new StoredFileModel
            {
                Content = content,
                MimeType = mimeType,
                Md5 = _hashHelper.GetMd5Hash(content),
                Sha1 = _hashHelper.GetSha1Hash(content),
                Ext = _mimeTypeChecker.GetExtention(mimeType),
                Size = content.Length
            };

            return model;
        }
    }
}
