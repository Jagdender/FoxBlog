using FoxBlog.Domain;

namespace FoxBlog.Infrastructure
{
    public sealed class ContentContext
    {
        public List<Post> Posts { get; set; } = default!;
        public string ContentPath { get; set; } = string.Empty;
    }
}
