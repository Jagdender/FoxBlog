namespace FoxBlog.View

open Giraffe.ViewEngine

type Index(context: UI.Context, settings: GlobalSettings, top: Top, side: Side, main: Main, head: Head) =

    let attributes =
        match context.current with
        | None -> [ flag "" ]
        | Some ui when context.asLang -> [ attr "ui" ui.name; attr "lang" ui.name ]
        | Some ui -> [ attr "ui" ui.name ]



    let body =
        let top =
            context.current
            |> Option.map (fun x -> x.config |> Json.contains "top")
            |> Option.defaultValue false
            |> function
                | true -> top.content
                | false -> str ""

        let side =
            context.current
            |> Option.map (fun x -> x.config |> Json.contains "side")
            |> Option.defaultValue false
            |> function
                | true -> side.content
                | false -> str ""

        let layout =
            context.current
            |> Option.map (fun x -> x.config |> Json.contains "side")
            |> Option.defaultValue false
            |> function
                | true -> "data-sidebar-layout"
                | false -> ""
            |> flag

        body [ layout ] [ top; side; main.content ]

    member _.Html = html attributes [ head.content; body ]
