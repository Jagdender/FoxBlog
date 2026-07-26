namespace FoxBlog.Frontend

open FoxBlog

module Script =
    open Browser
    open System
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

    module private UI =

        let private data =
            document.getElementById("data-ui").textContent |> Fable.Core.JS.JSON.parse :?> Types.UI

        let supported = data.supported
        let defaultUI = data.defaultUI




    let redirect url = window.location.href <- url

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


    let toggleUI target =
        let chosenOne = UI.supported |> Array.find (fun x -> x.name = target)

        document.getElementById "ui-button"
        |> function
            | x when isNullOrUndefined x -> ()
            | x -> x.innerHTML <- chosenOne.display

        localStorage.setItem ("ui", target)

        document.documentElement.getAttribute "ui"
        |> function
            | current when not (current = target) || isNullOrUndefined (current) ->
                let parts =
                    window.location.pathname.Split('/', StringSplitOptions.RemoveEmptyEntries)

                let pathname =
                    parts
                    |> Array.tryItem 0
                    |> Option.map (fun x -> x.Equals(current, StringComparison.InvariantCultureIgnoreCase))
                    |> Option.defaultValue (false)
                    |> function
                        | false -> parts |> Array.insertAt 0 target
                        | true ->
                            parts[0] <- target
                            parts
                    |> String.concat "/"

                window.location.pathname <- pathname

            | _ -> ()


    module private StartUp =
        let initTheme =
            localStorage.getItem "theme"
            |> fun theme ->
                let html = document.documentElement
                html?style?colorScheme <- theme
                html.setAttribute ("data-theme", theme)

        let initUI =
            document.documentElement.getAttribute "ui"
            |> function
                | x when isNullOrUndefined (x) -> ()
                | x -> toggleUI (x)
