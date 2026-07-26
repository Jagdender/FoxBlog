namespace FoxBlog.View

open Giraffe.ViewEngine
open System.IO

module UI =

    type UI =
        { name: string
          display: string
          defaultUI: bool
          config: System.Text.Json.JsonElement }

    type Context(settings: GlobalSettings, context: ViewContext) =

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
                  defaultUI =
                    element
                    |> Json.tryFind "default"
                    |> Option.map Json.deserialize<bool>
                    |> Option.defaultValue false
                  display =
                    element
                    |> Json.tryFind "display"
                    |> Option.map _.ToString()
                    |> Option.defaultValue (name.ToUpperInvariant()) })
            |> Seq.toList

        member this.current =
            context.ui
            |> Option.bind (fun x -> values |> Seq.tryFind _.name.IEquals(x))
            |> Option.orElse this.defaultUI

        member this.str key =
            this.current
            |> Option.bind (fun x -> x.config |> Json.tryFind "nodes")
            |> Option.bind (Json.tryFind key >> Option.map _.ToString())
            |> Option.defaultValue key

        member this.node = this.str >> str

        member this.attr attrName = this.str >> (attr attrName)

        member this.asLang =
            this.current
            |> Option.bind (fun x -> x.config |> Json.tryFind "language")
            |> Option.map Json.deserialize<bool>
            |> Option.defaultValue false


        member _.defaultUI = values |> Seq.tryFind _.defaultUI

        member _.supported = values
