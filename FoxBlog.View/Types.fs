namespace FoxBlog.View

module Types =
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



    and Link = { name: string; url: string }
    and Section = { links: Link list }

    type Post =
        { name: string option
          display: string option
          date: System.DateTime option
          tags: string list }
