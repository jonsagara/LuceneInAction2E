namespace LuceneInAction2E.Chapter01.Indexer

open System
open System.IO
open Lucene.Net.Documents
open Lucene.Net.Index
open Lucene.Net.Store
open Lucene.Net.Analysis.Standard
open Lucene.Net.Util

type Indexer(indexDir : string) =

    let mutable _writer : IndexWriter = null

    do
        let dir = FSDirectory.Open indexDir
        let analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48)
        let config = IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)
        _writer <- new IndexWriter(dir, config)

    interface IDisposable with
        member x.Dispose() = 
            _writer.Dispose()

    
    member private this.getDocument (fileName : string) =
        let doc = Document()

        doc.Add(TextField("contents",File.OpenText fileName))
        doc.Add(StringField("filename", Path.GetFileName fileName, Field.Store.YES))
        doc.Add(StringField("fullpath", Path.GetFullPath fileName, Field.Store.YES))

        doc

    member private this.indexFile (fileName : string) =
        printfn $"Indexing {Path.GetFullPath fileName}..."
        let doc = this.getDocument fileName
        _writer.AddDocument doc

    member this.index (dataDir : string) =
        //let di = new DirectoryInfo(dataDir)
        //let txtFiles = di.GetFiles("*.txt")
        let txtFiles = System.IO.Directory.GetFiles(dataDir, "*.txt", SearchOption.TopDirectoryOnly)
        //txtFiles
        //|> Array.iter (fun f -> printfn $".txt file: {f} ({Path.GetFullPath f})")
        
        txtFiles
        |> Array.iter (fun f -> this.indexFile f)

        _writer.NumDocs