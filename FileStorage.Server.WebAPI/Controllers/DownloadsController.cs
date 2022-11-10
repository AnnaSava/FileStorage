using FileStorage.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileStorage.Server.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DownloadsController : ControllerBase
    {
        private readonly IImageService _imageService;

        public DownloadsController(IImageService imageService)
        {
            _imageService = imageService;
        }

        // GET: api/<DownloadsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DownloadsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DownloadsController>
        [HttpPost]
        public async void Post([FromBody] UploadUriModel value)
        {
            var client = new HttpClient();

            var response = await client.GetByteArrayAsync(value.FileUri);

            await _imageService.SaveImage(response);
        }

        // PUT api/<DownloadsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DownloadsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
