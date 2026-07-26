[<AutoOpen>]

module Extensions

open Microsoft.AspNetCore.ResponseCompression
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Options
open System.Runtime.CompilerServices
open System.IO.Compression
open System.IO
open System
open FoxBlog.View


[<CLIMutable>]
type ConfigOptions = { Config: string }

type IServiceCollection with
    member this.AddViews() =
        this.AddScoped<Index>().AddScoped<Top>().AddScoped<Side>().AddScoped<Main>().AddScoped<Head>()

    member this.AddSerivces() =
        let compression = CompressionLevel.SmallestSize

        this
            .AddResponseCompression(fun options ->
                options.EnableForHttps <- true
                options.Providers.Add<BrotliCompressionProvider>()
                options.Providers.Add<GzipCompressionProvider>())
            .Configure(fun (options: BrotliCompressionProviderOptions) -> options.Level <- compression)
            .Configure(fun (options: GzipCompressionProviderOptions) -> options.Level <- compression)

            .AddHttpContextAccessor()
            .AddScoped<ViewContext>(fun services ->
                let context = ViewContext()

                context.ui <-
                    services
                        .GetRequiredService<IHttpContextAccessor>()
                        .HttpContext.Request.RouteValues.TryGetValue("ui")
                    |> function
                        | true, value -> Some(value.ToString())
                        | false, _ -> None

                context)
            .AddScoped<Content.Context>()
            .AddScoped<UI.Context>()

    member this.ConfigureGlobalSettings(configuration: IConfiguration) =
        let factory (sp: IServiceProvider) =
            let config = sp.GetRequiredService<IOptionsSnapshot<ConfigOptions>>().Value.Config

            let configFile =
                match config with
                | p when File.Exists p -> p
                | p when Directory.Exists p -> Path.Combine(p, "global.json")
                | _ -> failwith "404"

            let element = configFile |> Json.tryRead |> Option.defaultValue Json.empty

            { Json = element
              Root = Path.GetDirectoryName configFile }

        this.Configure<ConfigOptions>(configuration).AddScoped<GlobalSettings>(factory)
