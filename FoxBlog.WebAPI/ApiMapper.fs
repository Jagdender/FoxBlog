module Api

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.HttpResults
open FSharp.MinimalApi.Builder
open Giraffe.ViewEngine

#nowarn "20"

let routes =
    endpoints {



        get "/{ui:nonfile}/{*rest}" produces<ContentHttpResult> (fun (req: {| index: FoxBlog.View.Index |}) ->
            TypedResults.Content(req.index.Html |> RenderView.AsString.htmlDocument, "text/html"))

        get "/" produces<ContentHttpResult> (fun (req: {| index: FoxBlog.View.Index |}) ->
            TypedResults.Content(req.index.Html |> RenderView.AsString.htmlDocument, "text/html"))

    }
