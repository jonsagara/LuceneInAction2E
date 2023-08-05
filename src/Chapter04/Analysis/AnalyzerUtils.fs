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
            printf $"[{term}] "

    /// Analyze the text using the given Analyzer. Get the analyzer's token stream, and then 
    /// print each token to the screen.
    let displayTokens (analyzer : Analyzer) (text : string) =
        // NOTE: the fieldName "contents" is arbitrary.
        use tokenStream = analyzer.GetTokenStream("contents", new StringReader(text))
        displayTokensFromTokenStream tokenStream

    /// See the term, offsets, type, and position increment of each token.
    let displayTokensWithFullDetails (analyzer : Analyzer) (text : string) =
        // NOTE: the fieldName "contents" is arbitrary.
        use stream = analyzer.GetTokenStream("contents", new StringReader(text))
        
        // Display the type of Analyzer and TokenStream.
        let analyzerTypeName = analyzer.GetType().FullName
        printfn $"Analyzer type: {analyzerTypeName}"
        let tsTypeName = stream.GetType().FullName
        printfn $"TokenStream type: {tsTypeName}"

        let term = stream.GetAttribute<ICharTermAttribute>()

        // Unlike the book, which targets Lucene 3, SimpleAnalyzer doesn't have an IPositionIncrementAttribute. Look in 
        //   the Lucene.NET source file CharTokenizer.cs in the Init() method.
        let posIncrAttr = 
            match stream.HasAttribute<IPositionIncrementAttribute>() with
            | true -> Some (stream.GetAttribute<IPositionIncrementAttribute>())
            | false -> None

        let offset = stream.GetAttribute<IOffsetAttribute>()

        // Same for Type.
        let typeAttr =
            match stream.HasAttribute<ITypeAttribute>() with
            | true -> Some (stream.GetAttribute<ITypeAttribute>())
            | false -> None

        stream.Reset()

        let mutable position = 0

        while stream.IncrementToken() do
            let increment = 
                match posIncrAttr with
                | Some pia -> pia.PositionIncrement
                | None -> 1 // By default, all tokens created by Analyzers and Tokenizers have a PositionIncrement of 1.

            if increment > 0 then do
                position <- position + increment
                printfn ""
                printf $"{position}: "

            let typeName =
                match typeAttr with
                | Some ta -> ta.Type
                | None -> "(no ITypeAttribute in TokenStream)"

            printf $"[{term}:{offset.StartOffset}->{offset.EndOffset}:{typeName}] "

        printfn ""
    
