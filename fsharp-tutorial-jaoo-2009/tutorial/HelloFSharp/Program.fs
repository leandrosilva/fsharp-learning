let map f e =
  seq { for x in e -> f x }

  
  
  
  
  
  










































































//•   File new project, Visual F#, console app (just another VS lang)
// (add ref, projects, debugger, .net, ... all stuff you know & love)
 
//•   Open <namespace> (intellisense)
open System.Collections.Generic 
 
//•   Let to bind values (type inferred, tooltips)
let x = 42
let s = "Hello"
let d = new Dictionary<string,int>()
d.Add(s,x)
 
//•   Let to bind functions (types often inferred, '->', lack of function declaration/call parens)
let Square x = x * x
 
//•   Basic list literals, List.map square example
let l = [ 1; 2; 3 ]
let squares = List.map Square l
 
//•   Lambda (‘fun’)
let squares = List.map (fun x -> x*x) l
 
//•   Pipeline (map squares and sum)
let squares = 
    l
    |> List.map Square
    |> List.sum
    
let squares'' = List.map Square >> List.sum
let squares''' = List.sum << List.map Square 
 
//•   Significant whitespace
let SumOfSquares l =    
    l
    |> List.map Square
    |> List.sum 
 
//•   Immutable by default (assign op, implications: func style, no-mutable-shared-memory -> can scale up)
x <- 100
d.["foo"] <- 100
 
//•   For…in, if…then…[else]
for i in 1..10 do
    if i%2 = 0 then
        printfn "hi"
    else
        printfn "bye"