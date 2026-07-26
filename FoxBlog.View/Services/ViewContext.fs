namespace FoxBlog.View


type ViewContext() =
    member val ui: string option = None with get, set
    member val title: string option = None with get, set
