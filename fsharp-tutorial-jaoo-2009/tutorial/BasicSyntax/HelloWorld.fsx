//---------------------------------------------------------------------------
// This is a demonstration script showing the very simplest applications in F#
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// The simplest app
printfn "Hello, World"










// Another simple app
let numbers = [1 .. 10]
let square x = x * x

let squares = List.map square numbers

printfn "squares = %A" squares

open System
Console.WriteLine("(press any key)")
Console.ReadKey(true) |> ignore