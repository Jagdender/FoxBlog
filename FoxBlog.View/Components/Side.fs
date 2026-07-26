namespace FoxBlog.View


module private SideExtensions =
    type GlobalSettings with
        member setting.links =
            setting.Json
            |> Json.tryFind "side"
            |> Option.bind (Json.tryFind "links")
            |> Option.defaultValue Json.empty
            |> Json.deserialize<Types.Links>
            |> _.toList()


open Giraffe.ViewEngine
open SideExtensions

type Side(settings: GlobalSettings, categoriesCtx: Content.Context) =

    let rec list (source: Content.Category) =
        let posts =
            source.posts |> List.map (fun x -> li [] [ a [ _href x.path ] [ str x.name ] ])

        let categories =
            source.categories
            |> List.map (fun x -> li [] [ details [] [ summary [] [ str x.name ]; ul [] (list x) ] ])

        posts @ categories

    member _.content =
        let toLinkBtn (link: Types.Link) =
            a [ _class "button outline small"; _href link.url ] [ rawText link.name ]

        aside
            [ flag "data-sidebar" ]
            [ nav [] [ ul [] (list categoriesCtx.root) ]
              footer [ _class "gap-1 vstack" ] (settings.links |> List.map toLinkBtn) ]
