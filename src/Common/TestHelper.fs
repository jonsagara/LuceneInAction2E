namespace LuceneInAction2E.Common

module TestHelper =
    
    let assertEquals message expected actual =
        if expected = actual then
            printfn $"{message}: {nameof expected} '{expected}' matches {nameof actual} '{actual}'"
        else
            printfn $"ERROR"
            printfn $"{message}: expected value does not equal the actual value"
            printfn $"\texpected: {expected}"
            printfn $"\tactual: {actual}"
        
    let assertTrue message (predicate : bool) =
        if predicate then
            printfn $"{message}: predicate is true as expected"
        else
            printfn $"ERROR"
            printfn $"{message}: predicate is false"
            
    let assertFalse message (predicate : bool) =
        if not(predicate) then
            printfn $"{message}: predicate is false as expected"
        else
            printfn $"ERROR"
            printfn $"{message}: predicate is true"

