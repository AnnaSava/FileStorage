using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileStorage.Server.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebApiController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;

        public WebApiController(IFileRepository bookService)
        {
            _fileRepository = bookService;
        }

        // GET: api/<WebApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<WebApiController>/5
        [HttpGet("{id}")]
        public async Task<FileStreamResult> File(string id)
        {
            // Example https://localhost:7060/webapi/5e89bbd8f4802f1f841078be

            var file = await _fileRepository.DownLoadFileAsync(id);
            file.MimeType = "image/jpeg";

            var stream = new MemoryStream(file?.Content);
            return new FileStreamResult(stream, file.MimeType);
        }

        // POST api/<WebApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
