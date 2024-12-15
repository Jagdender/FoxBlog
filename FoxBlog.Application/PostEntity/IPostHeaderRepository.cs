namespace FoxBlog.Application.PostEntity;

public interface IPostHeaderRepository
{
    public string? GetHeaderImgUrl(PostKey post);

    public Stream? GetHeaderImgFile(PostKey post);
}
