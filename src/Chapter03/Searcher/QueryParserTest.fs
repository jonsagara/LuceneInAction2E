namespace LuceneInAction2E.Chatper03.Searcher

open Lucene.Net.Search
open Lucene.Net.Index
open LuceneInAction2E.Common.TestHelper
open Lucene.Net.QueryParsers.Classic
open LuceneInAction2E.Common
open Lucene.Net.Analysis.Core
open Lucene.Net.Store

module QueryParserTest =
    
    let testToString () =
        let query = BooleanQuery()
        query.Add(FuzzyQuery(Term("field", "kountry")), Occur.MUST)
        query.Add(TermQuery(Term("title", "western")), Occur.SHOULD)
        
        // The book says "+kountry~0.5 title:western", but Lucene.NET 4.8.0-beta00016 
        //   spits out "+kountry~2 title:western".
        assertEquals "both kinds" "+kountry~2 title:western" (query.ToString("field"))

    let testTermQuery () =
        use analyzer = new WhitespaceAnalyzer(IndexProperties.luceneVersion)
        let parser = QueryParser(IndexProperties.luceneVersion, "subject", analyzer)
        let query = parser.Parse("computers")

        printfn $"term: {query}"

    let testTermRangeQuery () =
        use analyzer = new WhitespaceAnalyzer(IndexProperties.luceneVersion)
        let parser = QueryParser(IndexProperties.luceneVersion, "subject", analyzer)
        let query = parser.Parse("title2:[Q TO V]")

        assertTrue "query is a TermRangeQuery" (query.GetType() = typeof<TermRangeQuery>)

        use indexDir = FSDirectory.Open(IndexProperties.bookIndexDirFromBinFolder)
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher indexReader

        let matches = searcher.Search(query, 10)
        assertTrue "Title is in results" (TestUtil.hitsIncludeTitle searcher matches "Tapestry in Action")

        let query2 = parser.Parse("title2:{Q TO \"Tapestry in Action\" }")
        let matches2 = searcher.Search(query2, 10)
        assertFalse "Title is not in results" (TestUtil.hitsIncludeTitle searcher matches2 "Tapestry in Action")
        
    let testLowercasing () =
        use analyzer = new WhitespaceAnalyzer(IndexProperties.luceneVersion)
        
        let parser = QueryParser(IndexProperties.luceneVersion, "field", analyzer)
        let query = parser.Parse("PrefixQuery*")
        assertEquals "lowercased" "prefixquery*" (query.ToString("field"))
        
        let parser2 = QueryParser(IndexProperties.luceneVersion, "field", analyzer)
        parser2.LowercaseExpandedTerms <- false
        let query2 = parser2.Parse("PrefixQuery*")
        assertEquals "not lowercased" "PrefixQuery*" (query2.ToString("field"))
        