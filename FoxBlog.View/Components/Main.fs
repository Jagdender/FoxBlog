namespace FoxBlog.View

open Giraffe.ViewEngine

type Main() =
    member _.content = main [] [ div [ _class "container" ] [ h1 [] [ str "👋" ] ] ]
