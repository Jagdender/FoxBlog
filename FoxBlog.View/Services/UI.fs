namespace FoxBlog.View

open Giraffe.ViewEngine
open System.IO
open FoxBlog.Types

module UI =
    let private read name settings element =
        element
        |> Json.map name
        |> Option.orElse (settings.Json |> Json.mapMany [ "ui"; name ])
        |> Option.bind Json.tryDeserialize

    type Type =
        { name: string
          display: string
          top: Section option
          side: Section option
          node: string -> string
          language: bool
          hidden: bool }


    and Section =
        { links: Link list }

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
                        |> Option.bind Json.tryDeserialize<Link list>
                        |> Option.defaultValue [] }
                    |> Some

    type Context(settings: GlobalSettings) =

        let values =
            settings.Root
            |> Directory.EnumerateDirectories
            |> Seq.tryFind (Path.GetFileName >> _.IEquals("ui"))
            |> Option.map (
                Directory.EnumerateFiles
                >> Seq.filter (Path.GetExtension >> _.IEquals(".conf"))
                >> Seq.map (fun x ->
                    (Path.GetFileNameWithoutExtension(x).ToLowerInvariant(),
                     Json.tryRead x |> Option.defaultValue Json.empty))
            )
            |> Option.defaultValue Seq.empty
            |> Seq.map (fun (name, element) ->
                let read name = read name settings element

                { name = name
                  language = read "language" |> Option.defaultValue false
                  hidden = read "hidden" |> Option.defaultValue false
                  display = read "display" |> Option.defaultValue (name.ToUpperInvariant())
                  top = Section.read "top" settings element
                  side = Section.read "side" settings element
                  node = fun x -> read "nodes" |> Json.bind x |> Option.map Json.str |> Option.defaultValue x })
            |> Seq.toList

        member val current: Type option = None with get, set

        member this.str key =
            this.current |> Option.map _.node(key) |> Option.defaultValue key

        member this.node = this.str >> str

        member this.attr attrName = this.str >> (attr attrName)

        member this.asLang = this.current |> Option.map _.language |> Option.defaultValue false

        member _.supported = values
