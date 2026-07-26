namespace FoxBlog.View

open System.Text.Json.Serialization
open System.Text.Json

module rec Types =
    [<JsonConverter(typeof<Converters.LinksJsonConverter>)>]
    type Links =
        | List of Link list
        | Map of Map<string, string>

        member this.toList() =
            match this with
            | List links -> links
            | Map links ->
                links
                |> Seq.map (fun pair -> { name = pair.Key; url = pair.Value })
                |> Seq.toList

    and Link = { name: string; url: string }


    type Post =
        { contentPath: string
          title: string
          category: string option
          tags: string list
          hidden: bool
          password: string option
          datetime: System.DateTime option }





    module private Converters =
        type LinksJsonConverter() =
            inherit JsonConverter<Links>()

            override _.Read
                (reader: byref<Utf8JsonReader>, typeToConvert: System.Type, options: JsonSerializerOptions)
                : Links =
                if base.CanConvert typeToConvert |> not then
                    raise (System.NotSupportedException())
                else
                    match reader.TokenType with
                    | JsonTokenType.StartArray -> JsonSerializer.Deserialize<List<Link>>(&reader, options) |> List
                    | JsonTokenType.StartObject ->
                        JsonSerializer.Deserialize<Map<string, string>>(&reader, options) |> Map
                    | _ -> raise (JsonException())

            override _.Write(_: Utf8JsonWriter, _: Links, _: JsonSerializerOptions) : unit =
                raise (System.NotSupportedException())
