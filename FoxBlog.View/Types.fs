namespace FoxBlog.View

module Types =
    type Link = { name: string; url: string }

    type UI =
        {
          // default if None
          name: string option
          // not shown in the dropdown selection if None
          display: string option
          top: Section option
          side: Section option
          nodes: Map<string, string>
          language: bool }

        static member defaultValue =
            { name = None
              display = None
              top = None
              side = None
              nodes = Map.empty
              language = false }

    and Section = { links: Link list }
