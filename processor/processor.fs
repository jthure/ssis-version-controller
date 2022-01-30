namespace Techendary.Processor

module Processor =

    open System.Xml
    open System.IO
    open System.Linq
    open System.Linq.Expressions
    open utils.Xml


    //type OrderdXmlNodeList() = inherit XmlNodeList()

    let parseXml (xmlPath : string) =
        let doc = new XmlDocument() in
            File.ReadAllText(xmlPath)
            |> doc.LoadXml
        doc

    let xmlWriterSettings : XmlWriterSettings =
        let settings = new XmlWriterSettings() in
           settings.Indent <- true;
           settings.OmitXmlDeclaration <- false;
           settings.NewLineOnAttributes <- true;
        settings

    let xmlWriter (path : string) = XmlWriter.Create(path, xmlWriterSettings)

    let processXml path :XmlDocument = 
        let xml = parseXml path
        let designProps = xml.GetElementsByTagName("DTS:DesignTimeProperties").Cast<XmlNode>()
    
        match designProps.FirstOrDefault() with
            | null -> null
            | node -> node.ParentNode.RemoveChild(node)
        |> ignore

        XmlUtils.SortXml(xml, false, ["paths"])
        xml

    [<EntryPoint>]
    let main argv =
        let processedXml = processXml "C:\\Users\\joth\\dev\\ssis-version-controller\\processor\\testfiles\\FetchProduct.dtsx" in
            xmlWriter "C:\\Users\\joth\\dev\\ssis-version-controller\\processor\\testfiles\\FetchProduct_output.dtsx"
            |> processedXml.Save

        0 // return an integer exit code


