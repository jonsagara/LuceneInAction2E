namespace LuceneInAction2E.Chapter04.Analysis

open System.IO
open Lucene.Net.Analysis
open Lucene.Net.Analysis.Core
open LuceneInAction2E.Common

(*
NOTE: At runtime, this fails with:

Unhandled exception. System.BadImageFormatException: Bad IL format.
   at LuceneInAction2E.Chapter04.Analysis.StopAnalyzer2.CreateComponents(String fieldName, TextReader reader)
   at Lucene.Net.Analysis.Analyzer.GetTokenStream(String fieldName, TextReader reader)
   at LuceneInAction2E.Chapter04.Analysis.AnalyzerUtils.assertAnalyzesTo(Analyzer analyzer, String input, String[] output) in C:\Dev\SANDBOX\LuceneInAction2E\src\Chapter04\Analysis\AnalyzerUtils.fs:line 104
   at LuceneInAction2E.Chapter04.Analysis.AnalyzerDemo.testStopAnalyzer2() in C:\Dev\SANDBOX\LuceneInAction2E\src\Chapter04\Analysis\AnalyzerDemo.fs:line 49
   at LuceneInAction2E.Chapter04.Analysis.Program.main(String[] argv) in C:\Dev\SANDBOX\LuceneInAction2E\src\Chapter04\Analysis\Program.fs:line 12

I'm guessing we need to find a way to properly implement CreateComponents instead of relying on calling the case class method.

Not implementing GetTokenStream and instead overriding CreateComponents makes the test pass. ¯\_(ツ)_/¯
*)
type StopAnalyzer2 =
    inherit Analyzer

    val private _stopWords : Util.CharArraySet

    new() = { 
        inherit Analyzer()
        _stopWords = StopAnalyzer.ENGLISH_STOP_WORDS_SET 
        }

    new(stopWords : string[]) = { 
        inherit Analyzer() 
        _stopWords = StopFilter.MakeStopSet(IndexProperties.luceneVersion, stopWords) 
        }


    override this.CreateComponents (fieldName : string, reader : TextReader) =
        //base.CreateComponents(fieldName, reader)

        let letterTokenizer = new LetterTokenizer(IndexProperties.luceneVersion, reader)
        let lowerCaseFilter = new LowerCaseFilter(IndexProperties.luceneVersion, letterTokenizer)
        let stopFilter = new StopFilter(IndexProperties.luceneVersion,lowerCaseFilter, this._stopWords)

        new TokenStreamComponents(letterTokenizer, stopFilter)

    // Let Analyzer take care of loading the TokenStream from the above components.
    //member this.GetTokenStream (fieldName : string, reader : TextReader) =
    //    // Divide the text at non-letters.
    //    let letterTokenizer = new LetterTokenizer(IndexProperties.luceneVersion, reader)
    //    // Lowercase the letters of each token.
    //    let lowerCaseFilter = new LowerCaseFilter(IndexProperties.luceneVersion, letterTokenizer)

    //    // Remove stop words from the token stream.
    //    new StopFilter(IndexProperties.luceneVersion, lowerCaseFilter, this._stopWords)
    

