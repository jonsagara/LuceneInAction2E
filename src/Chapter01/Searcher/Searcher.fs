namespace LuceneInAction2E.Chapter01.Searcher

open System.Diagnostics
open Lucene.Net.Analysis.Standard
open Lucene.Net.QueryParsers.Classic
open Lucene.Net.Index
open Lucene.Net.Search
open Lucene.Net.Store
open LuceneInAction2E.Common

module Searcher =
    
    let search (indexDir : string) (queryText : string) =
        
        use indexDir = FSDirectory.Open indexDir
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher indexReader
        
        use analyzer = new StandardAnalyzer(IndexProperties.luceneVersion)

        let queryParser = QueryParser(IndexProperties.luceneVersion, "contents", analyzer)
        let query = queryParser.Parse(queryText)

        let startTime = Stopwatch.GetTimestamp()
        let hits = searcher.Search(query, 10)
        let elapsed = Stopwatch.GetElapsedTime(startTime)

        printfn $"Found {hits.TotalHits} document(s) (in {elapsed}) that matched query '{queryText}': "
        hits.ScoreDocs
        |> Array.iter (fun hit ->
            let doc = searcher.Doc hit.Doc
            let fullPath = doc.Get("fullpath")
            printfn $"{fullPath}"
            )
