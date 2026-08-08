namespace FoxBlog.View

open Giraffe.ViewEngine
open System.IO

module UI =
    let private read name (settings: Global.Settings) element =
        element
        |> Json.map name
        |> Option.orElse (settings.Json |> Json.mapMany [ "ui"; name ])
        |> Option.bind Json.tryDeserialize

    type Global.Settings with
        member settings.defaultUI = settings.Json |> Json.map "default" |> Option.map Json.str

    type UI =
        { display: string
          top: Section option
          side: Section option
          nodes: Map<string, string>
          language: bool
          hidden: bool }


    and Section =
        { links: Types.Link list }

        static member read name settings element =
            let element = read name settings element

            element
            |> Json.bind "hidden"
            |> Option.bind Json.tryDeserialize<bool>
            |> Option.defaultValue false
            |> function
                | true -> None
                | false ->
                    { links =
                        element
                        |> Json.bind "links"
                        |> Option.bind Json.tryDeserialize<Types.Link list>
                        |> Option.defaultValue [] }
                    |> Some

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
                let read name = read name settings element

                name,
                { language = read "language" |> Option.defaultValue false
                  hidden = read "hidden" |> Option.defaultValue false
                  display = read "display" |> Option.defaultValue (name.ToUpperInvariant())
                  top = Section.read "top" settings element
                  side = Section.read "side" settings element
                  nodes =
                    read "nodes"
                    |> Option.map (Json.toMap >> Map.map (fun _ y -> Json.str y))
                    |> Option.defaultValue Map.empty })
            |> Map.ofSeq

        member val current: string option = None with get, set

        member this.currentUI =
            this.current |> Option.bind (fun x -> this.supported |> Map.tryFind x)

        member this.str key =
            this.currentUI |> Option.bind _.nodes.TryFind(key) |> Option.defaultValue key

        member this.node = this.str >> str

        member this.attr attrName = this.str >> (attr attrName)

        member _.supported = values
