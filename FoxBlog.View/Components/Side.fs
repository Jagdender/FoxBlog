namespace FoxBlog.View


open Giraffe.ViewEngine

type Side(settings: Global.Settings, content: Content.Context, ui: UI.Context) =

    let side = ui.currentUI |> Option.bind _.side

    let rec list (source: Content.Category) =
        let posts =
            source.posts |> List.map (fun x -> li [] [ a [ _href x.path ] [ str x.name ] ])

        let categories =
            source.categories
            |> List.map (fun x -> li [] [ details [] [ summary [] [ str x.name ]; ul [] (list x) ] ])

        posts @ categories


    member _.content =
        match side with
        | None -> str ""
        | Some side ->

            let toLinkBtn (link: Types.Link) =
                a [ _class "button outline small"; _href link.url ] [ rawText link.name ]

            aside
                [ flag "data-sidebar" ]
                [ nav [] [ ul [] (list content.root) ]
                  footer [ _class "gap-1 vstack" ] (side.links |> List.map toLinkBtn) ]
