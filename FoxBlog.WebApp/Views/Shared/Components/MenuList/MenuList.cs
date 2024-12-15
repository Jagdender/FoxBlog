using FoxBlog.Application.PostEntity;
using Microsoft.AspNetCore.Mvc;

namespace FoxBlog.WebApp.Views.Shared.Components.MenuList;

public sealed class MenuList(IPostContext context) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var posts = await context.GetAllAsync();
        return View(posts);
    }
}
