namespace FoxBlog.View

open Giraffe.ViewEngine

type Index(context: UI.Context, top: Top, side: Side, main: Main, head: Head) =

    let attributes =
        match context.current with
        | None -> [ flag "" ]
        | Some ui when context.asLang -> [ attr "ui" ui.name; attr "lang" ui.name ]
        | Some ui -> [ attr "ui" ui.name ]

    let body =
        let layout =
            context.current
            |> Option.bind _.side
            |> Option.map (fun _ -> "data-sidebar-layout")
            |> Option.defaultValue ""
            |> flag

        body [ layout ] [ top.content; side.content; main.content ]

    member _.Html = html attributes [ head.content; body ]
