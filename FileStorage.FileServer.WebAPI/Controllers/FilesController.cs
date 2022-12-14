using FileStorage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileStorage.Server.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<FilesController> _logger;
        private readonly FileServer.WebAPI.FileServerSettings _settings;

        public FilesController(IFileRepository fileRepository, 
            IWebHostEnvironment env,
            FileServer.WebAPI.FileServerSettings settings,
            ILogger<FilesController> logger)
        {
            _fileRepository = fileRepository;
            _env = env;
            _logger = logger;
            _settings = settings;
        }

        // GET: api/<WebApiController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get(int page, int count)
        {
            var fileIds = await _fileRepository.GetStoredFileIds(page, count);
            return fileIds.Select(m => string.Format(_settings.UriPattern, m));
        }

        // GET api/<WebApiController>/5
        [HttpGet("{id}")]
        public async Task<FileStreamResult> Get(string id)
        {
            // Example https://localhost:7060/webapi/5e89bbd8f4802f1f841078be

            var file = await _fileRepository.DownLoadFileAsync(id);
            file.MimeType = "image/jpeg";

            var stream = new MemoryStream(file?.Content);
            return new FileStreamResult(stream, file.MimeType);
        }

        // POST api/<WebApiController>
        [HttpPost]
        public async Task<ActionResult<IList<UploadResultModel>>> Post([FromForm] IEnumerable<IFormFile> files)
        {
            var maxAllowedFiles = 3;
            long maxFileSize = 1024 * 1500;
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

                            _logger.LogInformation("{FileName} saved at {Path}",
                                trustedFileNameForDisplay, path);
                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = trustedFileNameForFileStorage;
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

        // PUT api/<WebApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WebApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
