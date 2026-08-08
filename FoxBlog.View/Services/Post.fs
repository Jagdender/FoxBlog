namespace FoxBlog.View

open System.IO

module Post =

    module Metadata =

        [<Literal>]
        let configExt = ".conf"

        module private Mapper =
            let rec private ext (path: string) =
                seq {
                    match Path.GetExtension path with
                    | e when System.String.IsNullOrWhiteSpace e -> ()
                    | e ->
                        yield e |> _.TrimStart('.')
                        yield! path |> Path.GetFileNameWithoutExtension |> ext
                }

            let private name (path: string) =
                path |> Path.GetFileNameWithoutExtension |> _.Split('.') |> Array.item 0

            type private DicContext = { ui: string option; name: string }


            let private enumerate (dic: DirectoryInfo) =
                dic.EnumerateDirectories()
                |> Seq.map (fun x -> dic.EnumerateFiles() |> Seq.tryFind _.Name.IEquals($"{x.Name}.json"))
                |> Seq.map (
                    Option.map (fun file ->
                        file.FullName
                        |> Json.tryRead
                        |> Json.bind "name"
                        |> Option.bind Json.tryDeserialize<string>
                        |> Option.defaultValue (name file.Name)
                        |> (fun x -> { ui = None; name = x }))
                )
                |> Seq.map (Option.defaultValue { ui = None; name = "" })





        type Context(settings: Global.Settings, ui: UI.Context) =

            let directory =
                settings.Root
                |> Directory.EnumerateDirectories
                |> Seq.tryFind (Path.GetFileName >> _.IEquals("posts"))

            let files =
                directory
                |> Option.map (fun root ->
                    Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories)
                    |> Seq.map (fun path -> Path.GetRelativePath(root, path)))
                |> Option.defaultValue Seq.empty

    module Content =
        ()
