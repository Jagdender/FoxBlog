namespace FoxBlog.View

open System.IO

module Content =

    type Post = { name: string; path: string }

    type Category =
        { name: string
          categories: Category list
          posts: Post list }

    type Context(settings: Global.Settings) =
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

        let toPost name =
            Path.GetRelativePath(directory.Value, name)
            |> _.Split(Path.DirectorySeparatorChar, System.StringSplitOptions.RemoveEmptyEntries)
            |> String.concat "/"
            |> fun x ->
                { name = Path.GetFileName name
                  path = $"/{x}" }

        let mutable set = Set.empty

        let iter action source =
            Seq.iter action source
            source

        let rec enumerate (path: string) = //PERF: iter ?
            let categories =
                Directory.EnumerateDirectories path
                |> Seq.map real
                |> Seq.filter (set.Contains >> not)
                |> Seq.filter (Directory.EnumerateDirectories >> Seq.isEmpty >> not)
                |> iter (fun x -> set <- set.Add x)
                |> Seq.map enumerate
                |> Seq.toList

            let posts =
                Directory.EnumerateDirectories path
                |> Seq.map real
                |> Seq.filter (Directory.EnumerateDirectories >> Seq.isEmpty)
                |> Seq.map toPost
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
