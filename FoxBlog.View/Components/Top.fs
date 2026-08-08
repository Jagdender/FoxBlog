namespace FoxBlog.View

open Giraffe.ViewEngine
open Giraffe.ViewEngine.Extensions

module private SVG =
    let ThemeBtn =
        rawText
            """
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="icon-light" role="img" aria-label="Light mode" data-darkreader-inline-stroke="" style="--darkreader-inline-stroke: currentColor;">
                <circle cx="12" cy="12" r="5"></circle>
                <line x1="12" y1="1" x2="12" y2="3"></line><line x1="12" y1="21" x2="12" y2="23"></line>
                <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"></line><line x1="18.36" y1="18.36" x2="19.78" y2="19.78"></line>
                <line x1="1" y1="12" x2="3" y2="12"></line><line x1="21" y1="12" x2="23" y2="12"></line>
                <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"></line><line x1="18.36" y1="5.64" x2="19.78" y2="4.22"></line>
            </svg>

            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="icon-dark" role="img" aria-label="Dark mode" data-darkreader-inline-stroke="" style="--darkreader-inline-stroke: currentColor;">
                <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
            </svg>
            """

    let LinkBtn =
        rawText
            """
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="m6 9 6 6 6-6"></path></svg>
            """

type Top(ui: UI.Context, content: Content.Context) =

    let top = ui.currentUI |> Option.bind _.top

    let links = top |> Option.map _.links |> Option.defaultValue []

    let fullLinks =
        links
        |> List.map (fun link -> li [] [ a [ _class "button ghost small"; _href link.url ] [ str link.name ] ])
        |> menu [ _class "buttons items-center" ]

    let dropdownLinks =
        links
        |> List.map (fun link -> a [ role "menuitem"; _href link.url ] [ str link.name ])
        |> dropdown [ _class "outline small" ] [ ui.node "LINKS"; SVG.LinkBtn ]


    member _.content =
        if Option.isNone top then
            str ""
        else

            let redirect ui = $"/{ui}{content.path}"

            let uiBtn =
                ui.currentUI
                |> Option.map (fun x -> [ str x.display ])
                |> Option.defaultValue []
                |> dropdown [ _class "ghost small"; flag "ui" ]
                |> fun x ->
                    ui.supported
                    |> Map.toSeq
                    |> Seq.map (fun (name, ui) -> a [ role "menuitem"; _href (redirect name) ] [ str ui.display ])
                    |> Seq.toList
                    |> x

            let themeBtn =
                span [] [ button [ _class "ghost small"; _onclick "toggleTheme()" ] [ SVG.ThemeBtn ] ]

            let linkBtns =
                match links with
                | [] -> str ""
                | [ _ ] -> fullLinks
                | _ when links.Length > 5 -> dropdownLinks
                | _ -> span [ flag "links" ] [ fullLinks; dropdownLinks ]

            nav
                [ flag "data-topnav" ]
                [ div
                      [ _class "items-center hstack"; flag "left" ]
                      [ button [ flag "data-sidebar-toggle" ] [ ui.node "MENU" ]
                        a [ _href "/" ] [ ui.node "HOME" ] ]
                  div [ _class "col-end justify-end hstack"; flag "right" ] [ linkBtns; uiBtn; themeBtn ] ]
