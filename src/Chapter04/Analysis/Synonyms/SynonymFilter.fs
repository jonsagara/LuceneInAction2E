namespace LuceneInAction2E.Chapter04.Analysis.Synonym

open System.Collections.Generic
open Lucene.Net.Analysis
open Lucene.Net.Analysis.TokenAttributes
open Lucene.Net.Util

type SynonymFilter(input : TokenStream, engine : ISynonymEngine) as this =
    inherit TokenFilter(input)

    let _engine = engine
    let _synonymStack = Stack<string>()
    let mutable _termAttr : ICharTermAttribute = null
    let mutable _posIncrAttr : IPositionIncrementAttribute = null
    let mutable _current : AttributeSource.State = null

    do
        _termAttr <- this.GetAttribute<ICharTermAttribute>()
        _posIncrAttr <- this.GetAttribute<IPositionIncrementAttribute>()


    /// If the term has synonyms, add them to the stack for processing.
    member private this.AddAliasesToStack () =
        let synonyms = _engine.GetSynonyms(_termAttr.ToString())

        if synonyms.Length = 0 then
            false
        else
            for synonym in synonyms do
                _synonymStack.Push(synonym)

            true

    override this.IncrementToken () =
        if _synonymStack.Count > 0 then
            //
            // A previous invocation identified synonyms for the term. Put those synonyms at the 
            //   same position increment as the original term.
            //

            // Get a synonym.
            let syn = _synonymStack.Pop()

            // Restore the state of attributes to when the synonyms were identified.
            this.RestoreState(_current)

            // Copy the synonym to the term, using the same position as the original term.
            let synChars = syn.ToCharArray()
            _termAttr.CopyBuffer(synChars, 0, synChars.Length)
            _posIncrAttr.PositionIncrement <- 0

            // Tell the caller to try to keep processing terms.
            true
        elif not(this.m_input.IncrementToken()) then
            // There are no more terms to process.
            false
        else
            // There are currently no synonyms on the stack. Check the current token to see if it
            //   has any synonyms. If so, add them to the stack, and capture the filter's attribute state.
            if this.AddAliasesToStack() then do
                _current <- this.CaptureState()
            
            // Tell the caller to try to keep processing terms. This really means that they'll process any
            //   added synonyms.
            true
    
