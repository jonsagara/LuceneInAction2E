namespace LuceneInAction2E.Chapter04.Analysis

open LuceneInAction2E.Chapter04.Analysis.SoundsLike
open Lucene.Net.Documents
open Lucene.Net.Index
open Lucene.Net.QueryParsers.Classic
open Lucene.Net.Search
open Lucene.Net.Store
open LuceneInAction2E.Common
open LuceneInAction2E.Common.TestHelper
open LuceneInAction2E.Chapter04.Analysis.Synonym
open System.IO
open Lucene.Net.Analysis.TokenAttributes

module SynonymTest =

    let testJumps () =
        
        //use directory = new RAMDirectory()
        //use analyzer = new SynonymAnalyzer(TestSynonymEngine())
                
        //let writerConfig = IndexWriterConfig(IndexProperties.luceneVersion, analyzer)
        //writerConfig.OpenMode <- OpenMode.CREATE
        //use writer = new IndexWriter(directory, writerConfig)

        //let docToAdd = Document()
        //docToAdd.Add(TextField("contents", "The quick brown fox jumps over the lazy dog", Field.Store.YES))
        //writer.AddDocument(docToAdd)
        //writer.Commit()

        //use reader = DirectoryReader.Open(directory)
        //let searcher = IndexSearcher(reader)

        use analyzer = new SynonymAnalyzer(TestSynonymEngine())
        use stream = analyzer.GetTokenStream("contents", new StringReader("jumps"))

        let termAttr = stream.GetAttribute<ICharTermAttribute>()
        let posIncrAttr = stream.GetAttribute<IPositionIncrementAttribute>()

        stream.Reset()

        let expected = [|
            "jumps"
            "hops"
            "leaps"
            |]

        let mutable ix = 0

        while stream.IncrementToken() do
            assertEquals "Expected term" expected[ix] (termAttr.ToString())

            let mutable expectedPos = 0
            if ix = 0 then
                // This is the initial term. The Position Increment should be 1.
                expectedPos <- 1
            else
                // This is a synonym. The Position Increment should be 0.
                expectedPos <- 0

            assertEquals "Expected Position Increment" expectedPos (posIncrAttr.PositionIncrement)

            ix <- ix + 1

        assertEquals "Term count" 3 ix
        

        //// Search with query "kool kat", using the same Metaphone Analyzer as the indexer.
        //let parser = QueryParser(IndexProperties.luceneVersion, "contents", analyzer)
        //let query = parser.Parse("kool kat")

        //let hits = searcher.Search(query, 1)
        //assertEquals "Hit count" 1 hits.TotalHits

        //let docId = hits.ScoreDocs[0].Doc
        //let docReadFromSearcher = searcher.Doc(docId)
        //assertEquals "Sounds Like" "cool cat" (docReadFromSearcher.Get("contents"))

        ()