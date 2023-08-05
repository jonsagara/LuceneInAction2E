﻿namespace LuceneInAction2E.Chapter04.Analysis

open System.IO
open Lucene.Net.Analysis
open Lucene.Net.Analysis.TokenAttributes
open Lucene.Net.Util

module AnalyzerUtils =

    //
    // Private functions
    //

    /// If the TokenStream has the given attribute, get it and return it.
    let private getAttribute<'a when 'a :> IAttribute> (stream : TokenStream) =
        match stream.HasAttribute<'a>() with
        | true -> Some (stream.GetAttribute<'a>())
        | false -> None


    //
    // Public functions
    //

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
        printfn $"{nameof Analyzer} type: {analyzer.GetType().FullName}"
        printfn $"{nameof TokenStream} type: {stream.GetType().FullName}"

        //
        // NOTE: You interact with a separate reused attribute interface for each element of the token (term, offsets, 
        //   position increments, etc.). You first obtain the concrete implemtation of the attributes of interest. Then,
        //   you iterate through all tokens by calling IncrementToken(). This method returns true if it has advanced to a
        //   new token and false once you've exhausted the stream. You then interact with the previously obtained 
        //   attributes to get that attribute's value for each token. When IncrementToken() returns true, all attributes
        //   within it will have altered their internal state to the next token.
        //

        let term = stream.GetAttribute<ICharTermAttribute>()

        // Unlike the book, which targets Lucene 3, SimpleAnalyzer doesn't have an IPositionIncrementAttribute. Look in 
        //   the Lucene.NET source file CharTokenizer.cs in the Init() method.
        let posIncrAttr = getAttribute<IPositionIncrementAttribute> stream

        let offset = stream.GetAttribute<IOffsetAttribute>()

        // SimpleAnalyzer also doesn't have an ITypeAttribute.
        let typeAttr = getAttribute<ITypeAttribute> stream

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
    
