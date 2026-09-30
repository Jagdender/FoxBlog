namespace FoxBlog.View

open System.IO

module Content =

    type Post = { name: string; path: string }

    type Category =
        { name: string
          categories: Category list
          posts: Post list }

    type Context(uiCtx: UI.Context, settings: Global.Settings) =
        let directory =
            settings.Root
            |> Directory.EnumerateDirectories
            |> Seq.tryFind (Path.GetFileName >> _.IEquals("posts"))

        let real path =
            DirectoryInfo(path)
            |> _.ResolveLinkTarget(true)
            |> Option.ofObj
            |> Option.map _.FullName
            |> Option.defaultValue (DirectoryInfo(path).FullName)

        let toPost =
            Seq.groupBy (Path.realname uiCtx.supported)
            >> Seq.map (fun (name, paths) ->
                match Seq.toList paths with
                | [ path ] -> path
                | paths ->
                    paths
                    |> List.map (fun path -> path, Path.tryGetUI uiCtx.supported path)
                    |> List.filter (function
                        | _, Some { name = None } -> true
                        | _, ui -> ui = uiCtx.current)
                    |> function
                        | [ path, _ ] -> path
                        | paths ->
                            paths
                            |> List.pick (fun (path, ui) -> ui |> Option.bind _.name |> Option.map (fun _ -> path))
                |> fun path -> (directory.Value, path)
                |> Path.GetRelativePath
                |> _.Split(Path.DirectorySeparatorChar, System.StringSplitOptions.RemoveEmptyEntries)
                |> String.concat "/"
                |> fun x -> { name = name; path = $"/{x}" })

        let mutable set = Set.empty

        let iter action source =
            Seq.iter action source
            source

        let rec enumerate (path: string) = //PERF: iter ?
            let categories =
                Directory.EnumerateDirectories path
                |> Seq.map real
                |> Seq.filter (set.Contains >> not)
                |> iter (fun x -> set <- set.Add x)
                |> Seq.map enumerate
                |> Seq.toList

            let posts =
                Directory.EnumerateFiles path
                |> Seq.map real
                |> Seq.filter (set.Contains >> not)
                |> iter (fun x -> set <- set.Add x)
                |> Seq.filter _.IEndsWith($".md")
                |> toPost
                |> Seq.toList

            { name = Path.GetFileName path
              categories = categories
              posts = posts }

        member val path = "/" with get, set

        member val current: Post option = None with get, set

        member _.root =
            directory
            |> Option.map enumerate
            |> Option.defaultValue
                { name = "posts"
                  categories = []
                  posts = [] }
