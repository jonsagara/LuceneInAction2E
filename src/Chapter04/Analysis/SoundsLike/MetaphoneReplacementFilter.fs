namespace LuceneInAction2E.Chapter04.Analysis.SoundsLike

open Lucene.Net.Analysis
open Lucene.Net.Analysis.TokenAttributes
open Lucene.Net.Analysis.Phonetic.Language

type MetaphoneReplacementFilter(input : TokenStream) as this =
    inherit TokenFilter(input)

    let _metaphoner : Metaphone = Metaphone()
    let mutable _termAttr : ICharTermAttribute = null
    let mutable _typeAttr : ITypeAttribute = null

    do
        _termAttr <- this.GetAttribute<ICharTermAttribute>()
        _typeAttr <- this.GetAttribute<ITypeAttribute>()


    override this.IncrementToken () =
        if not(this.m_input.IncrementToken()) then
            false
        else
            let encoded = _metaphoner.Encode(_termAttr.ToString())
            let encodedChars = encoded.ToCharArray()
            _termAttr.CopyBuffer(encodedChars, 0, encodedChars.Length)
            _typeAttr.Type <- MetaphoneHelper.METAPHONE
            true