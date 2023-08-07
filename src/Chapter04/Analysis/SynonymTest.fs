namespace LuceneInAction2E.Chapter04.Analysis

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
open Lucene.Net.Analysis.Standard
open System

module SynonymTest =

    type private TestSetupResult() =
        
        member val Searcher : IndexSearcher = null with get, set
        
        interface IDisposable with
            member this.Dispose() = 
                ()
        

    let private setupTest () =
        let directory = new RAMDirectory()
        use synonymAnalyzer = new SynonymAnalyzer(TestSynonymEngine())
                
        let writerConfig = IndexWriterConfig(IndexProperties.luceneVersion, synonymAnalyzer)
        writerConfig.OpenMode <- OpenMode.CREATE
        use writer = new IndexWriter(directory, writerConfig)

        let docToAdd = Document()
        docToAdd.Add(TextField("content", "The quick brown fox jumps over the lazy dog", Field.Store.YES))
        writer.AddDocument(docToAdd)
        writer.Dispose()

        let reader = DirectoryReader.Open(directory)
        let s = IndexSearcher(reader)

        new TestSetupResult(Searcher = IndexSearcher(reader))


    //
    // Public functions
    //

    let testJumps () =

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
                // This is a synonym. The Position Increment should be 0. In other words, synonyms are
                //   placed in the same position as the initial word.
                expectedPos <- 0

            assertEquals "Expected Position Increment" expectedPos (posIncrAttr.PositionIncrement)

            ix <- ix + 1

        assertEquals "Term count" 3 ix


    let testSearchByAPI () =
        
        use setupResult = setupTest()

        // TermQuery
        let termQuery = TermQuery(Term("content", "hops"))
        assertEquals "TermQuery hit count" 1 (TestUtil.hitCount setupResult.Searcher termQuery)

        // PhraseQuery
        let phraseQuery = PhraseQuery()
        phraseQuery.Add(Term("content", "fox"))
        phraseQuery.Add(Term("content", "hops"))
        assertEquals "PhraseQuery hit count" 1 (TestUtil.hitCount setupResult.Searcher phraseQuery)


    let testWithQueryParser () =
        
        use setupResult = setupTest ()

        // SynonymAnalyzer
        use synonymAnalyzer = new SynonymAnalyzer(TestSynonymEngine())
        let synonymParser = QueryParser(IndexProperties.luceneVersion, "content", synonymAnalyzer)
        let synonymQuery = synonymParser.Parse("\"fox jumps\"")

        assertEquals "Synonym query hits" 1 (TestUtil.hitCount setupResult.Searcher synonymQuery)

        let synonymQueryAsString = synonymQuery.ToString("content")
        printfn $"With {nameof SynonymAnalyzer}, \"fox jumps\" parses to {synonymQueryAsString}"

        // StandardAnalyzer
        use standardAnalyzer = new StandardAnalyzer(IndexProperties.luceneVersion)
        let standardParser = QueryParser(IndexProperties.luceneVersion, "content", standardAnalyzer)
        let standardQuery = standardParser.Parse("\"fox jumps\"")

        assertEquals "Standard query hits" 1 (TestUtil.hitCount setupResult.Searcher standardQuery)

        let standardQueryAsString = standardQuery.ToString("content")
        printfn $"With {nameof StandardAnalyzer}, \"fox jumps\" parses to {standardQueryAsString}"
