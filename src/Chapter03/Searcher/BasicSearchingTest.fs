namespace LuceneInAction2E.Chatper03.Searcher

open System.IO
open LuceneInAction2E.Common
open Lucene.Net.Store
open Lucene.Net.Search
open Lucene.Net.Index
open Lucene.Net.QueryParsers.Classic
open Lucene.Net.Analysis.Core

module BasicSearchingTest = 

    let private assertEquals message expected actual =
        if expected = actual then
            printfn $"{message}: {nameof expected} '{expected}' matches {nameof actual} '{actual}'"
        else
            printfn $"ERROR"
            printfn $"{message}: expected value does not equal the actual value"
            printfn $"\texpected: {expected}"
            printfn $"\tactual: {actual}"
        
            
    
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
