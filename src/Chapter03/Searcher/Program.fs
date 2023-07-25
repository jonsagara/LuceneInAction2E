namespace LuceneInAction2E.Chapter03.Searcher

open System.IO
open LuceneInAction2E.Chatper03.Searcher

module Program =

    [<EntryPoint>]
    let main argv =

        BasicSearchingTest.testTerm()
        
        0
