namespace FoxBlog.View

open Giraffe.ViewEngine

module private Head =

    let private misc =
        rawText
            """
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <link rel="stylesheet" href="https://unpkg.com/@knadh/oat/oat.min.css">
            <link rel="stylesheet" href="/style.css">
            <script src="https://unpkg.com/@knadh/oat/oat.min.js" defer></script>
            <script type="module" src="/main.js" defer></script>
            """

    let content = head [] [ misc; title [] [] ]

module private Main =
    let content (ui: UI.Context) (content: Content.Context) (settings: Global.Settings) =
        async {
            let! text =
                content.current
                |> Option.map (_.filename >> System.IO.File.ReadAllTextAsync)
                |> Option.defaultValue (System.Threading.Tasks.Task.FromResult("404")) // TODO: default 404 content
                |> Async.AwaitTask

            let markdown = text |> Md.parse |> Md.toHtml
            return main [] [ div [ _class "container" ] [ h1 [] [ rawText markdown ] ] ]
        }

module private Top =
    let private LinkBtn =
        rawText
            """
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="m6 9 6 6 6-6"></path></svg>
            """

    let private fullLinks (links: Types.Link list) =
        links
        |> List.map (fun link -> li [] [ a [ _class "button ghost small"; _href link.url ] [ str link.name ] ])
        |> menu [ _class "buttons items-center" ]

    let private dropdownLinks (ui: UI.Context) (links: Types.Link list) =
        links
        |> List.map (fun link -> a [ role "menuitem"; _href link.url ] [ str link.name ])
        |> dropdown [ _class "outline small" ] [ ui.node "LINKS"; LinkBtn ]

    let private uiBtn (ui: UI.Context) (content: Content.Context) =
        ui.current
        |> Option.bind _.display
        |> Option.map (fun currentDisplay ->
            let items =
                ui.supported
                |> List.map (fun ui ->
                    match ui.display with
                    | Some display ->
                        let name = ui.name |> Option.map _.Trim('/') |> Option.defaultValue ""

                        let path =
                            content.current |> Option.map _.path.TrimStart('/') |> Option.defaultValue ""

                        let href =
                            System.IO.Path.Combine("/", name, path)
                            |> String.map (fun c -> if c = System.IO.Path.DirectorySeparatorChar then '/' else c)

                        a [ role "menuitem"; _href href ] [ display |> str ]
                    | None -> emptyText)

            dropdown [ _class "ghost small" ] [ str currentDisplay ] items)
        |> Option.defaultValue emptyText

    let private themeBtn =
        span
            []
            [ button
                  [ _class "ghost"; _onclick "toggleTheme()" ]
                  [ span [ _class "icon-dark" ] [ str "🌙" ]
                    span [ _class "icon-light" ] [ str "☀️" ] ] ]

    let content (ui: UI.Context) (content: Content.Context) =
        ui.current
        |> Option.bind _.top
        |> Option.map (fun top ->
            let linkBtns =
                match top.links with
                | [] -> emptyText
                | [ _ ] -> fullLinks top.links
                | links when links.Length > 5 -> dropdownLinks ui links
                | links -> span [ flag "links" ] [ fullLinks links; dropdownLinks ui links ]

            let uiName = ui.current |> Option.bind _.name |> Option.defaultValue ""

            let href = $"/{uiName}"

            nav
                [ flag "data-topnav" ]
                [ div
                      [ _class "items-center hstack"; flag "left" ]
                      [ button [ flag "data-sidebar-toggle" ] [ ui.node "MENU" ]
                        a [ _href href ] [ ui.node "HOME" ] ]
                  div [ _class "col-end justify-end hstack"; flag "right" ] [ linkBtns; uiBtn ui content; themeBtn ] ])
        |> Option.defaultValue emptyText

module private Side =

    let rec list (ui: Types.UI option) (source: Content.Category) =
        let hrefWithUI path =
            ui
            |> Option.bind _.name
            |> Option.map (fun x -> $"/{x}{path}")
            |> Option.defaultValue path
            |> _href

        let posts =
            source.posts
            |> List.map (fun x -> li [] [ a [ hrefWithUI x.path ] [ str x.name ] ])

        let categories =
            source.categories
            |> List.map (fun x -> li [] [ details [] [ summary [] [ str x.name ]; ul [] (list ui x) ] ])

        posts @ categories

    let content (ui: UI.Context) (content: Content.Context) =
        ui.current
        |> Option.bind _.side
        |> Option.map (fun side ->
            let inline toLinkBtn (link: Types.Link) =
                a [ _class "button outline small"; _href link.url ] [ rawText link.name ]

            aside
                [ flag "data-sidebar" ]
                [ nav [] [ ul [] (list ui.current content.root) ]
                  footer [ _class "gap-1 vstack" ] (side.links |> List.map toLinkBtn) ])
        |> Option.defaultValue emptyText


type Index(ui: UI.Context, content: Content.Context, settings: Global.Settings) =

    let attributes =
        match ui.current with
        | None -> []
        | Some value ->
            [ match ui.current with
              | Some ui when ui.language ->
                  match ui.name with
                  | Some name -> yield attr "lang" name
                  | None -> ()
              | _ -> () ]

    let body =
        let layout =
            ui.current
            |> Option.bind _.side
            |> Option.map (fun _ -> "data-sidebar-layout")
            |> Option.defaultValue ""
            |> flag

        let top = Top.content ui content
        let side = Side.content ui content
        let main = Main.content ui content settings |> Async.RunSynchronously

        body [ layout ] [ top; side; main ]

    member _.Html = html attributes [ Head.content; body ]
