namespace FoxBlog.View

module Types =
    type Link = { name: string; url: string }

    type UI =
        { display: string option
          top: Section option
          side: Section option
          nodes: Map<string, string>
          hidden: bool
          language: bool }

    and Section = { links: Link list }
