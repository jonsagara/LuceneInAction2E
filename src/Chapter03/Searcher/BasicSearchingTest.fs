namespace LuceneInAction2E.Chatper03.Searcher

open System.IO
open LuceneInAction2E.Common
open Lucene.Net.Store
open Lucene.Net.Search
open Lucene.Net.Index

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

        let indexDirPath = Path.GetFullPath(Path.Combine("../../../../..", IndexProperties.bookIndexName))
        printfn $"Index directory: {indexDirPath}"
        
        use indexDir = FSDirectory.Open(indexDirPath)
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher indexReader
        
        let term = Term("subject", "ant")
        let query = TermQuery(term)
        let docs = searcher.Search(query, 10)
        assertEquals "Ant in Action" 1 docs.TotalHits

        let term2 = Term("subject", "junit")
        let docs = searcher.Search(TermQuery(term2), 10)
        assertEquals "Ant in Action, JUnit in Action, Second Edition" 2 docs.TotalHits
        
        ()

