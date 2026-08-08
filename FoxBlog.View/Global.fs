module Global

type Settings =
    { Root: string
      Json: System.Text.Json.JsonElement }

[<Literal>]
let Filename = "settings"
