namespace LuceneInAction2E.Chatper03.Searcher

open Lucene.Net.Search
open Lucene.Net.Index
open LuceneInAction2E.Common.TestHelper
open Lucene.Net.QueryParsers.Classic
open LuceneInAction2E.Common
open Lucene.Net.Analysis.Core
open Lucene.Net.Store
open Lucene.Net.Analysis.Standard

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

    let testPhraseQuery () =
        use analyzer = new StandardAnalyzer(IndexProperties.luceneVersion)
        let parser = QueryParser(IndexProperties.luceneVersion, "field", analyzer)

        let query = parser.Parse("\"This is Some Phrase*\"")
        assertEquals "analyzed" "\"? ? some phrase\"" (query.ToString("field"))

        let query2 = parser.Parse("\"term\"")
        assertTrue "reduced to TermQuery" (query2.GetType() = typeof<TermQuery>)

    let testSlop () =
        use analyzer = new StandardAnalyzer(IndexProperties.luceneVersion)
        let parser = QueryParser(IndexProperties.luceneVersion, "field", analyzer)

        let query = parser.Parse("\"exact phrase\"")
        assertEquals "zero slop" "\"exact phrase\"" (query.ToString("field"))

        let parser2 = QueryParser(IndexProperties.luceneVersion, "field", analyzer)
        parser2.PhraseSlop <- 5
        let query2 = parser2.Parse("\"sloppy phrase\"")
        assertEquals "sloppy, implicitly" "\"sloppy phrase\"~5" (query2.ToString("field"))
        
    let testFuzzyQuery () =
        use analyzer = new StandardAnalyzer(IndexProperties.luceneVersion)
        let parser = QueryParser(IndexProperties.luceneVersion, "field", analyzer)

        // In the book, this returns a similarity of 0.5. For us, returns 2. The API appears to have changed in 4.x.
        let query = parser.Parse("kountry~")
        printfn $"fuzzy: {query} {query.GetType().FullName}"

        // In the book, this returns a similarity of 0.7. For us, returns 2. The API appears to have changed in 4.x.
        let query2 = parser.Parse("kountry~0.7")
        printfn $"fuzzy 2: {query2} {query2.GetType().FullName}"

    let testGrouping () =
        use indexDir = FSDirectory.Open(IndexProperties.bookIndexDirFromBinFolder)
        use indexReader = DirectoryReader.Open indexDir
        let searcher = IndexSearcher indexReader

        use analyzer = new StandardAnalyzer(IndexProperties.luceneVersion)
        let parser = QueryParser(IndexProperties.luceneVersion, "subject", analyzer)
        let query = parser.Parse("(agile OR extreme) AND methodology")
        let matches = searcher.Search(query, 10)

        assertTrue"Has Extremem Programming Explained" (TestUtil.hitsIncludeTitle searcher matches "Extreme Programming Explained")
        assertTrue"Has The Pragmatic Programmer" (TestUtil.hitsIncludeTitle searcher matches "The Pragmatic Programmer")

