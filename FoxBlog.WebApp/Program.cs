using FoxBlog.Infrastructure.Configurations;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);
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

//builder.Services.AddResponseCompression(options =>
//{
//    options.EnableForHttps = true;
//    options.Providers.Add<BrotliCompressionProvider>();
//    options.Providers.Add<GzipCompressionProvider>();
//    options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
//});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

//app.UseResponseCompression();

app.UseRequestLocalization();

app.UseMarkdown();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapFallback(context =>
{
    context.Response.StatusCode = StatusCodes.Status404NotFound;
    context.Response.Redirect("/");
    return Task.CompletedTask;
});

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");

app.Run();
