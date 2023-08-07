namespace LuceneInAction2E.Common

open Lucene.Net.Search

module TestUtil =

    let hitsIncludeTitle (searcher : IndexSearcher) (hits : TopDocs) (title : string) =
        let anyHitsIncludeTitle =
            hits.ScoreDocs
            |> Array.exists (fun scoreDoc ->
                let doc = searcher.Doc scoreDoc.Doc
                title.Equals(doc.Get("title"))
                )

        if not(anyHitsIncludeTitle) then do
            printfn $"title '{title}' not found"

        anyHitsIncludeTitle

    /// Executes the query and returns the Total Hits.
    let hitCount (searcher : IndexSearcher) (query : Query) =
        searcher.Search(query, 1).TotalHits

