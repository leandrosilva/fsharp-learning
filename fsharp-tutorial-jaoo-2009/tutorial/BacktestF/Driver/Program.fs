open System
open StockTicker.Common
open StockTicker.Loader
open StockTicker.Algorithms
open StockTicker.Persistence


let main () = 
    let args = Environment.GetCommandLineArgs ()
    if args.Length <> 4 then
        printfn "Usage is ReturnCalculator tickerList startDate endDate where tickerlist is in the form [ticker1 ticker2 ... tickerN]\nInstead passing %d values" args.Length
        Environment.Exit(0)
    let tickers = args.[1].Split([|' ';'[';']'|], StringSplitOptions.RemoveEmptyEntries)
    let startDate = DateTime.Parse(args.[2])
    let endDate = DateTime.Parse(args.[3])

    let tickerSpanTuples =
        tickers
        |> Array.map (fun ticker -> ticker, {Start = startDate; End = endDate})
        |> Array.toList
    let results = calculateYearlyReturnsAsync tickerSpanTuples
    
    printfn "Downloading data ..."
    Async.RunSynchronously(results) |> Array.iter (fun (ticker, _, ret) -> printfn "%s\t%3.2f%%" ticker (float ret * 100.))

main ()