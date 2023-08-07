namespace LuceneInAction2E.Chapter04.Analysis.Synonym

open System
open System.Collections.Generic

type ISynonymEngine =
    abstract member GetSynonyms : string -> string[]

type TestSynonymEngine() =
    
    let _synonyms = Dictionary<string, string[]>(dict [|
        "quick", [| "fast"; "speedy" |]
        "jumps", [| "leaps"; "hops" |]
        "over", [| "above" |]
        "lazy", [| "apathetic"; "sluggish" |]
        "dog", [| "canine"; "pooch" |]
        |])

    interface ISynonymEngine with
        member this.GetSynonyms (s : string) =
            let success, synonyms = _synonyms.TryGetValue(s)

            match success with
            | true -> synonyms
            | false -> 
                // The sample code returns null. We're more civilized, so we'll return an empty array.
                Array.Empty<string>()

    


