namespace FoxBlog.View

open Giraffe.ViewEngine
open System.IO
open Types

module UI =
    let private read name (settings: Global.Settings) element =
        element
        |> Json.map name
        |> Option.orElse (settings.Json |> Json.mapMany [ "ui"; name ])
        |> Option.bind Json.tryDeserialize

    type Context(settings: Global.Settings) =

        let values =
            settings.Root
            |> Directory.EnumerateDirectories
            |> Seq.tryFind (Path.GetFileName >> _.IEquals("ui"))
            |> Option.map (
                Directory.EnumerateFiles // TODO: more extensions support
                >> Seq.distinctBy Path.GetFileNameWithoutExtension // NOTE: no guarantee for multi configs to the same ui
                >> Seq.map (fun x ->
                    (Path.GetFileNameWithoutExtension(x).ToLowerInvariant(),
                     Json.tryRead x |> Option.defaultValue Json.empty))
            )
            |> Option.defaultValue Seq.empty
            |> Seq.map (fun (name, element) ->
                name,
                element
                |> Json.tryDeserialize<UI>
                |> Option.defaultValue
                    { display = None
                      top = None
                      side = None
                      nodes = Map.empty
                      hidden = false
                      language = false })
            |> Map.ofSeq

        let mutable _current = None

        member this.current
            with get () = _current
            and set value =
                value
                |> Option.filter (fun x -> this.supported |> Map.containsKey x)
                |> fun x -> _current <- x


        member this.currentUI =
            this.current |> Option.bind (fun x -> this.supported |> Map.tryFind x)

        member this.str key =
            this.currentUI |> Option.bind _.nodes.TryFind(key) |> Option.defaultValue key

        member this.node = this.str >> str

        member this.attr attrName = this.str >> (attr attrName)

        member _.supported = values
