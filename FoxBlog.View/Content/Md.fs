namespace FoxBlog.View

open Markdig
open Markdig.Extensions.Yaml
open Markdig.Syntax
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions


module Md =
    let private pipeline =
        MarkdownPipelineBuilder().UseAdvancedExtensions().UseYamlFrontMatter().Build()

    let private deserializer =
        DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()

    let parse mardown = Markdown.Parse(mardown, pipeline)

    let properties (doc: MarkdownDocument) =
        doc.Descendants<YamlFrontMatterBlock>()
        |> Seq.tryHead
        |> Option.map (
            _.Lines.ToString()
            >> deserializer.Deserialize<Map<string, objnull>>
            >> Map.filter (fun _ value -> value |> isNull |> not)
            >> Map.map (fun _ value -> value |> string)
        )

    let toHtml doc = Markdown.ToHtml(doc, pipeline)
