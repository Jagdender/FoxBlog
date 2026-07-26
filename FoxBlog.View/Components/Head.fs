namespace FoxBlog.View

open Giraffe.ViewEngine
open FoxBlog.Types

module private Meta =

    let withMeta (attrName: string) (attrValue: string) (content: string) =
        meta [ attr attrName attrValue; _content content ]

    let withName (name: string) (content: string) = withMeta "name" name content
    let withProperty (property: string) (content: string) = withMeta "property" property content

module private Script =
    let withJson id content =
        script [ _id id; _type "application/json" ] [ rawText content ]

type Head(context: UI.Context) =
    let misc =
        rawText
            """
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <link rel="stylesheet" href="https://unpkg.com/@knadh/oat/oat.min.css">
            <link rel="stylesheet" href="/style.css">
            <script src="https://unpkg.com/@knadh/oat/oat.min.js" defer></script>
            <script type="module" src="/main.js" defer></script>
            """

    let ui =
        { supported = context.supported |> List.map UIdto.map
          defaultUI = context.defaultUI |> Option.map UIdto.map }

    member _.content = head [] [ misc; title [] [] ]
