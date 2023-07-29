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
        
        { IndexDir = indexDir; 
          Searcher = IndexSearcher(DirectoryReader.Open(indexDir)) }

    let private indexSingleFieldDocs (fields : Field[]) =
        let indexDir = new RAMDirectory()
        let writerConfig = IndexWriterConfig(IndexProperties.luceneVersion, new WhitespaceAnalyzer(IndexProperties.luceneVersion))
        use indexWriter = new IndexWriter(indexDir, writerConfig)

        fields
        |> Array.iter (fun field ->
            let doc = Document()
            doc.Add(field)
            indexWriter.AddDocument(doc)
            )

        indexWriter.Commit()

        { IndexDir = indexDir; 
          Searcher = IndexSearcher(DirectoryReader.Open(indexDir)) }
        
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


    //
    // Tests
    //
    
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
        
    let testMultiple () =
        let indexSetup = setup()
        
        assertFalse "not close enough" (matched [| "quick"; "jumped"; "lazy" |] 3 indexSetup.Searcher)
        assertTrue "just enough" (matched [| "quick"; "jumped"; "lazy" |] 4 indexSetup.Searcher)
        assertFalse "almost but not quite" (matched [| "lazy"; "jumped"; "quick" |] 7 indexSetup.Searcher)
        assertTrue "just right" (matched [| "lazy"; "jumped"; "quick" |] 8 indexSetup.Searcher)
        
        tearDown indexSetup
        
    let testWildcard () =
        let (fields : Field[]) = [|
            TextField("contents", "wild", Field.Store.YES)
            TextField("contents", "child", Field.Store.YES)
            TextField("contents", "mild", Field.Store.YES)
            TextField("contents", "mildew", Field.Store.YES)
            |]

        let indexSetup = indexSingleFieldDocs fields

        let query = WildcardQuery(Term("contents", "?ild*"))
        let matches = indexSetup.Searcher.Search(query, 10)

        assertEquals "child no match" 3 matches.TotalHits
        assertEqualsFloatsWithDelta "score the same" matches.ScoreDocs[0].Score matches.ScoreDocs[1].Score 0.0f
        assertEqualsFloatsWithDelta "score the same" matches.ScoreDocs[1].Score matches.ScoreDocs[2].Score 0.0f

        tearDown indexSetup

    let testFuzzy () =
        let fields : Field[] = [|
            TextField("contents", "fuzzy", Field.Store.YES)
            TextField("contents", "wuzzy", Field.Store.YES)
            |]

        let indexSetup = indexSingleFieldDocs fields

        let query = FuzzyQuery(Term("contents", "wuzza"))
        let matches = indexSetup.Searcher.Search(query, 10)

        assertEquals "both close enough" 2 matches.TotalHits
        assertTrue "wuzzy closer than fuzzy" (matches.ScoreDocs[0].Score <> matches.ScoreDocs[1].Score)

        let doc = indexSetup.Searcher.Doc(matches.ScoreDocs[0].Doc)
        assertEquals "wuzza bear" "wuzzy" (doc.Get("contents"))

        tearDown indexSetup