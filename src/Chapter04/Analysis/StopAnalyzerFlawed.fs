namespace LuceneInAction2E.Chapter04.Analysis

open System.IO
open Lucene.Net.Analysis
open Lucene.Net.Analysis.Core
open LuceneInAction2E.Common

type StopAnalyzerFlawed =
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

        let letterTokenizer = new LetterTokenizer(IndexProperties.luceneVersion, reader)

        // Compared to StopAnalyzer2, the order of StopFilter and LowerCaseFilter are swapped. This means 
        //   not all stop words will match because they haven't been lowercased.
        let stopFilter = new StopFilter(IndexProperties.luceneVersion, letterTokenizer, this._stopWords)
        let lowerCaseFilter = new LowerCaseFilter(IndexProperties.luceneVersion,  stopFilter)

        new TokenStreamComponents(letterTokenizer, lowerCaseFilter)
    

