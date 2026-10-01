namespace FoxBlog.View

open System.IO

module Content =

    type Post =
        { name: string
          path: string
          filename: string }

    type Category =
        { name: string
          categories: Category list
          posts: Post list }

    type Context(uiCtx: UI.Context, settings: Global.Settings) =
        let directory = settings.Root

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
                |> fun path -> Path.GetRelativePath(directory, path)
                |> _.Split(Path.DirectorySeparatorChar, System.StringSplitOptions.RemoveEmptyEntries)
                |> String.concat "/"
                |> fun x ->
                    { name = name
                      path = $"/{x}"
                      filename = Path.Combine(directory, x) })

        let mutable linkSet = Set.empty

        let resolve =
            Seq.map real
            >> Seq.filter (linkSet.Contains >> not)
            >> Seq.tee (fun x -> linkSet <- linkSet.Add x)

        let rec enumerate (path: string) = //PERF: iter ?
            let categories =
                Directory.EnumerateDirectories path
                |> resolve
                |> Seq.map enumerate
                |> Seq.toList

            let posts =
                Directory.EnumerateFiles path
                |> Seq.filter _.IEndsWith($".md")
                |> resolve
                |> toPost
                |> Seq.toList

            { name = Path.GetFileName path
              categories = categories
              posts = posts }

        member this.mapToPost(path: string) =
            let find category name =
                category |> Option.bind (_.categories >> List.tryFind _.name.IEquals(name))

            path
            |> _.TrimEnd(".md").Split('/', System.StringSplitOptions.RemoveEmptyEntries)
            |> function
                | [||] -> None
                | [| item |] -> this.root.posts |> List.tryFind _.name.IEquals(item)
                | array ->
                    Array.sub array 0 (array.Length - 1)
                    |> Array.fold find (Some this.root)
                    |> Option.bind (_.posts >> List.tryFind _.name.IEquals(array |> Array.last))


        member val current: Post option = None with get, set

        member _.root = directory |> enumerate
