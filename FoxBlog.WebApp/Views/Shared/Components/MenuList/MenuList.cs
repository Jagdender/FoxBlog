using FoxBlog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FoxBlog.WebApp.Views.Shared.Components.MenuList
{
    public sealed class MenuList(IOptionsSnapshot<ContentContext> context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(context.Value);
        }
    }
}
