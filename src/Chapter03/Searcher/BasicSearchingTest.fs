namespace LuceneInAction2E.Chatper03.Searcher

open System.IO
open LuceneInAction2E.Common
open Lucene.Net.Store
open Lucene.Net.Search
open Lucene.Net.Index
open Lucene.Net.QueryParsers.Classic
open Lucene.Net.Analysis.Core
open Lucene.Net.Analysis.Standard
open Lucene.Net.Documents

module BasicSearchingTest = 

    let private assertEquals message expected actual =
        if expected = actual then
            printfn $"{message}: {nameof expected} '{expected}' matches {nameof actual} '{actual}'"
        else
            printfn $"ERROR"
            printfn $"{message}: expected value does not equal the actual value"
            printfn $"\texpected: {expected}"
            printfn $"\tactual: {actual}"
        
    let private assertTrue message (predicate : bool) =
        if predicate then
            printfn $"{message}: predicate is true as expected"
        else
            printfn $"ERROR"
            printfn $"{message}: predicate is false"
            
    
    let testTerm () =

        printfn $"Index directory: {IndexProperties.bookIndexDirFromBinFolder}"
        
        use indexDir = FSDirectory.Open(IndexProperties.bookIndexDirFromBinFolder)
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher indexReader
        
        let term = Term("subject", "ant")
        let query = TermQuery(term)
        let docs = searcher.Search(query, 10)
        assertEquals "Ant in Action" 1 docs.TotalHits

        let term2 = Term("subject", "junit")
        let docs = searcher.Search(TermQuery(term2), 10)
        assertEquals "Ant in Action, JUnit in Action, Second Edition" 2 docs.TotalHits


    let testQueryParser () =

        use indexDir = FSDirectory.Open(IndexProperties.bookIndexDirFromBinFolder)
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher indexReader

        let parser = QueryParser(IndexProperties.luceneVersion, "contents", new SimpleAnalyzer(IndexProperties.luceneVersion))

        let query = parser.Parse("+JUNIT +ANT -MOCK")
        let docs = searcher.Search(query, 10)
        assertEquals "QueryParser" 1 docs.TotalHits

        let query2 = parser.Parse("mock OR junit")
        let docs2 = searcher.Search(query2, 10)
        assertEquals "Ant in Action, JUnit in Action, Second Edition" 2 docs2.TotalHits

    let testNearRealTime() =
        
        use indexDir = FSDirectory.Open(IndexProperties.bookIndexDirFromBinFolder)
        
        let writerConfig = IndexWriterConfig(IndexProperties.luceneVersion, new StandardAnalyzer(IndexProperties.luceneVersion))
        writerConfig.OpenMode <- OpenMode.CREATE
        use writer = new IndexWriter(indexDir, writerConfig)

        [| 0 .. 9 |]
        |> Array.iter (fun ix ->
            let doc = Document()
            doc.Add(StringField("id", ix.ToString(), Field.Store.NO))
            doc.Add(TextField("text", "aaa", Field.Store.NO))
            writer.AddDocument(doc)
            )

        // Create a near-real-time reader.
        use reader = writer.GetReader(applyAllDeletes = true)
        let searcher = IndexSearcher(reader)

        // Run a quick search.
        let query = TermQuery(Term("text", "aaa"))
        let docs = searcher.Search(query, 1)
        assertEquals "Term query for text 'aaa'" 10 docs.TotalHits

        // Delete a document.
        writer.DeleteDocuments(Term("id", "7"))

        // Add another document.
        let doc = Document()
        doc.Add(StringField("id", "11", Field.Store.NO))
        doc.Add(TextField("text", "bbb", Field.Store.NO))
        writer.AddDocument(doc)

        // Open a new near-real-time reader.
        use newReader = DirectoryReader.OpenIfChanged(reader)
        assertTrue "newReader is not null" (newReader |> isNull |> not)

        // Close the old reader.
        reader.Dispose()

        // Ensure the old query matches only 9 documents this time.
        let searcher2 = IndexSearcher(newReader)
        let docs2 = searcher2.Search(query, 10)
        assertEquals "Search modified index" 9 docs2.TotalHits

        // Create a new query for bbb and ensure there's only one match.
        let query2 = TermQuery(Term("text", "bbb"))
        let docs3 = searcher2.Search(query2, 1)
        assertEquals "Query text for bbb" 1 docs3.TotalHits

    let testExplanation () =
        
        use indexDir = FSDirectory.Open IndexProperties.bookIndexDirFromBinFolder

        let queryParser = QueryParser(IndexProperties.luceneVersion, "contents", new SimpleAnalyzer(IndexProperties.luceneVersion))
        let query = queryParser.Parse("junit")
        printfn $"Query: {query.ToString()}"

        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher(indexReader)
        let topDocs = searcher.Search(query, 10)

        topDocs.ScoreDocs
        |> Array.iter (fun scoreDoc ->
            let explanation = searcher.Explain(query, scoreDoc.Doc)

            printfn "----------"
            let doc = searcher.Doc scoreDoc.Doc
            let title = doc.Get("title")
            printfn $"{title}"
            printfn $"{explanation.ToString()}"
            ())
        
    let testKeyword () =
        
        use indexDir = FSDirectory.Open IndexProperties.bookIndexDirFromBinFolder        
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher(indexReader)
        
        let term = Term("isbn", "9781935182023")
        let query = TermQuery(term)
        
        let docs = searcher.Search(query, 10)
        assertEquals "JUnit in Action, Second Edition" 1 docs.TotalHits
        
    let testTermRangeQuery () =
        
        use indexDir = FSDirectory.Open IndexProperties.bookIndexDirFromBinFolder        
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher(indexReader)
        
        let query = TermRangeQuery.NewStringRange(field = "title2", lowerTerm = "d", upperTerm = "j", includeLower = true, includeUpper = true)
        
        let docs = searcher.Search(query, 100)
        assertEquals "TermRangeQuery" 3 docs.TotalHits
        
    let testInclusiveNumericRangeQuery () =
        
        use indexDir = FSDirectory.Open IndexProperties.bookIndexDirFromBinFolder        
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher(indexReader)
        
        let query = NumericRangeQuery.NewInt32Range(field = "pubmonth", min = 200605, max = 200609, minInclusive = true, maxInclusive = true)
        
        let docs = searcher.Search(query, 10)
        assertEquals "NumericRangeQuery" 1 docs.TotalHits

    let testExclusiveNumericRangeQuery () =
        
        use indexDir = FSDirectory.Open IndexProperties.bookIndexDirFromBinFolder        
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher(indexReader)
        
        let query = NumericRangeQuery.NewInt32Range(field = "pubmonth", min = 200605, max = 200609, minInclusive = false, maxInclusive = false)
        
        let docs = searcher.Search(query, 10)
        assertEquals "NumericRangeQuery" 0 docs.TotalHits
