namespace FoxBlog.View

open Giraffe.ViewEngine
open Giraffe.ViewEngine.Extensions

module private SVG =
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
            emptyText
        else

            let uiBtn =
                ui.currentUI
                |> Option.bind _.display
                |> Option.orElse ui.current
                |> Option.map (
                    _.ToUpperInvariant()
                    >> str
                    >> List.singleton
                    >> dropdown [ _class "ghost small"; flag "ui" ]
                    >> fun x ->
                        ui.supported
                        |> Map.toSeq
                        |> Seq.map (fun (name, ui) ->
                            a
                                [ role "menuitem"; _href ($"/{name}{content.path}") ]
                                [ ui.display |> Option.defaultValue name |> _.ToUpperInvariant() |> str ])
                        |> Seq.toList
                        |> x
                )
                |> Option.defaultValue emptyText




            let themeBtn =
                span
                    []
                    [ button
                          [ _class "ghost"; _onclick "toggleTheme()" ]
                          [ span [ _class "icon-dark" ] [ str "🌙" ]
                            span [ _class "icon-light" ] [ str "☀️" ] ] ]

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
