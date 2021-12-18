open System.Xml
open System.IO

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
    let xmlString = xml.ToString()
    printfn "%s" xmlString
    0 // return an integer exit code
