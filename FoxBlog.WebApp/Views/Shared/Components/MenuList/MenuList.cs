using Microsoft.AspNetCore.Mvc;

namespace FoxBlog.WebApp.Views.Shared.Components.MenuList
{
    public sealed class MenuList : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
