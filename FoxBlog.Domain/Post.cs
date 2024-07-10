global using PostId = int;

namespace FoxBlog.Domain
{
    public sealed class Post
    {
        public PostId Id { get; init; }
        public string Title { get; set; } = "Unnamed";
        public string[] Tags { get; init; } = [];
        public string Category { get; init; } = "Default";
        public DateTime DateTime { get; init; }
        public string? Header { get; set; }
    }
}
