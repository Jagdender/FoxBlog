using System.Diagnostics;
using FoxBlog.Infrastructure;
using FoxBlog.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FoxBlog.WebApp.Controllers;

public class HomeController(IOptionsSnapshot<ContentOptions> options) : Controller
{
    private readonly ContentOptions options = options.Value;

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        string? filename = options.GetHomeFilePath();

        if (!System.IO.File.Exists(filename))
            return NotFound();

        string markdown = await System.IO.File.ReadAllTextAsync(filename, cancellationToken);

        ViewData["Markdown"] = markdown;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}

internal readonly record struct MarkdownContent(string Value);
