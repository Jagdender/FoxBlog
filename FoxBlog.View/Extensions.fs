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

type System.Text.Json.JsonElement with
    member inline this.IsTrue = this.ValueKind = System.Text.Json.JsonValueKind.True
    member inline this.IsFalse = this.ValueKind = System.Text.Json.JsonValueKind.False
