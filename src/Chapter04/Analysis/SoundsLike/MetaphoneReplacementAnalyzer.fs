namespace LuceneInAction2E.Chapter04.Analysis.SoundsLike

open System.IO
open Lucene.Net.Analysis
open Lucene.Net.Analysis.Core
open LuceneInAction2E.Common
open Lucene.Net.Analysis.TokenAttributes

type MetaphoneReplacementAnalyzer() =
    inherit Analyzer()

    override this.CreateComponents (fieldName : string, reader : TextReader) =

        let letterTokenizer = new LetterTokenizer(IndexProperties.luceneVersion, reader)

        // We need the Type Attribute so that we can change it to Metaphone after we replace the term with the 
        //   Metaphone-encoded term.
        letterTokenizer.AddAttribute<ITypeAttribute>() |> ignore

        let metaphoneReplacementFilter = new MetaphoneReplacementFilter(letterTokenizer)

        new TokenStreamComponents(letterTokenizer, metaphoneReplacementFilter)
