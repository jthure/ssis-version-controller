open System.Xml
open System.IO
open System.Linq
open System.Linq.Expressions

// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp
// See the 'F# Tutorial' project for more help.

// Define a function to construct a message to print
let parseXml xmlPath =
    let doc = new XmlDocument() in
        File.ReadAllText(xmlPath)
        |> doc.LoadXml
    doc
    

[<EntryPoint>]
let main argv =
    let xml = parseXml "C:\\Users\\joth\\dev\\ssis-version-controller\\test\\FetchProduct.dtsx"
    let designProps = xml.GetElementsByTagName("DTS:DesignTimeProperties").Cast<XmlNode>()
    
    match designProps.FirstOrDefault() with
        | null -> null
        | node -> node.ParentNode.RemoveChild(node)
    |> ignore

    xml.Save("C:\\Users\\joth\\dev\\ssis-version-controller\\test\\FetchProduct_output.dtsx")
    0 // return an integer exit code
