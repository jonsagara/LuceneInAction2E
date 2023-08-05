namespace LuceneInAction2E.Chapter04.Analysis

open System.IO
open Lucene.Net.Analysis
open Lucene.Net.Analysis.TokenAttributes

module AnalyzerUtils =

    /// Iterate through the TokenStream and display each term.
    let displayTokensFromTokenStream (stream : TokenStream) =
        // See: https://stackoverflow.com/a/54337796
        let term = stream.GetAttribute<ICharTermAttribute>()

        stream.Reset()

        while stream.IncrementToken() do
            let termText = term.ToString()
            printf $"[{termText}] "

    /// Analyze the text using the given Analyzer. Get the analyzer's token stream, and then 
    /// print each token to the screen.
    let displayTokens (analyzer : Analyzer) (text : string) =
        use tokenStream = analyzer.GetTokenStream("contents", new StringReader(text))
        displayTokensFromTokenStream tokenStream
    
