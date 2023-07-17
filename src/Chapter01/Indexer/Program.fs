namespace LuceneInAction2E.Chapter01.Indexer

open System.Diagnostics

module Program =

    [<EntryPoint>]
    let main argv =

        let startTimestamp = Stopwatch.GetTimestamp()
        use indexer = new Indexer("index")

        let numIndexed = indexer.index("data")
        let elapsed = Stopwatch.GetElapsedTime(startTimestamp)

        printfn $"Indexing {numIndexed} files took {elapsed}."

        0
