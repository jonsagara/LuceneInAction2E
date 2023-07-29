namespace LuceneInAction2E.Chatper03.Searcher

open Lucene.Net.Analysis.Core
open Lucene.Net.Documents
open Lucene.Net.Index
open Lucene.Net.Search
open Lucene.Net.Store
open LuceneInAction2E.Common
open LuceneInAction2E.Common.TestHelper

module PhraseQueryTest =
    
    type private SetupResult = {
            IndexDir : Directory
            Searcher : IndexSearcher
        }
    
    let private setup () =
        let indexDir = new RAMDirectory()
        let writerConfig = IndexWriterConfig(IndexProperties.luceneVersion, new WhitespaceAnalyzer(IndexProperties.luceneVersion))
        use indexWriter = new IndexWriter(indexDir, writerConfig)
        
        let doc = Document()
        doc.Add(TextField("field", "the quick brown fox jumped over the lazy dog", Field.Store.YES))
        indexWriter.AddDocument(doc)
        indexWriter.Commit()
        
        { IndexDir = indexDir; Searcher = IndexSearcher(DirectoryReader.Open(indexDir)) }
        
    let private tearDown (setupResult : SetupResult) =
        setupResult.IndexDir.Dispose()
        setupResult.Searcher.IndexReader.Dispose()
        
    let private matched (phrase: string[]) (slop : int) (searcher : IndexSearcher) =
        let query = PhraseQuery()
        query.Slop <- slop
        
        phrase
        |> Array.iter (fun word -> query.Add(Term("field", word)))
        
        let matches = searcher.Search(query, 10)
        matches.TotalHits > 0
    
    let testSlopComparison () =
        let indexSetup = setup()
        
        let phrase = [| "quick"; "fox"; |]
        
        assertFalse "exact phrase not found" (matched phrase 0 indexSetup.Searcher)
        assertTrue "close enough" (matched phrase 1 indexSetup.Searcher)
        
        tearDown indexSetup
        
    let testReverse () =
        let indexSetup = setup()
        
        let phrase = [| "fox";  "quick"; |]
        
        assertFalse "hop flop" (matched phrase 2 indexSetup.Searcher)
        assertTrue "hop hop slop" (matched phrase 3 indexSetup.Searcher)
        
        tearDown indexSetup
        

