namespace FoxBlog

module Types =
    type UI =
        { supported: UIdto array
          defaultUI: UIdto option }

    and UIdto =
        { name: string
          display: string }

        static member inline map(ui: 'T when 'T: (member name: string) and 'T: (member display: string)) =
            { name = ui.name; display = ui.display }
