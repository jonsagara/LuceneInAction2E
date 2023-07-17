namespace LuceneInAction2E.Chapter01.Searcher

open System.IO

module Program =

    [<EntryPoint>]
    let main argv =
        // Indexer put the index into the parent Chatper01 directory, in a directory called license_index.
        let chapter01Dir = Path.GetFullPath("../../../..")
        let indexDir = Path.Combine(chapter01Dir, "license_index")
        //printfn $"Chapter01? {chapter01Dir}"

        Searcher.search indexDir "patent"
        0
