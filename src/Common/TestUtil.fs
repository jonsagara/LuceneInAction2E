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

