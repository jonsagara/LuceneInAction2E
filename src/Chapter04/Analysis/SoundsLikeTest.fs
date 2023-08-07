namespace LuceneInAction2E.Chapter04.Analysis

open LuceneInAction2E.Chapter04.Analysis.SoundsLike
open Lucene.Net.Documents
open Lucene.Net.Index
open Lucene.Net.QueryParsers.Classic
open Lucene.Net.Search
open Lucene.Net.Store
open LuceneInAction2E.Common
open LuceneInAction2E.Common.TestHelper

module SoundsLikeTest =

    let testKoolKat () =
        use directory = new RAMDirectory()
        use analyzer = new MetaphoneReplacementAnalyzer()
        
        let writerConfig = IndexWriterConfig(IndexProperties.luceneVersion, analyzer)
        writerConfig.OpenMode <- OpenMode.CREATE
        use writer = new IndexWriter(directory, writerConfig)

        // Add Document with contents "cool cat".
        let docToAdd = Document()
        docToAdd.Add(TextField("contents", "cool cat", Field.Store.YES))
        writer.AddDocument(docToAdd)
        writer.Commit()

        use reader = DirectoryReader.Open(directory)
        let searcher = IndexSearcher(reader)
        
        // Search with query "kool kat", using the same Metaphone Analyzer as the indexer.
        let parser = QueryParser(IndexProperties.luceneVersion, "contents", analyzer)
        let query = parser.Parse("kool kat")

        let hits = searcher.Search(query, 1)
        assertEquals "Hit count" 1 hits.TotalHits

        let docId = hits.ScoreDocs[0].Doc
        let docReadFromSearcher = searcher.Doc(docId)
        assertEquals "Sounds Like" "cool cat" (docReadFromSearcher.Get("contents"))


