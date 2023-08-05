namespace LuceneInAction2E.Chapter04.Analysis

open System

module Program =

    [<EntryPoint>]
    let main argv =
        
        //AnalyzerDemo.doDemo (Array.Empty<string>())
        AnalyzerDemo.doDemoWithFullDetails ()

        0