using FoxBlog.Domain;

namespace FoxBlog.Application.Contexts
{
    public interface IPostContext
    {
        public Task<IEnumerable<Post>> ReadAsync();
    }
}
