//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of using F# 
// meta-programming using quotations.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

// Literals
let printQuotation x =
    match x with
    | Int32  v -> printfn "x is an int with value %d" v
    | String v -> printfn "x is a string with value %s" v
    | Double v -> printfn "x is a float with value %f" v
    | _        -> printfn "I don't know what x is"

printQuotation <@@ 1   @@>
printQuotation <@@ 1.0 @@>
printQuotation <@@ "x" @@>


// Functions
[<ReflectedDefinition>]
let rec factorial = function 0 -> 1 | x -> x * factorial (x - 1)

let rec describeQuotation = 
    function
    | Lambda (_, e) ->
        printfn "Expr is a Lambda..."
        describeQuotation e
    | Call(_, mi, _) ->
        printfn "Expr is a Call to '%s'..." mi.Name
    | x -> 
        printfn "And so on..."

describeQuotation <@@ describeQuotation @@>