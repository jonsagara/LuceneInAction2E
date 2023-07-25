namespace LuceneInAction2E.Common

open Lucene.Net.Util
open Lucene.Net.Store
open System.IO

module IndexProperties =

    /// The version of Lucene.NET used in these examples.
    let luceneVersion = LuceneVersion.LUCENE_48

    /// The name of the index containing book metadata.
    [<Literal>]
    let bookIndexName = "book_index"

    /// Gets the path to the book_index directory from the runtime working directory that is a subdirectory
    /// of the bin directory.
    let bookIndexDirFromBinFolder = Path.GetFullPath(Path.Combine("../../../../..", bookIndexName))