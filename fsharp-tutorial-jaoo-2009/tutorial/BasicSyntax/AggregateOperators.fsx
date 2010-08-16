//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of using aggregate 
// operators in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

// Iterate
[1 .. 10] |> List.iter (fun i -> printfn "i = %d" i) 

// Map
[1 .. 10]    |> List.map (fun i -> i.ToString())

['a' .. 'z'] |> List.map (fun c -> System.Char.ToUpper(c))

// Sum, Average, Min, Max
[1 .. 5] |> List.sum 

[1 .. 5] 
    |> List.map (fun x ->  x * 2) 
    |> List.map (fun x ->  x * 2) 
    |> List.map (fun x ->  x * 2) 
    |> List.map (fun x ->  x * 2) 
    |> List.map (fun x ->  x * 2) 
    |> List.map (fun x ->  x * 2) 
    |> List.sum

[ 10.0; 3.0; 7.0 ] 
   |> List.average

[ 10.0; 3.0; 7.0 ] 
   |> List.min

[ 10.0; 3.0; 7.0 ] 
   |> List.max

// sumBy, averageBy, minBy, maxBy
[ "one"; "two"; "three" ]

[ "one"; "two"; "three" ]  |> List.sumBy (fun x -> x.Length)

[ (10.0, "Kurt"); (3.0,"Stefan"); (7.0,"Barbara") ] |> List.averageBy fst

[ (10.0, "Kurt"); (3.0,"Stefan"); (7.0,"Barbara") ] |> List.minBy fst

[ (10.0, "Kurt"); (3.0,"Stefan"); (7.0,"Barbara") ] |> List.maxBy fst

// Folds
List.fold  (+) 0 [1 .. 5]   // ((((0 + 1) + 2) + 3) + 4) + 5

List.fold  (*) 1 [1 .. 5]   // ((((0 * 1) * 2) * 3) * 4) * 5

// Fold/FoldBack 

List.fold     (+) 0 [1 .. 5]  // ((((0 + 1) + 2) + 3) + 4) + 5

List.foldBack (+) [1 .. 5] 0  // 1 + (2 + (3 + (4 + (5 + 0))))

List.fold     (-) 0 [1 .. 5]  // ((((0 - 1) - 2) - 3) - 4) - 5

List.foldBack (-) [1 .. 5] 0  // 1 - (2 - (3 - (4 - (5 - 0))))


// Many other operators under List.*, e.g. List.reduce:    
[1 .. 5] 
|> List.map (fun i -> i.ToString()) 
|> List.reduce(fun a b -> sprintf "%s, %s" a b)

