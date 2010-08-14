let rec fib x =
  if x <= 2 then 1 else fib(x - 1) + fib(x - 2)

// (1) "async { ... }" is used to specify a number of CPU tasks
//
// (2) These are composed in parallel using the fork-join combinator Async.Parallel
//
// (3) In this case the composition is executed using Async.RunSynchronously, which
//     starts an instance of the async and synchronously waits for the overall result. 
let fibs =
  Async.Parallel [for i in 0..40 -> async { return fib(i) }]
  |> Async.RunSynchronously

printfn "fibs = %A" fibs