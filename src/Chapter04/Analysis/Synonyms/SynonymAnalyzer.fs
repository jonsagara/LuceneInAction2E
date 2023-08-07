namespace LuceneInAction2E.Chapter04.Analysis.Synonym

open System.IO
open Lucene.Net.Analysis
open Lucene.Net.Analysis.Core
open LuceneInAction2E.Common
open Lucene.Net.Analysis.Standard

type SynonymAnalyzer(engine : ISynonymEngine) =
    inherit Analyzer()

    let _engine = engine

    override this.CreateComponents (fieldName : string, reader : TextReader) =

        let tokenizer = new StandardTokenizer(IndexProperties.luceneVersion, reader)

        let standardFilter = new StandardFilter(IndexProperties.luceneVersion, tokenizer)
        let lowerCaseFilter = new LowerCaseFilter(IndexProperties.luceneVersion, standardFilter)
        let stopFilter = new StopFilter(IndexProperties.luceneVersion, lowerCaseFilter, StopAnalyzer.ENGLISH_STOP_WORDS_SET)
        let synonymFilter = new SynonymFilter(stopFilter,_engine)

        new TokenStreamComponents(tokenizer, synonymFilter)
