namespace LuceneInAction2E.Chatper03.Searcher

open Lucene.Net.Search
open Lucene.Net.Index
open LuceneInAction2E.Common.TestHelper

module QueryParserTest =
    
    let testToString () =
        let query = BooleanQuery()
        query.Add(FuzzyQuery(Term("field", "kountry")), Occur.MUST)
        query.Add(TermQuery(Term("title", "western")), Occur.SHOULD)

        assertEquals "both kinds" "+kountry~2 title:western" (query.ToString("field"))

