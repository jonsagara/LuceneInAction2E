namespace LuceneInAction2E.Chapter04.Analysis

open Lucene.Net.Analysis
open Lucene.Net.Analysis.Core
open Lucene.Net.Analysis.Standard
open LuceneInAction2E.Common

module AnalyzerDemo =

    let private _examples = [|
        "The quick brown fox jumped over the lazy dog"
        "XY&Z Corporation - xyz@example.com"
        |]

    let private _analyzers : Analyzer[] = [|
        new WhitespaceAnalyzer(IndexProperties.luceneVersion)
        new SimpleAnalyzer(IndexProperties.luceneVersion)
        new StopAnalyzer(IndexProperties.luceneVersion)
        new StandardAnalyzer(IndexProperties.luceneVersion)
        |]

    let private analyze (text : string) =
        printfn $"Analyzing \"{text}\""

        for analyzer in _analyzers do
            let name = analyzer.GetType().FullName
            printfn $"  {name}:"
            printf $"    "
            AnalyzerUtils.displayTokens analyzer text
            printfn ""

        printfn ""

    let doDemo (args : string[]) =
        let mutable strings = _examples

        if args.Length > 0 then do
            strings <- args

        for text in strings do
            analyze text

    let doDemoWithFullDetails () =
        AnalyzerUtils.displayTokensWithFullDetails (new SimpleAnalyzer(IndexProperties.luceneVersion)) "The quick brown fox...."

    let testStopAnalyzer2 () =
        use sa2 = new StopAnalyzer2()

        AnalyzerUtils.assertAnalyzesTo sa2 "The quick brown..." [| "quick"; "brown" |]

    let testStopAnalyzerFlawed () =
        use saFlawed = new StopAnalyzerFlawed()

        AnalyzerUtils.assertAnalyzesTo saFlawed "The quick brown..." [| "the"; "quick"; "brown" |]


