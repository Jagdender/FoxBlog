module Api

open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.HttpResults
open FSharp.MinimalApi.Builder
open Giraffe.ViewEngine
open FoxBlog.View

#nowarn "20"

let routes =
    endpoints {



        get "/{ui:nonfile}/{*rest}" produces<ContentHttpResult> (fun (req: {| index: Index |}) ->
            TypedResults.Content(req.index.Html |> RenderView.AsString.htmlDocument, "text/html"))

        get "/" produces<ContentHttpResult> (fun (req: {| index: Index |}) ->
            TypedResults.Content(req.index.Html |> RenderView.AsString.htmlDocument, "text/html"))

    }

//let uiMiddlware =
//    createMiddleware (fun context request ->

//        let ui = context.RequestServices.GetRequiredService<UI.Context>()
//        let path = context.Request.Path

//        let next pickedUI =
//            ui.current <- Some pickedUI
//            request.Invoke context

//        match ui.defaultUI with
//        | None ->
//            ui.supported
//            |> List.tryFind (fun x -> path.IStartsWithSegments $"{x.name}")
//            |> Option.map next
//            |> Option.defaultValue context.Response.NotFound
//        | Some defaultUI ->
//            ui.supported
//            |> List.tryFind (fun x -> path.IStartsWithSegments $"{x.name}")
//            |> Option.map next
//            |> Option.defaultValue context.Response.Redirect // TODO: action eval

//    )

let routeMiddleware =
    createMiddleware (fun context request ->
        let ui = context.RequestServices.GetRequiredService<UI.Context>()
        let settings = context.RequestServices.GetRequiredService<Global.Settings>()
        let content = context.RequestServices.GetRequiredService<Content.Context>()

        let mutable path = context.Request.Path

        ui.supported
        |> List.tryFind (
            _.name
            >> Option.map (fun x -> $"/{x}")
            >> Option.exists (fun x -> context.Request.Path.StartsWithSegments(x, &path))
        )
        |> Option.orElseWith (fun _ ->
            path <- context.Request.Path
            ui.supported |> List.tryFindBack (_.name >> Option.isNone))
        |> fun current -> ui.current <- current

        content.path <- path

        request.Invoke context)
