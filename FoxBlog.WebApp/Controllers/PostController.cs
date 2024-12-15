using FoxBlog.Application.PostEntity;
using Microsoft.AspNetCore.Mvc;

namespace FoxBlog.WebApp.Controllers;

public sealed partial class PostController(IPostContext context) : Controller
{
    [Route("[controller]/{post}")]
    public async Task<IActionResult> Index(int post, CancellationToken cancellationToken)
    {
        PostKey key = new(post);

        var postEntity = await context.GetAsync(key, cancellationToken);

        if (postEntity == null)
            return Redirect("/");

        // TODO: Implement private postEntity handling
        if (postEntity.IsPrivate)
            return Redirect("/");

        return View(postEntity);
    }

    [Route("/Posts")]
    public async Task<IActionResult> List(
        [FromQuery] int? category = null,
        CancellationToken cancellationToken = default
    )
    {
        var posts = await context.GetAllAsync(cancellationToken);

        if (category.HasValue)
            posts = posts.Where(p => p.CategoryKey.Value == category);

        return View(posts);
    }
}
