namespace LuceneInAction2E.Chapter00.CreateTestIndex

open System
open System.IO
open Lucene.Net.Documents
open Lucene.Net.Index
open Lucene.Net.Store
open LuceneInAction2E.Common
open Authlete.Util


type Indexer(dataDir : string, indexDir : string) =
    let mutable _writer : IndexWriter = null

    /// <summary>
    /// Indexed, not tokenized, omits norms, indexes
    /// <see cref="IndexOptions.DOCS_ONLY"/>, stored
    /// </summary>
    let StringFieldTypeStored = FieldType(IsIndexed = true, OmitNorms = true, IndexOptions = IndexOptions.DOCS_ONLY, IsStored = true, IsTokenized = false)

    /// <summary>
    /// Indexed, tokenized, not stored. </summary>
    let TextFieldTypeNotStored = new FieldType(IsIndexed = true, IsTokenized = true)

    /// <summary>
    /// Indexed, tokenized, stored. </summary>
    let TextFieldTypeStored = new FieldType(IsIndexed = true, IsTokenized = true, IsStored = true)

    let setFieldWithPositionsAndOffset (field : Field) =
        field.FieldType.StoreTermVectors <- true
        field.FieldType.StoreTermVectorPositions <- true
        field.FieldType.StoreTermVectorOffsets <- true

    let setFieldIndexedNotAnalyzedNoNorms (field : Field) =
        field.FieldType.IsIndexed <- true
        field.FieldType.IsTokenized <- false
        field.FieldType.OmitNorms <- true

    do
        printfn $"Data directory: {dataDir}"
        printfn $"Index directory: {indexDir}"

        let dir = FSDirectory.Open indexDir
        let analyzer = new MyStandardAnalyzer(IndexProperties.luceneVersion)
        let config = IndexWriterConfig(IndexProperties.luceneVersion, analyzer)
        config.OpenMode <- OpenMode.CREATE

        //// DEBUG. Very verbose.
        ////config.SetInfoStream(Console.Out) |> ignore

        _writer <- new IndexWriter(dir, config)

    interface IDisposable with
        member x.Dispose() = 
            _writer.Dispose()

    
    member private this.getDocument (dataDir : string) (filePath : string) =
        use propertiesStreamReader = new StreamReader(filePath)
        let properties = PropertiesLoader.Load(propertiesStreamReader)

        // Category comes from relative path below the base directory.
        let parentDirInfo = Directory.GetParent filePath
        let mutable category = parentDirInfo.FullName.Substring(dataDir.Length)
        category <- category.Replace("\\", "/")

        let doc = Document()

        let isbn = properties["isbn"]
        let title = properties["title"]
        let author = properties["author"]
        let url = properties["url"]
        let subject = properties["subject"]
        let pubMonth = properties["pubmonth"]

        printfn $"{title}\n{author}\n{subject}\n{pubMonth}\n{category}\n---------"

        doc.Add(StringField("isbn", isbn, Field.Store.YES))
        doc.Add(StringField("category", category, Field.Store.YES))

        // Equivalent of a TextField, but we need to be able to modify the FieldType.
        let titleField = Field("title", title,TextFieldTypeStored)
        setFieldWithPositionsAndOffset titleField
        doc.Add(titleField)

        // Equivalent of a TextField, but we need to be able to modify the FieldType.
        let title2Field = Field("title2", title.ToLower(), TextFieldTypeStored)
        setFieldIndexedNotAnalyzedNoNorms title2Field
        setFieldWithPositionsAndOffset title2Field
        doc.Add(title2Field)

        // split multiple authors into unique field instances
        author.Split(",")
        |> Array.iter (fun auth ->
            // Equivalent of a StringField, but we need to be able to modify the FieldType.
            let authorField = Field("author", auth, StringFieldTypeStored)
            setFieldWithPositionsAndOffset authorField
            doc.Add(authorField)
            )

        // Equivalent of a StringField, but we need to be able to modify the FieldType.
        let urlField = Field("url", url, StringFieldTypeStored)
        setFieldIndexedNotAnalyzedNoNorms urlField
        doc.Add(urlField)

        // Equivalent of a TextField, but we need to be able to modify the FieldType.
        let subjectField = Field("subject", subject, TextFieldTypeStored)
        setFieldWithPositionsAndOffset subjectField
        doc.Add(subjectField)

        doc.Add(Int32Field("pubmonth", Int32.Parse(pubMonth), Field.Store.YES))

        let pubMonthDateTime = DateTools.StringToDate(pubMonth)
        let pubMonthDateTimeOffset = DateTimeOffset(pubMonthDateTime)
        let pubMonthAsDay = int32(pubMonthDateTimeOffset.ToUnixTimeMilliseconds() / int64(1000*3600*24))
        doc.Add(Int32Field("pubmonthAsDay", pubMonthAsDay, Field.Store.YES))
        
        [| title; subject; author; category |]
        |> Array.iter(fun contentField ->
            // Equivalent of a TextField, but we need to be able to modify the FieldType.
            let contentsField = Field("contents", contentField, TextFieldTypeNotStored)
            setFieldWithPositionsAndOffset contentsField
            doc.Add(contentsField)
            )

        doc

    member this.index () =
        let propsFiles = Directory.GetFiles(dataDir, "*.properties", SearchOption.AllDirectories)
        propsFiles
        |> Array.iter (fun file ->
            let doc = this.getDocument dataDir file
            _writer.AddDocument(doc)
            )

        _writer.Commit()
        _writer.NumDocs

