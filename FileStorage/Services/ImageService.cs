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
        private readonly IImageRepository _imageRepository;
        private readonly ImageEditor _imageEditor;
        private readonly FileProcessingService _fileProcessingService;
        private readonly FileServerSettings _settings;

        public ImageService(IImageRepository imageRepository,
            ImageEditor imageEditor,
            FileProcessingService fileProcessingService,
            FileServerSettings settings)
        {
            _imageRepository = imageRepository;
            
            _imageEditor = imageEditor;
            _fileProcessingService = fileProcessingService;

            _settings = settings;
        }

        //TODO async???
        public IEnumerable<ImageModel> GetImages(int page = 1, int count = 24)
        {
            List<ImageModel> images = _imageRepository.GetImages(page, count).ToList();

            foreach(var img in images)
            {
                img.PreviewId = string.Format(_settings.UriPattern, img.PreviewId);

                foreach(var file in img.Files)
                {
                    file.FileId = string.Format(_settings.UriPattern, file.FileId);
                }
            }

            return images;
        }

        public ImageModel GetImage(string id)
        {
            var img = _imageRepository.GetImage(id);

            img.PreviewId = string.Format(_settings.UriPattern, img.PreviewId);

            foreach (var file in img.Files)
            {
                file.FileId = string.Format(_settings.UriPattern, file.FileId);
            }

            return img;
        }

        public async Task SaveImage(byte[] content)
        {
            var storedImageFiles = new Dictionary<string, StoredFileModel>();

            var uploadedFileModel = await _fileProcessingService.UploadFilePreventDuplicate(content);
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

            var saved = await _fileProcessingService.UploadFilePreventDuplicate(content);
            return saved;
        }

        private async Task<StoredFileModel> CropAndSave(StoredFileModel fileModel)
        {
            using var ms = new MemoryStream();
            _imageEditor.SquareCrop(fileModel.Content, ms);

            var content = ms.ToArray();

            var saved = await _fileProcessingService.UploadFilePreventDuplicate(content);
            return saved;
        }

        
    }
}
