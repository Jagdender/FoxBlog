using System.Globalization;
using FoxBlog.Application.PostEntity;

namespace FoxBlog.Infrastructure;

public sealed class ContentOptions
{
    public const string PostsSection = "posts";
    public const string ImageSection = "images";
    public const string HomeSection = "home";
    public const string DefaultMarkdownFilename = "index.md";
    public const string RecordsFilename = "records.json";

    public required string SrcPath { get; init; }
    public string? PostsSectionPath =>
        Path.Combine(SrcPath, PostsSection).DirectoryValidationPipeline();
    public string? GlobalImageSectionPath =>
        Path.Combine(SrcPath, ImageSection).DirectoryValidationPipeline();
    public string? RecordsFilePath =>
        Path.Combine(SrcPath, RecordsFilename).FilePathValidationPipeline();

    public string? GetPostHeaderFilePath(PostKey key)
    {
        string? path = GetPostSectionPath(key);

        if (path is null)
            return null;

        string[] files = Directory.GetFiles(path, "header.*", SearchOption.TopDirectoryOnly);

        return files.FirstOrDefault();
    }

    public string? GetPostSectionPath(PostKey key)
    {
        if (PostsSectionPath is null)
            return null;

        string path = Path.Combine(PostsSectionPath, key);

        return Directory.Exists(path) ? path : null;
    }

    public string? GetPostMarkdownFilePath(PostKey post)
    {
        if (PostsSectionPath is null)
            return null;

        string path = Path.Combine(PostsSectionPath, post);

        if (!Directory.Exists(path))
            return null;

        string[] files = Directory.GetFiles(path, "*.md", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
            return null;

        string languageSpecificMarkdown =
            $"index.{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}.md";

        string? file =
            files.FirstOrDefault(f => f.EndsWith(languageSpecificMarkdown))
            ?? files.FirstOrDefault(f => f.EndsWith(DefaultMarkdownFilename));

        return file;
    }

    public string? GetHomeFilePath()
    {
        if (PostsSectionPath is null)
            return null;

        string path = Path.Combine(PostsSectionPath, HomeSection);

        if (!Directory.Exists(path))
            return null;

        string[] files = Directory.GetFiles(path, "*.md", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
            return null;

        string languageSpecificMarkdown =
            $"index.{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}.md";

        string? file =
            files.FirstOrDefault(f => f.EndsWith(languageSpecificMarkdown))
            ?? files.FirstOrDefault(f => f.EndsWith(DefaultMarkdownFilename));

        return file;
    }
}

file static class FileSystemExtensions
{
    public static string? FilePathValidationPipeline(this string path) =>
        File.Exists(path) ? path : null;

    public static string? DirectoryValidationPipeline(this string path) =>
        Directory.Exists(path) ? path : null;
}
