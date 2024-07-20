namespace FoxBlog.Domain
{
    public sealed class Post
    {
        public required int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string[] Tags { get; init; } = [];
        public DateTime DateTime { get; init; } = DateTime.MinValue;
        public Accessibility Accessibility { get; init; } = Accessibility.Public;
        public string? Header { get; init; } = null;

        public string Content { get; init; } = string.Empty;
    }
}
