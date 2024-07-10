using FoxBlog.Domain;

namespace FoxBlog.Infrastructure
{
    public sealed class ContentContext
    {
        public List<Post> Posts { get; set; } = default!;
        public string MarkdownPath { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}
