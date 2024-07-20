using System.Globalization;
using System.Text.RegularExpressions;
using FoxBlog.Application.Contexts;
using FoxBlog.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FoxBlog.Infrastructure.Factories
{
    internal sealed partial class PostContext(
        IOptionsSnapshot<ContentOptions> options,
        ILogger<PostContext> logger
    ) : IPostContext
    {
        private readonly ContentOptions _options = options.Value;
        private readonly ILogger<PostContext> _logger = logger;

        public async Task<IEnumerable<Post>> ReadAsync()
        {
            List<Post> posts = [];

            var directories = Directory.GetDirectories(_options.MarkdownPath);

            foreach (var directory in directories)
            {
                var markdownFilePaths = Directory
                    .GetFiles(directory)
                    .Where(file => file.EndsWith(CultureInfo.CurrentCulture.Name + ".md"));

                var category = directory.Split(Path.DirectorySeparatorChar).Last();

                bool isPrivate = string.Equals(
                    category,
                    nameof(Accessibility.Private),
                    StringComparison.InvariantCultureIgnoreCase
                );
                if (isPrivate)
                    continue;

                foreach (var markdownFilePath in markdownFilePaths)
                {
                    var post = await ReadAsync(markdownFilePath, category);
                    if (post is not null)
                        posts.Add(post);
                }
            }

            return posts;
        }

        private async Task<Post?> ReadAsync(string markdownPath, string category)
        {
            bool success = int.TryParse(Path.GetFileName(markdownPath).Split('.')[0], out int id);
            if (success == false)
            {
                _logger.LogError("The path [{path}] couldn't be parsed to id.", markdownPath);
                return null;
            }

            string content = (await File.ReadAllTextAsync(markdownPath)).Trim();
            DateTime datetime = File.GetLastWriteTime(markdownPath);

            string title = "No Title";
            Match titleMatch = TitleRegex().Match(content);
            if (titleMatch.Success)
            {
                title = titleMatch.Groups[1].Value.Trim();
                content = TitleRegex().Replace(content, string.Empty);
            }

            string? header = null;
            Match headerMatch = HeaderImgRegex().Match(content);
            if (headerMatch.Success)
            {
                header = headerMatch.Groups[2].Value.Trim();
                content = HeaderImgRegex().Replace(content, string.Empty);
            }

            Post viewmodel =
                new()
                {
                    Id = id,
                    Content = content,
                    Title = title,
                    Header = header,
                    Category = category,
                    DateTime = datetime
                };

            return viewmodel;
        }

        [GeneratedRegex(@"^#\s+(.*)", RegexOptions.Multiline, 3000)]
        private static partial Regex TitleRegex();

        [GeneratedRegex(@"\A\!\[\s*(.*?)\s*\]\((.*?)\)", RegexOptions.CultureInvariant, 3000)]
        private static partial Regex HeaderImgRegex();
    }
}
