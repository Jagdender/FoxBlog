namespace FoxBlog.View

open Giraffe.ViewEngine
open Giraffe.ViewEngine.Extensions
open FoxBlog.Types

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

type Top(ui: UI.Context) =

    let links = ui.current |> Option.map _.top.links |> Option.defaultValue []

    let fullLinks =
        menu
            [ _class "buttons items-center" ]
            (links
             |> List.map (fun link -> li [] [ a [ _class "button ghost small"; _href link.url ] [ str link.name ] ]))

    let dropdownLinks =
        dropdown
            { id = "top-links-menu"
              attributes = [ _class "outline small" ]
              contents = [ ui.node "LINKS"; SVG.LinkBtn ] }
            (links
             |> List.map (fun link -> li [] [ a [ attr "role" "menuitem"; _href link.url ] [ str link.name ] ]))

    let uis =
        ui.supported
        |> List.map (fun ui ->
            a
                [ attr "role" "menuitem"
                  attr "data-ui-menuitem" ui.name
                  _onclick $"toggleUI('{ui.name}')" ]
                [ rawText (ui.display) ])

    member _.content =
        let uiBtn =
            if ui.supported.Length <= 1 then
                span [] []
            else
                dropdown
                    { id = "ui-menu"
                      attributes = [ _class "ghost small"; _id "ui-button" ]
                      contents = [] }
                    uis

        let themeBtn =
            span [] [ button [ _class "ghost small"; _onclick "toggleTheme()" ] [ SVG.ThemeBtn ] ]

        let linkBtns =
            match links with
            | [] -> span [] []
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
