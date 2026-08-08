namespace FoxBlog.View

open Giraffe.ViewEngine

type Index(context: UI.Context, top: Top, side: Side, main: Main, head: Head) =

    let attributes =
        match context.current with
        | None -> []
        | Some value ->
            [ yield attr "ui" value
              match context.currentUI with
              | Some ui when ui.language -> yield attr "lang" value
              | _ -> () ]

    let body =
        let layout =
            context.currentUI
            |> Option.bind _.side
            |> Option.map (fun _ -> "data-sidebar-layout")
            |> Option.defaultValue ""
            |> flag

        body [ layout ] [ top.content; side.content; main.content ]

    member _.Html = html attributes [ head.content; body ]
