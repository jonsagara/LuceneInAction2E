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
        printfn $"Index directory: {Path.GetFullPath indexDir}"
        let dir = FSDirectory.Open indexDir
        let analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48)

        let config = IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)
        config.OpenMode <- OpenMode.CREATE

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

    member this.index (dataDir : string) =
        IO.Directory.GetFiles(dataDir, "*.txt", SearchOption.TopDirectoryOnly)
        |> Array.iter (fun fileName -> 
            printfn $"Indexing {Path.GetFullPath fileName}..."
            let doc = this.getDocument fileName
            _writer.AddDocument doc)

        _writer.Commit()
        _writer.NumDocs