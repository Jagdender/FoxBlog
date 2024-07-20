using FoxBlog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FoxBlog.WebApp.Controllers
{
    public class FileController(IOptionsSnapshot<ContentOptions> options) : Controller
    {
        private readonly ContentOptions _options = options.Value;

        [HttpGet("/image/{filename}")]
        public IActionResult Image(string filename)
        {
            filename = Path.Combine(_options.ImagePath, filename);

            if (System.IO.File.Exists(filename) == false)
            {
                return NotFound();
            }
            var stream = System.IO.File.OpenRead(filename);
            return File(stream, "image/*");
        }
    }
}
