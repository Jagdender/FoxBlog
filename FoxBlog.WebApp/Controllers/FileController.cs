using System.Diagnostics.CodeAnalysis;
using FoxBlog.Application.PostEntity;
using FoxBlog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FoxBlog.WebApp.Controllers;

public class FileController(
    IOptionsSnapshot<ContentOptions> options,
    IPostHeaderRepository headerRepository
) : Controller
{
    private readonly ContentOptions options = options.Value;

    [HttpGet("/image/{filename}")]
    [HttpGet("/image/{post:int}/{filename}")]
    public IActionResult Image(string filename, int? post = null)
    {
        if (post.HasValue)
        {
            PostKey key = new(post.Value);
            string? section = options.GetPostSectionPath(key);
            if (!Directory.Exists(section))
                return NotFound();

            filename = Path.Combine(section, filename);
            return filename.Exists() ? File(filename.OpenRead(), "image/*") : NotFound();
        }

        if (options.GlobalImageSectionPath is null)
            return NotFound();

        filename = Path.Combine(options.GlobalImageSectionPath, filename);

        if (filename.NotExists())
            return NotFound();

        return File(filename.OpenRead(), "image/*");
    }

    [HttpGet("/header/{post:int}")]
    public IActionResult Header(int post)
    {
        var postKey = new PostKey(post);
        var file = headerRepository.GetHeaderImgFile(postKey);

        return file is null ? NotFound() : File(file, "image/*");
    }
}

file static class FileSystemExtensions
{
    public static bool Exists([NotNullWhen(true)] this string? filename)
    {
        return File.Exists(filename);
    }

    public static bool NotExists([NotNullWhen(false)] this string? filename)
    {
        return !File.Exists(filename);
    }

    public static Stream OpenRead(this string filename)
    {
        return File.OpenRead(filename);
    }
}
