using Athn.Helpers.MimeType;
using FileStorage.Helpers;
using FileStorage.Helpers.Images;
using FileStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public class ImageService : IImageService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IImageRepository _imageRepository;
        private readonly MimeTypeChecker _mimeTypeChecker;
        private readonly HashHelper _hashHelper;
        private readonly ImageEditor _imageEditor;

        public ImageService(IFileRepository fileRepository,
            IImageRepository imageRepository,
            MimeTypeChecker mimeTypeChecker,
            HashHelper hashHelper,
            ImageEditor imageEditor)
        {
            _fileRepository = fileRepository;
            _imageRepository = imageRepository;
            _mimeTypeChecker = mimeTypeChecker;
            _hashHelper = hashHelper;
            _imageEditor = imageEditor;
        }

        //TODO async???
        public IEnumerable<ImageModel> GetImages(int page = 1, int count = 24)
        {
            var images = _imageRepository.GetImages(page, count);
            return images;
        }

        public async Task SaveImage(byte[] content)
        {
            var fileModel = MakeFileModelToUpload(content);

            var storedImageFiles = new Dictionary<string, StoredFileModel>();

            var uploadedFileModel = await UploadFilePreventDuplicate(fileModel);
            storedImageFiles.Add("Original", uploadedFileModel);

            var resizedFileModel = await ResizeAndSave(uploadedFileModel);
            storedImageFiles.Add("Resized", resizedFileModel);

            var thumbFileModel = await CropAndSave(resizedFileModel);
            storedImageFiles.Add("Thumb", thumbFileModel);

            var imageModel = new ImageModel
            {
                PreviewId = thumbFileModel.Id,
                Files = new List<ImageFileModel>()
            };

            foreach (var image in storedImageFiles)
            {
                var size = _imageEditor.GetSize(image.Value.Content);

                var imgFile = new ImageFileModel
                {
                    FileId = image.Value.Id,
                    Type = image.Key,
                    Width = size.Item1,
                    Height = size.Item2
                };

                imageModel.Files.Add(imgFile);
            }

            await _imageRepository.Create(imageModel);
        }

        private async Task<StoredFileModel> ResizeAndSave(StoredFileModel fileModel)
        {
            using var ms = new MemoryStream();
            _imageEditor.Resize(fileModel.Content, ms);

            var content = ms.ToArray();
            var resizedFileModel = MakeFileModelToUpload(content);

            var saved = await UploadFilePreventDuplicate(resizedFileModel);
            return saved;
        }

        private async Task<StoredFileModel> CropAndSave(StoredFileModel fileModel)
        {
            using var ms = new MemoryStream();
            _imageEditor.SquareCrop(fileModel.Content, ms);

            var content = ms.ToArray();
            var croppedFileModel = MakeFileModelToUpload(content);

            var saved = await UploadFilePreventDuplicate(croppedFileModel);
            return saved;
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

        private async Task<StoredFileModel> UploadFilePreventDuplicate(StoredFileModel fileModel)
        {
            // Checking for duplicates, step 1
            fileModel.IsDuplicateMd5 = await _fileRepository.FindAnyByMd5(fileModel.Md5);

            StoredFileModel duplicateFileModel = null;

            if (fileModel.IsDuplicateMd5)
            {
                // Checking for duplicates, step 2
                var duplicateMda5Files = await _fileRepository.FindAllByMd5(fileModel.Md5);

                foreach (var df in duplicateMda5Files)
                {
                    if (fileModel.Sha1 == df.Sha1)
                    {
                        fileModel.IsDuplicateSha1 = true;
                        duplicateFileModel = df;
                        //_logger.Log($"duplicate file {filePath}\n");
                        break;
                    }
                }
            }

            if (fileModel.IsDuplicateSha1)
                return duplicateFileModel;

            var fileId = await _fileRepository.UploadFileAsync(fileModel);

            return await _fileRepository.DownLoadFileAsync(fileId);
        }
    }
}
