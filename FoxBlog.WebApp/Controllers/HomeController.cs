using System.Diagnostics;
using FoxBlog.WebApp.Models;
using FoxBlog.WebApp.Views.Shared.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FoxBlog.WebApp.Controllers
{
    public class HomeController(IStringLocalizer<SharedLangResource> lang) : Controller
    {
        private readonly IStringLocalizer<SharedLangResource> _lang = lang;

        public IActionResult Index()
        {
            if (_lang["Current"] == "zh")
                return View("Index.zh");
            else
                return View("Index.en");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}
