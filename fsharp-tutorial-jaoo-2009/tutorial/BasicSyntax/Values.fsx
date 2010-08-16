//---------------------------------------------------------------------------
// This is a demonstration script showing how to construct and manipulate
// some basic data values in F#
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// List basics
let aList = [1; 2; 3]

// - Concat (joins two lists)
let combinedList = aList @ [0] @ aList 

// List comprehensions
let l = [1; 3; 5; 7; 9]

let l2 = [1 .. 2 .. 10]

let l3 = [ for i in 1 .. 10 -> (i, i * i)]

// Tuples
let tuple = (1, '2', "three") 

// unit
let u = ()

let print x = printfn "%d" x
let result = print 5

let returnVowels = ['a'; 'e'; 'i'; 'o'; 'i']


// Option types
let sidekickOf x = 
    match x with 
    | "Batman"   -> Some("Robin")
    | "Superman" -> None
    | _ -> None

// List comprehensions  

//let l3 = [ for i in 1 .. 10 -> (i, i * i)]
let l4 = [ for i in 1 .. 10 do
               yield i, i * i ]
 
let l5 = [
            for i in 0 .. 10 do
                for j in 0 .. i do
                    for k in 0 .. j do
                        yield (i, j, k) 
        ]
      
let genList n = 
    [
        let square x = x * x
        for i in 0 .. n do
            yield (i, square i) 
    ]
        
