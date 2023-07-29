namespace LuceneInAction2E.Common

open System

/// Bad approximation of JUnit assertions. The main difference is that here we print a message for the 
/// affirmative case.
module TestHelper =

    type AssertionFailedException(message : string) =
        inherit Exception(message)


    //
    // Private functions
    //

    let private formatSuccess message expected actual =
        $"{message}: Success. Expected:<{expected}> and it was:<{actual}>"

    let private failNotEquals (message : string) expected actual =
        raise(AssertionFailedException($"{message}: ERROR. Expected:<{expected}> but was:<{actual}>"))
    

    //
    // Public assertion functions
    //

    let assertEquals message expected actual =
        if expected = actual then
            printfn "%s" (formatSuccess message expected actual)
        else
            failNotEquals message expected actual

    let assertEqualsFloatsWithDelta (message : string) (expected : float32) (actual : float32) (delta : float32) =
        if expected.CompareTo(actual) = 0 then 
            // Doubles are already equal. Nothing further to test.
            printfn "%s" (formatSuccess message expected actual)
        elif not(Math.Abs(expected - actual) <= delta) then
            failNotEquals message expected actual

    let assertEqualsDoublesWithDelta (message : string) (expected : float) (actual : float) (delta : float) =
        if expected.CompareTo(actual) = 0 then 
            // Doubles are already equal. Nothing further to test.
            printfn "%s" (formatSuccess message expected actual)
        elif not(Math.Abs(expected - actual) <= delta) then
            failNotEquals message expected actual

    let assertFalse message (predicate : bool) =
        if not(predicate) then
            printfn "%s" (formatSuccess message false predicate)
        else
            failNotEquals message false predicate
        
    let assertTrue message (predicate : bool) =
        if predicate then
            printfn "%s" (formatSuccess message true predicate)
        else
            failNotEquals message true predicate

