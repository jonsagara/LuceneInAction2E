namespace LuceneInAction2E.Chapter00.CreateTestIndex

open System.Diagnostics
open System.IO

module Program =

    [<EntryPoint>]
    let main argv =

        // The files are located in the data directory.
        let dataDir = Path.GetFullPath("./data")
        //printfn $"data: {dataDir}"

        // Put the index into the src directory, in a directory called test_index.
        let indexDir = Path.GetFullPath(Path.Combine("../../../../..", "test_index"))
        //printfn $"test_index: {indexDir}"

        //// Find all .properties files in the data directory and its subdirectories.
        //let propsFiles = Directory.GetFiles(dataDir, "*.properties", SearchOption.AllDirectories)
        //printfn $"{propsFiles.Length} books to index"

        //propsFiles
        //|> Array.iter (fun filePath -> 
        //    printfn $"{filePath}"
        //    let parentDirInfo = Directory.GetParent filePath
        //    printfn $"Parent directory: {parentDirInfo.FullName}"
        //    let category = parentDirInfo.FullName.Substring(dataDir.Length)
        //    printfn $"Category raw: {category}"
        //    let cat2 = category.Replace("\\", "/")
        //    printfn $"Category separaters: {cat2}"
        //    )

        let startTimestamp = Stopwatch.GetTimestamp()

        use indexer = new Indexer(dataDir, indexDir)
        let numIndexed = indexer.index()
        let elapsed = Stopwatch.GetElapsedTime(startTimestamp)

        printfn $"Indexing {numIndexed} files took {elapsed}."

        0