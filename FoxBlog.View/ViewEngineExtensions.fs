namespace Giraffe.ViewEngine

module Extensions =
    type DropdownBtn =
        { id: string
          attributes: XmlAttribute list
          contents: XmlNode list }

    let dropdown (btn: DropdownBtn) (options: XmlNode list) =
        tag
            "ot-dropdown"
            []
            [ button ([ attr "popovertarget" btn.id ] @ btn.attributes) btn.contents
              menu [ _id btn.id; flag "popover" ] options ]
