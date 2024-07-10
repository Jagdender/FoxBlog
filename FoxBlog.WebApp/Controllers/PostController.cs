using FoxBlog.Infrastructure;
using FoxBlog.WebApp.Views.Shared.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace FoxBlog.WebApp.Controllers
{
    public sealed partial class PostController(
        IStringLocalizer<SharedLangResource> lang,
        IOptionsSnapshot<ContentContext> context
    ) : Controller
    {
        private readonly ContentContext _context = context.Value;
        private readonly string _suffix = '.' + lang["Current"] + ".md";

        [Route("[controller]/{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (post is null)
                return View("404");

            string key = Path.Combine(_context.MarkdownPath, id + _suffix);
            if (System.IO.File.Exists(key) == false)
                return View("404");

            string content = await System.IO.File.ReadAllTextAsync(key);
            ViewData["Markdown"] = content;
            ViewData["Title"] = post.Title;

            return View(post);
        }

        [Route("/Posts")]
        [Route("/Posts/{category}")]
        public IActionResult List(string? category = null)
        {
            var posts = _context.Posts.AsEnumerable();

            if (string.IsNullOrWhiteSpace(category) == false)
                posts = _context.Posts.Where(p => p.Category == category);

            if (Request.Query.TryGetValue("tag", out var tags))
                posts = posts.Where(p => p.Tags.Except(tags).Any());

            return View(posts);
        }
    }
}
