module Json

open System.Text.Json.Serialization
open System.Text.Json
open System.IO
open FoxBlog.View.Types

let tryParse (reader: byref<Utf8JsonReader>) =
    JsonElement.TryParseValue(&reader)
    |> function
        | false, _ -> None
        | true, element when element.HasValue |> not -> None
        | true, element -> Some element.Value

let tryRead file =
    if File.Exists file then
        let bytes = File.ReadAllBytes file
        let mutable reader = Utf8JsonReader bytes

        try
            match JsonElement.TryParseValue(&reader) with
            | true, value -> Some value.Value
            | false, _ -> None
        with :? JsonException ->
            None
    else
        None



let str (element: JsonElement) = element.ToString()

let contains (name: string) (element: JsonElement) =
    element.EnumerateObject() |> Seq.exists (fun e -> e.Name.IEquals name)


let map (name: string) (element: JsonElement) =
    try
        element.EnumerateObject()
        |> Seq.tryFind (fun e -> e.Name.IEquals name)
        |> Option.map _.Value
    with :? JsonException ->
        None

let rec mapMany (names: string list) (element: JsonElement) =
    match names with
    | [] -> Some element
    | head :: tail ->
        map head element
        |> function
            | Some element -> mapMany tail element
            | None -> None


let bind = map >> Option.bind

let bindMany names element = element |> Option.bind (mapMany names)

let toMap (element: JsonElement) =
    element.EnumerateObject() |> Seq.map (fun x -> (x.Name, x.Value)) |> Map.ofSeq

let toList (element: JsonElement) = element.EnumerateArray() |> List.ofSeq

let defaultWith (value: string) = JsonElement.Parse(value)

let empty = defaultWith "{}"


module private Converters =
    type LinksJsonConverter() =
        inherit JsonConverter<Link list>()

        override _.Read
            (reader: byref<Utf8JsonReader>, typeToConvert: System.Type, options: JsonSerializerOptions)
            : Link list =
            if base.CanConvert typeToConvert |> not then
                raise (System.NotSupportedException())
            else
                match reader.TokenType with
                | JsonTokenType.StartArray -> JsonSerializer.Deserialize<Link list>(&reader)
                | JsonTokenType.StartObject ->
                    JsonSerializer.Deserialize<Map<string, string>>(&reader)
                    |> Map.toList
                    |> List.map (fun (k, v) -> { name = k; url = v })
                | _ -> []

        override _.Write(_: Utf8JsonWriter, _: Link list, _: JsonSerializerOptions) : unit =
            raise (System.NotSupportedException())

    type UIJsonConverter() =
        inherit JsonConverter<UI>()

        override _.Read
            (reader: byref<Utf8JsonReader>, typeToConvert: System.Type, options: JsonSerializerOptions)
            : UI =
            if base.CanConvert typeToConvert |> not then
                raise (System.NotSupportedException())
            else
                JsonElement.TryParseValue(&reader)
                |> function
                    | true, e ->
                        e
                        |> Option.ofNullable
                        |> fun x ->
                            { name = x |> bind "name" |> Option.map str |> Option.orElse UI.defaultValue.name
                              language =
                                x
                                |> bind "language"
                                |> Option.map _.IsTrue
                                |> Option.defaultValue UI.defaultValue.language
                              nodes =
                                x
                                |> bind "nodes"
                                |> Option.map (toMap >> Map.map (fun _ v -> v |> str))
                                |> Option.defaultValue UI.defaultValue.nodes
                              display =
                                x
                                |> bind "display"
                                |> Option.map str
                                |> Option.orElse UI.defaultValue.display
                                |> Option.map _.ToUpperInvariant()
                              top =
                                x
                                |> bind "top"
                                |> Option.map (
                                    map "links"
                                    >> Option.map _.Deserialize<Link list>(options)
                                    >> Option.defaultValue []
                                    >> fun list -> { links = list }
                                )
                                |> Option.orElse UI.defaultValue.top
                              side =
                                x
                                |> bind "side"
                                |> Option.map (
                                    map "links"
                                    >> Option.map _.Deserialize<Link list>(options)
                                    >> Option.defaultValue []
                                    >> fun list -> { links = list }
                                )
                                |> Option.orElse UI.defaultValue.side }
                    | _, _ -> JsonException() |> raise


        override _.Write(_: Utf8JsonWriter, _: UI, _: JsonSerializerOptions) : unit =
            raise (System.NotSupportedException())



let options =
    JsonFSharpOptions
        .Default()
        .WithTypes(JsonFSharpTypes.Collections ||| JsonFSharpTypes.OptionalTypes)
        .ToJsonSerializerOptions()
    |> fun x ->
        x.PropertyNameCaseInsensitive <- true
        x.Converters.Insert(0, Converters.UIJsonConverter())
        x.Converters.Insert(0, Converters.LinksJsonConverter())
        x

let tryDeserialize<'T> (element: JsonElement) : 'T option =
    try
        element.Deserialize<'T>(options) |> Some
    with :? JsonException ->
        None

let serialize (value: 'T) =
    JsonSerializer.Serialize(value, options)
