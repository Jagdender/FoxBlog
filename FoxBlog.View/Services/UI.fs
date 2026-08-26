namespace FoxBlog.View

open Giraffe.ViewEngine
open Types

module UI =
    let private read name (settings: Global.Settings) element =
        element
        |> Json.map name
        |> Option.orElse (settings.Json |> Json.mapMany [ "ui"; name ])
        |> Option.bind Json.tryDeserialize

    type Context(settings: Global.Settings) =

        member val current = None with get, set

        member this.str key =
            this.current |> Option.bind _.nodes.TryFind(key) |> Option.defaultValue key

        member this.node = this.str >> str

        member this.attr attrName = this.str >> (attr attrName)

        member _.supported =
            settings.Json
            |> Json.mapMany [ "ui"; "items" ]
            |> Option.bind Json.tryDeserialize<UI list>
            |> Option.defaultValue List.empty
