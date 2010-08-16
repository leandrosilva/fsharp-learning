module StockTicker.Common 

open System

[<Measure>] type money
let money (f:float) = f * 1.<money>
[<Measure>] type shares
let shares (f:float) = f * 1.<shares>
[<Measure>] type volume
let volume (f:float) = f * 1.<volume>
[<Measure>] type rateOfReturn
let rateOfReturn (f:float) = f * 1.<rateOfReturn>

type Span = { Start: DateTime; End: DateTime }
type Price = { Open: float<money>; High: float<money>; Low:float<money>; Close:float<money>; Volume: float<volume>}
type Event =
    | Price of Price
    | Split of float
    | Dividend of float<money>
type Observation = { Date: DateTime; Event: Event}

let span sy sm sd ey em ed = {Start = new DateTime(sy, sm, sd); End = new DateTime(ey, em, ed)}
let date y m d = new DateTime(y, m, d)
let now () = DateTime.Now

let idem x = x
let someIdem x = Some(x)

open System.Threading

// Reusable concept of calling a reporting function on the UI thread
type Reporter(f) =
    let reportFunction = f
    let uiThread = SynchronizationContext.Current
    member r.ReportStatus percentageCompletion (displayString: string) =
        async {
            if percentageCompletion < 0. || percentageCompletion > 1. then
                failwith "Percentage of completion must be betwen 0 and 1"
            do! Async.SwitchToContext uiThread
            do reportFunction percentageCompletion displayString
            do! Async.SwitchToThreadPool ()
            }

type Async with 
    static member Parallel3 (p1,p2,p3) = 
        async { let! res = Async.Parallel [ async { let! res = p1 in return Choice1Of3 res }; 
                                            async { let! res = p2 in return Choice2Of3 res }
                                            async { let! res = p3 in return Choice3Of3 res } ] 
                match res with
                | [| Choice1Of3 r1; Choice2Of3 r2; Choice3Of3 r3 |] -> return (r1,r2,r3)
                | _ -> return! failwith "unrechable" }
                    
