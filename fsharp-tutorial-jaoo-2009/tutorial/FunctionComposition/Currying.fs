

let add x y = x + y

let addTo10 = add 10

let printInteger x  = printfn "%d" x
let curriedPrintInt = printfn "%d"

[1 .. 10] |> List.iter curriedPrintInt

// Curried symbolic function
let secretNumber = Array.filter ( (=) 3 ) [| 1 .. 30 |]