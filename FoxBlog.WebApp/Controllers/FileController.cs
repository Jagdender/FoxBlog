using FoxBlog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FoxBlog.WebApp.Controllers
{
    public class FileController(IOptionsSnapshot<ContentContext> context) : Controller
    {
        private readonly ContentContext _context = context.Value;

        [HttpGet("/image/{filename}")]
        public IActionResult Image(string filename)
        {
            filename = Path.Combine(_context.ImagePath, filename);

            if (System.IO.File.Exists(filename) == false)
            {
                return NotFound();
            }
            var stream = System.IO.File.OpenRead(filename);
            return File(stream, "image/*");
        }
    }
}
