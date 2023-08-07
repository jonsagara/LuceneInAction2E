namespace LuceneInAction2E.Chapter04.Analysis

open System

module Program =

    [<EntryPoint>]
    let main argv =
        
        //AnalyzerDemo.doDemo (Array.Empty<string>())
        //AnalyzerDemo.doDemoWithFullDetails ()
        //AnalyzerDemo.testStopAnalyzer2 ()
        //AnalyzerDemo.testStopAnalyzerFlawed ()

        //SoundsLikeTest.testKoolKat ()
        //AnalyzerDemo.displayMetaphoneReplacementTokens ()

        //SynonymTest.testJumps ()
        //SynonymTest.testSearchByAPI ()
        //SynonymTest.testWithQueryParser ()
        SynonymTest.viewSyonymAnalyzerTokens (Array.Empty<string>())

        0