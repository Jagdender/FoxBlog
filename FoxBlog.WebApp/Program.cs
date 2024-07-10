using FoxBlog.Infrastructure.Configurations;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("content.json", reloadOnChange: true, optional: false);
builder.Services.AddContext(builder.Configuration);

// Add services to the container.
builder.Services.AddMarkdown();
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    string[] supportedCultures = ["en", "zh"];
    options
        .SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
    options.ApplyCurrentCultureToResponseHeaders = true;
    var stringQuery = options.RequestCultureProviders[0];
    options.RequestCultureProviders.RemoveAt(0);
    options.RequestCultureProviders.Add(stringQuery);
    ((CookieRequestCultureProvider)options.RequestCultureProviders[0]).CookieName = "lang";
});
builder
    .Services.AddControllersWithViews()
    .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly)
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRequestLocalization();

app.UseMarkdown();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");

app.Run();
