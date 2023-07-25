namespace LuceneInAction2E.Chapter00.CreateTestIndex

open System.IO
open Lucene.Net.Analysis.Standard
open Lucene.Net.Util
open Lucene.Net.Analysis.Util
open Lucene.Net.Analysis.Core
open Lucene.Net.Analysis

(*
    The Java sample code shows deriving from StandardAnalyzer to customize its behavior, however, the version
    in Lucene.NET is a sealed class, which means we can't derive from it.

    Instead, mimic the StandardAnalyzer code from 4.8.0.beta00016, but with the additional customizations from 
    the Java version.
*)

(*
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */
*)

/// <summary>
/// Filters <see cref="StandardTokenizer"/> with <see cref="StandardFilter"/>, 
/// <see cref="LowerCaseFilter"/> and <see cref="StopFilter"/>, using a list of
/// English stop words.
/// 
/// <para>You must specify the required <see cref="LuceneVersion"/>
/// compatibility when creating <see cref="StandardAnalyzer"/>:
/// <list type="bullet">
///   <item><description> As of 3.4, Hiragana and Han characters are no longer wrongly split
///        from their combining characters. If you use a previous version number,
///        you get the exact broken behavior for backwards compatibility.</description></item>
///   <item><description> As of 3.1, <see cref="StandardTokenizer"/> implements Unicode text segmentation,
///        and <see cref="StopFilter"/> correctly handles Unicode 4.0 supplementary characters
///        in stopwords.  <see cref="ClassicTokenizer"/> and <see cref="ClassicAnalyzer"/> 
///        are the pre-3.1 implementations of <see cref="StandardTokenizer"/> and
///        <see cref="StandardAnalyzer"/>.</description></item>
///   <item><description> As of 2.9, <see cref="StopFilter"/> preserves position increments</description></item>
///   <item><description> As of 2.4, <see cref="Token"/>s incorrectly identified as acronyms
///        are corrected (see <a href="https://issues.apache.org/jira/browse/LUCENE-1068">LUCENE-1068</a>)</description></item>
/// </list>
/// </para>
/// </summary>
type MyStandardAnalyzer(matchVersion : LuceneVersion, stopWords : CharArraySet) =
    inherit StopwordAnalyzerBase(matchVersion, stopWords)

    static let STOP_WORDS_SET = StopAnalyzer.ENGLISH_STOP_WORDS_SET

    /// <summary>
    /// Default maximum allowed token length
    /// </summary>
    static let DEFAULT_MAX_TOKEN_LENGTH = 255

    let mutable maxTokenLength = DEFAULT_MAX_TOKEN_LENGTH

    /// <summary>
    /// Builds an analyzer with the default stop words (<see cref="STOP_WORDS_SET"/>).
    /// </summary>
    /// <param name="matchVersion"> Lucene compatibility version - See <see cref="StandardAnalyzer"/> </param>
    new(matchVersion : LuceneVersion) =
        new MyStandardAnalyzer(matchVersion, STOP_WORDS_SET)

    /// <summary>
    /// Builds an analyzer with the stop words from the given reader.
    /// </summary>
    /// <seealso cref="WordlistLoader.GetWordSet(TextReader, LuceneVersion)"/>
    /// <param name="matchVersion"> Lucene compatibility version - See <see cref="StandardAnalyzer"/> </param>
    /// <param name="stopwords"> <see cref="TextReader"/> to read stop words from  </param>
    new(matchVersion : LuceneVersion, stopwords : TextReader) =
        new MyStandardAnalyzer(matchVersion = matchVersion, stopWords = StopwordAnalyzerBase.LoadStopwordSet(stopwords, matchVersion))

    /// <summary>
    /// Set maximum allowed token length.  If a token is seen
    /// that exceeds this length then it is discarded.  This
    /// setting only takes effect the next time tokenStream or
    /// tokenStream is called.
    /// </summary>
    member this.MaxTokenLength
        with get() = maxTokenLength
        and set value = maxTokenLength <- value

    override this.CreateComponents (fieldName : string, reader : TextReader) =
        let src = new StandardTokenizer(this.m_matchVersion, reader)
        src.MaxTokenLength <- maxTokenLength
        let mutable tok : TokenStream = new StandardFilter(this.m_matchVersion, src)
        tok <- new LowerCaseFilter(this.m_matchVersion, tok)
        tok <- new StopFilter(this.m_matchVersion, tok, this.m_stopwords)
        new TokenStreamComponentsAnonymousClass(this, src, tok)

    /// Override multi-valued position increment
    override this.GetPositionIncrementGap(field : string) =
        if field = "contents" then
            100
        else
            0

and TokenStreamComponentsAnonymousClass(outerInstance : MyStandardAnalyzer, src : StandardTokenizer, tok : TokenStream) =
    inherit TokenStreamComponents(src, tok)

    let outerInstance = outerInstance
    let src = src

    override this.SetReader(reader : TextReader) =
        src.MaxTokenLength <- outerInstance.MaxTokenLength
        base.SetReader(reader)