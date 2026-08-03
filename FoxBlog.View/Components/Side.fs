namespace FoxBlog.View


open Giraffe.ViewEngine
open FoxBlog.Types

type Side(settings: GlobalSettings, content: Content.Context, ui: UI.Context) =

    let side = ui.current |> Option.bind _.side

    let rec list (source: Content.Category) =
        let posts =
            source.posts |> List.map (fun x -> li [] [ a [ _href x.path ] [ str x.name ] ])

        let categories =
            source.categories
            |> List.map (fun x -> li [] [ details [] [ summary [] [ str x.name ]; ul [] (list x) ] ])

        posts @ categories

    let links = side |> Option.map _.links |> Option.defaultValue []

    member _.content =
        if Option.isNone side then
            str ""
        else

            let toLinkBtn link =
                a [ _class "button outline small"; _href link.url ] [ rawText link.name ]

            aside
                [ flag "data-sidebar" ]
                [ nav [] [ ul [] (list content.root) ]
                  footer [ _class "gap-1 vstack" ] (links |> List.map toLinkBtn) ]
