using FileStorage.Helpers.Images;
using FileStorage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileStorage.FileService.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IFileRepository _fileRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ImageEditor _imageEditor;
        private readonly IImageService _imageService;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IFileRepository fileRepository,
            IImageRepository imageRepository,
            ImageEditor imageEditor,
            IWebHostEnvironment env,
            IImageService imageService,
            ILogger<ImagesController> logger)
        {
            _fileRepository = fileRepository;
            _imageRepository = imageRepository;
            _imageEditor = imageEditor;
            _env = env;
            _logger = logger;
            _imageService = imageService;
        }

        // GET: api/<ImagesController>
        [HttpGet]
        public IEnumerable<ImageModel> Get(int page, int count)
        {
            return _imageService.GetImages(page, count);
        }

        // GET api/<ImagesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ImagesController>
        [HttpPost]
        public async Task<ActionResult<IList<UploadResultModel>>> Post([FromForm] IEnumerable<IFormFile> files)
        {
            var maxAllowedFiles = 3;
            long maxFileSize = 1024 * 2000;
            var filesProcessed = 0;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            List<UploadResultModel> uploadResults = new();

            foreach (var file in files)
            {
                var uploadResult = new UploadResultModel();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay =
                    WebUtility.HtmlEncode(untrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        _logger.LogInformation("{FileName} length is 0 (Err: 1)",
                            trustedFileNameForDisplay);
                        uploadResult.ErrorCode = 1;
                    }
                    else if (file.Length > maxFileSize)
                    {
                        _logger.LogInformation("{FileName} of {Length} bytes is " +
                            "larger than the limit of {Limit} bytes (Err: 2)",
                            trustedFileNameForDisplay, file.Length, maxFileSize);
                        uploadResult.ErrorCode = 2;
                    }
                    else
                    {
                        try
                        {
                            trustedFileNameForFileStorage = Path.GetRandomFileName();
                            var path = Path.Combine(_env.ContentRootPath,
                                _env.EnvironmentName, "unsafe_uploads",
                                trustedFileNameForFileStorage);

                            await using FileStream fs = new(path, FileMode.Create);
                            await file.CopyToAsync(fs);
                            
                            CropAndSave(file);
                            ResizeAndSave(file);

                            _logger.LogInformation("{FileName} saved at {Path}",
                                trustedFileNameForDisplay, path);
                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = trustedFileNameForFileStorage;

                            using var ms = new MemoryStream();
                            await file.CopyToAsync(ms);
                            await _imageService.SaveImage(ms.ToArray());
                        }
                        catch (IOException ex)
                        {
                            _logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                                trustedFileNameForDisplay, ex.Message);
                            uploadResult.ErrorCode = 3;
                        }
                    }

                    filesProcessed++;
                }
                else
                {
                    _logger.LogInformation("{FileName} not uploaded because the " +
                        "request exceeded the allowed {Count} of files (Err: 4)",
                        trustedFileNameForDisplay, maxAllowedFiles);
                    uploadResult.ErrorCode = 4;
                }

                uploadResults.Add(uploadResult);
            }

            return new CreatedResult(resourcePath, uploadResults);
        }

        private void CropAndSave(IFormFile file)
        {
            var cropFileName = Path.GetRandomFileName();
            var cropPath = Path.Combine(_env.ContentRootPath,
                _env.EnvironmentName, "unsafe_uploads",
                cropFileName);

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            using var ms2 = new MemoryStream();
            _imageEditor.SquareCrop(ms.ToArray(), ms2);

            using var stream = System.IO.File.Create(cropPath);

            var cropped = ms2.ToArray();
            stream.Write(cropped, 0, cropped.Length);
        }

        private void ResizeAndSave(IFormFile file)
        {
            var cropFileName = Path.GetRandomFileName();
            var cropPath = Path.Combine(_env.ContentRootPath,
                _env.EnvironmentName, "unsafe_uploads",
                cropFileName);

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            using var ms2 = new MemoryStream();
            _imageEditor.Resize(ms.ToArray(), ms2);

            using var stream = System.IO.File.Create(cropPath);

            var cropped = ms2.ToArray();
            stream.Write(cropped, 0, cropped.Length);
        }

        // PUT api/<ImagesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ImagesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
