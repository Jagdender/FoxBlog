using FoxBlog.Application.Contexts;
using FoxBlog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FoxBlog.WebApp.Controllers
{
    public sealed partial class PostController(
        IPostContext context,
        IOptionsSnapshot<ContentOptions> options
    ) : Controller
    {
        private readonly IPostContext _context = context;
        private readonly ContentOptions _options = options.Value;

        [Route("[controller]/{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            var posts = await _context.ReadAsync();
            var post = posts.FirstOrDefault(p => p.Id == id);
            if (post is null)
                return View("404");

            return View(post);
        }

        [Route("/Posts")]
        [Route("/Posts/{category}")]
        public async Task<IActionResult> List(string? category = null)
        {
            var posts = await _context.ReadAsync();

            if (string.IsNullOrWhiteSpace(category) == false)
                posts = posts.Where(p => p.Category == category);

            if (Request.Query.TryGetValue("tag", out var tags))
                posts = posts.Where(p => p.Tags.Except(tags).Any());

            return View(posts);
        }
    }
}
