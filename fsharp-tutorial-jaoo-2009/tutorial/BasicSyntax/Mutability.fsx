//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of scoping
// and mutability in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// Scoping 
let result =
    let x1 = 1
    let x2 = x1 + x1
    let x3 = x2 + x2
    let x4 = x3 + x3
    x4
    
let mutable x = 1
x <- x + x
x <- x + x
x <- x + x

x

(*
let _ =
    let mutable listElements = 0
    [1 .. 100] |> List.iter (fun x -> listElements <- listElements + 1)
*)

let _ =
    let listElements = ref 0
    
    [1 .. 100] |> List.iter (fun x -> listElements := !listElements + 1)
    
    printfn "There are %d elements in the list." !listElements







  
// Exceptions
    
exception BadMojo of int
exception BadJuju

let castHex (incantation : string) =
    if incantation.Length > 100 then 
        failwith "Bad omen"
    if incantation.Length % 20 = 0 then
        raise (BadMojo (incantation.Length / 20))
    if incantation.Contains("xyzzy") then
        raise BadJuju
        
// Safe hex casting
try
    castHex "abracadabraabracadab"
with 
| BadMojo(x) -> printfn "Caught a BadMojo exception."
| BadJuju    -> printfn "Caught a BadJuju exception."
| e          -> printfn "Caught a %s exception\n%s" e.Message e.StackTrace

// File IO and Resource Cleanup!

open System.IO
open System.Collections.Generic

let readAllLines(file) =
    use inp = File.OpenText file
    
    let res = new ResizeArray<_>()
    
    while not inp.EndOfStream do

        res.Add(inp.ReadLine())

    res


    
readAllLines @"c:\fsharp\log-mDefaultDomain.log"



let readAllLinesDesugared(file) =
    let inp = File.OpenText file
    try
        let res = new ResizeArray<_>()
        while not inp.EndOfStream do
            res.Add(inp.ReadLine())
        res
    finally 
        (inp :> System.IDisposable).Dispose()
        
        
        
readAllLinesDesugared @"c:\fsharp\log-mDefaultDomain.log"
        