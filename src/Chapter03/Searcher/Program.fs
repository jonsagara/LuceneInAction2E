namespace LuceneInAction2E.Chapter03.Searcher

open LuceneInAction2E.Chatper03.Searcher

module Program =

    [<EntryPoint>]
    let main argv =

        //BasicSearchingTest.testTerm()
        //BasicSearchingTest.testQueryParser()
        //BasicSearchingTest.testNearRealTime()
        //BasicSearchingTest.testExplanation()
        //BasicSearchingTest.testKeyword()
        //BasicSearchingTest.testTermRangeQuery()
        //BasicSearchingTest.testInclusiveNumericRangeQuery()
        //BasicSearchingTest.testExclusiveNumericRangeQuery()
        //BasicSearchingTest.testPrefixQuery()
        //BasicSearchingTest.testBooleanAndQuery()
        //BasicSearchingTest.testBooleanOrQuery()
        
        //PhraseQueryTest.testSlopComparison()
        //PhraseQueryTest.testReverse()
        //PhraseQueryTest.testMultiple()
        //PhraseQueryTest.testWildcard()
        //PhraseQueryTest.testFuzzy()

        //QueryParserTest.testToString()
        //QueryParserTest.testTermQuery()
        //QueryParserTest.testTermRangeQuery()
        //QueryParserTest.testLowercasing()
        //QueryParserTest.testPhraseQuery()
        //QueryParserTest.testSlop()
        QueryParserTest.testFuzzyQuery()

        0
