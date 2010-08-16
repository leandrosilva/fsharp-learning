module StockTicker.Algorithms

open System
open StockTicker.Common
open StockTicker.Loader

let getClose d observations =
    let priceClose obs =
        match obs.Event with Price(p) -> p.Close | _ -> invalidArg "obs" "Not a Price"
    let optObs =
        observations
        |> List.tryPick (fun x -> if x.Date = d then Some(x) else None)
    match optObs with
        | None  -> failwith (sprintf "GetClose %A: No obs at this date" d)
        | Some(obs) -> obs |> priceClose

let getDiv d observations =
    let dividend obs = match obs.Event with Dividend(amount) -> amount | _ -> invalidArg "obs" "Not a dividend"
    observations |> List.filter (fun x -> match x.Event with Dividend(_) when x.Date = d -> true | _ -> false) |> List.tryPick someIdem
                 |> Option.get |> dividend 

let compoundYearlyReturn ((startDate:DateTime), (startPrice:float<money>)) (endDate, endPrice) =
    let days = float (endDate - startDate).Days
    let dailyReturn = (endPrice / startPrice) ** (1. / days) - 1.
    rateOfReturn((1. + dailyReturn) ** 365. - 1.)

let calcCompoundRetGivenTickers span asHappenedObs =
    let adj = asHappenedObs |> adjusted adjStart
    compoundYearlyReturn (span.Start, getClose span.Start adj) (span.End, getClose span.End adj)
    
let calculateYearlyReturnsAsync tickerSpanTuples =
    async {
        let! tickers = loadTickersAsync tickerSpanTuples
        return tickers
                    |> Array.map (fun (ticker, span, obs) -> ticker, span, obs |> asHappened 1.)
                    |> Array.map (fun (ticker, span, asH) -> ticker, span, asH |> calcCompoundRetGivenTickers span)}
         