namespace FoxBlog.Frontend

module Script =
    open Browser
    open Fable.Core.JsInterop

    [<AutoOpen>]
    module private Extensions =
        type Types.DOMStringMap with
            member map.get(name: string) =
                let data = map?(name)
                if isNullOrUndefined data then None else Some(data: string)

            member map.set (name: string) (value: string) = map?(name) <- value

        type Types.Element with
            member element.style
                with set (value: string) = element?style <- value

    module private Cookie =
        let set (name: string) (value: string) =
            document.cookie <- $"{name}={value}; Path=/; Max-Age=31536000;"

        let get (name: string) =
            document.cookie.Split(';')
            |> Array.tryFind (fun cookie -> cookie.StartsWith $"{name}=")
            |> Option.map (fun x -> x.Split('=') |> Array.get <| 1)

    module private Meta =
        let metas =
            document.head.getElementsByTagName "meta"
            |> Fable.Core.JS.Constructors.Array.from<Types.Element>

        let getName name =
            metas
            |> Array.tryFind (fun meta -> meta.getAttribute "name" = name)
            |> Option.map (fun meta -> meta.getAttribute "content")

        let getProperty property =
            metas
            |> Array.tryFind (fun meta -> meta.getAttribute "property" = property)
            |> Option.map (fun meta -> meta.getAttribute "content")





    let toggleTheme () =
        let theme =
            localStorage.getItem "theme"
            |> function
                | "dark" -> "light"
                | _ -> "dark"

        let html = document.documentElement
        html?style?colorScheme <- theme
        html.dataset.set "theme" theme
        localStorage.setItem ("theme", theme)

    module private StartUp =
        let initTheme =
            localStorage.getItem "theme"
            |> fun theme ->
                let html = document.documentElement
                html?style?colorScheme <- theme
                html.setAttribute ("data-theme", theme)
