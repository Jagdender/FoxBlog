namespace Giraffe.ViewEngine

module Extensions =

    let dropdown (attributes: XmlAttribute list) (content: XmlNode list) (options: XmlNode list) =
        let id = System.Guid.NewGuid().ToString()

        tag
            "ot-dropdown"
            []
            [ button ([ attr "popovertarget" id ] @ attributes) content
              menu [ _id id; flag "popover" ] options ]

    let role value = attr "role" value
