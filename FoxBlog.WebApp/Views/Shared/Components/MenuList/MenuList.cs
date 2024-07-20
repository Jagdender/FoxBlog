using FoxBlog.Application.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace FoxBlog.WebApp.Views.Shared.Components.MenuList
{
    public sealed class MenuList(IPostContext context) : ViewComponent
    {
        private readonly IPostContext _context = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _context.ReadAsync();
            return View(posts);
        }
    }
}
