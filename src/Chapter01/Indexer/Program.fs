namespace LuceneInAction2E.Chapter01.Indexer

open System.Diagnostics
open System.IO

module Program =

    [<EntryPoint>]
    let main argv =

        // Put the index into the parent Chatper01 directory, in a directory called license_index.
        let chapter01Dir = Path.GetFullPath("../../../..")
        //printfn $"Chapter01? {chapter01Dir}"

        let startTimestamp = Stopwatch.GetTimestamp()
        use indexer = new Indexer(Path.Combine(chapter01Dir, "license_index"))

        let numIndexed = indexer.index("data")
        let elapsed = Stopwatch.GetElapsedTime(startTimestamp)

        printfn $"Indexing {numIndexed} files took {elapsed}."

        0
