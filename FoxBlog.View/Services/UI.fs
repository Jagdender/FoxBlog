namespace FoxBlog.View

open Giraffe.ViewEngine
open System.IO
open FoxBlog.Types

module UI =

    type Type =
        { name: string
          display: string
          top: Section
          side: Section
          config: System.Text.Json.JsonElement }

    and Section =
        { links: Link list
          hidden: bool }

        static member read name uisetting globalsetting =
            { links =
                uisetting
                |> Json.map name
                |> Option.orElse (globalsetting |> Json.map name)
                |> Option.bind (Json.map "links")
                |> Option.map Json.tryDeserialize<Link list>
                |> Option.defaultValue []
              hidden = //PERF: eval "hidden" first then skip? "links"
                uisetting
                |> Json.map name
                |> Option.bind (Json.map "hidden")
                |> Option.map Json.tryDeserialize<bool>
                |> Option.defaultValue false }

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
                { name = name
                  config = element
                  display =
                    element
                    |> Json.map "display"
                    |> Option.map _.ToString()
                    |> Option.defaultValue (name.ToUpperInvariant())
                  top = Section.read "top" element settings.Json
                  side = Section.read "side" element settings.Json })
            |> Seq.toList

        member val current = None with get, set

        member this.str key =
            this.current
            |> Option.bind (fun x -> x.config |> Json.map "nodes")
            |> Option.bind (Json.map key >> Option.map _.ToString())
            |> Option.defaultValue key

        member this.node = this.str >> str

        member this.attr attrName = this.str >> (attr attrName)

        member this.asLang =
            this.current
            |> Option.bind (fun x -> x.config |> Json.map "language")
            |> Option.map Json.tryDeserialize<bool>
            |> Option.defaultValue false

        member _.supported = values
