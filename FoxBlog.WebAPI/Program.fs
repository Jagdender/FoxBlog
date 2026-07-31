namespace FoxBlog

#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddSerivces().AddViews().ConfigureGlobalSettings(builder.Configuration)

        builder.Services.AddControllers()

        let app = builder.Build()

        app.UseResponseCompression()

        app.UseStaticFiles()

        app.Use(Api.routeMiddleware)

        app.Run()

        0
