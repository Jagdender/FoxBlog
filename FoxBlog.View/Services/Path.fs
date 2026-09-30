namespace FoxBlog.View

module Path =
    let realname (supportedUIs: Types.UI list) (filename: string) =
        let name = filename |> System.IO.Path.GetFileNameWithoutExtension

        supportedUIs
        |> List.choose _.name
        |> List.tryFind (fun x -> name.IEndsWith $".{x}")
        |> Option.map (fun x -> name.Substring(0, name.Length - $".{x}".Length))
        |> Option.defaultValue name


    let tryGetUI (supportedUIs: Types.UI list) (filename: string) =
        let name = filename |> System.IO.Path.GetFileNameWithoutExtension

        supportedUIs
        |> List.tryFind (
            _.name
            >> function
                | Some ui -> name.IEndsWith $".{ui}"
                | None -> false
        )
        |> Option.orElse (supportedUIs |> List.tryFind (_.name >> Option.isNone))
