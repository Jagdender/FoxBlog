[<AutoOpen>]
module Extensions

type System.String with
    member inline this.IEquals other =
        System.String.Equals(this, other, System.StringComparison.InvariantCultureIgnoreCase)

    member inline this.IEndsWith other =
        this.EndsWith(other, System.StringComparison.InvariantCultureIgnoreCase)

    member inline this.IStartsWith other =
        this.StartsWith(other, System.StringComparison.InvariantCultureIgnoreCase)

    member inline this.TrimEnd(other: string) =
        match this.EndsWith(other) with
        | true -> this[.. (this.LastIndexOf(other) - 1)]
        | false -> this

    member inline this.TrimStart(other: string) =
        match this.StartsWith(other) with
        | true -> this[(other.Length) ..]
        | false -> this

module Json =
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

    let toMap (element: JsonElement) =
        element.EnumerateObject() |> Seq.map (fun x -> (x.Name, x.Value)) |> Map.ofSeq

    let toList (element: JsonElement) = element.EnumerateArray() |> List.ofSeq

    let defaultWith (value: string) = JsonElement.Parse(value)

    let empty = defaultWith "{}"

    module private rec Converters =
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
                    | _ -> raise (JsonException())

            override _.Write(_: Utf8JsonWriter, _: Link list, _: JsonSerializerOptions) : unit =
                raise (System.NotSupportedException())




    let options =
        JsonFSharpOptions
            .Default()
            .WithTypes(JsonFSharpTypes.Collections ||| JsonFSharpTypes.OptionalTypes)
            .ToJsonSerializerOptions()
        |> fun x ->
            x.PropertyNameCaseInsensitive <- true
            x.Converters.Insert(0, Converters.LinksJsonConverter())
            x

    let tryDeserialize<'T> (element: JsonElement) =
        try
            element.Deserialize<'T>(options) |> Some
        with :? JsonException ->
            None

    let serialize (value: 'T) =
        JsonSerializer.Serialize(value, options)
