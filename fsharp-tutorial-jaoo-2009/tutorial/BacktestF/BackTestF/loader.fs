module StockTicker.Loader

open System
open System.Net
open System.IO
open System.Threading
open System.Text.RegularExpressions
open StockTicker.Common
open StockTicker.Html

let adjStart = (1., 0.<money>, 1., 1., 1., 1., 1.)
 
let commonUrl ticker span =
    @"http://ichart.finance.yahoo.com/table.csv?s=" + ticker + "&a=" + (span.Start.Month - 1).ToString() +
    "&b=" + span.Start.Day.ToString() + "&c=" + span.Start.Year.ToString() + "&d=" + (span.End.Month - 1).ToString() + "&e=" + span.End.Day.ToString() +
    "&f=" + span.End.Year.ToString()

let priceUrl ticker span = commonUrl ticker span + "&g=d&ignore=.csv"

let divUrl ticker span = commonUrl ticker span + "&g=v&ignore=.csv"

let splitUrl ticker span page =
    "http://finance.yahoo.com/q/hp?s=" + ticker + "&a=" + (span.Start.Month - 1).ToString() +
    "&b=" + span.Start.Day.ToString() + "&c=" + span.Start.Year.ToString() + "&d=" + (span.End.Month - 1).ToString() + "&e=" + span.End.Day.ToString() +
    "&f=" + span.End.Year.ToString() + "&g=v&z=66&y=" + (66 * page).ToString();

let parsePrice (line: string) =
    let tokens = line.Split([|','|])
    { Date = DateTime.Parse(tokens.[0]);
        Event = Price ({Open = money (Double.Parse(tokens.[1])) ; High = money (Double.Parse(tokens.[2]));
            Low = money (Double.Parse(tokens.[3])); Close = money (Double.Parse(tokens.[4])); Volume = volume (Double.Parse(tokens.[5]))})}        

let parseDiv (line: string) =
    let tokens = line.Split([|','|])
    let date = DateTime.Parse(tokens.[0])
    let amount = money (Double.Parse(tokens.[1]))
    {Date = date; Event = Dividend amount}        

let rec loadFromLineReader (reader:StringReader) listOfThings parseLineFunc =
    match  reader.ReadLine () with
    | null  -> listOfThings
    | line  -> loadFromLineReader reader (parseLineFunc line::listOfThings) parseLineFunc        

let loadFromLineString text listOfThings parseLineFunc =
    let reader = new StringReader(text)
    reader.ReadLine ()|> ignore // skip header
    loadFromLineReader reader listOfThings parseLineFunc

let loadWebStringAsync url =
    async { 
        let req = WebRequest.Create(url: string)
        use! response = req.AsyncGetResponse()
        use reader = new StreamReader(response.GetResponseStream())
        return reader.ReadToEnd()}

let loadFromUrlAsync url parseFunc =
    async { 
        let! text = loadWebStringAsync url
        return loadFromLineString text [] parseFunc}
 
let parseSplits rows =
    let parseRow row =
        if row |> Seq.exists (fun (x:string) -> x.Contains("Stock Split"))
        then
            let dateS = Seq.head row
            let splitS = Seq.nth 1 row
            let date = DateTime.Parse(dateS)
            let regex = Regex.Match(splitS,@"(\d+)\s+:\s+(\d+)\s+Stock Split", RegexOptions.Multiline)
            let newShares = shares (float (regex.Groups.Item(1).Value))
            let oldShares = shares (float (regex.Groups.Item(2).Value))
            Some({Date = date; Event = Split(newShares / oldShares)})
        else None
    rows |> Seq.choose parseRow |> Seq.toList

let containsDivsOrSplits cells =
    cells |> Seq.exists (fun (x:string) -> Regex.IsMatch(x, @"\$.+Dividend", RegexOptions.Multiline) || Regex.IsMatch(x, "Stock Split"))  

let rec loadWebSplitAsync ticker span page splits =
    let parseSplit text splits =
        splits @ (parseSplits (scrapHtmlRows text)), not(containsDivsOrSplits (scrapHtmlCells text))
    async {     
        let url = splitUrl ticker span page
        let! text = loadWebStringAsync url
        let splits, beyondLastPage = parseSplit text splits
        if beyondLastPage then 
            return splits 
        else 
            return! loadWebSplitAsync ticker span (page + 1) splits }

let loadPricesAsync ticker span = loadFromUrlAsync (priceUrl ticker span) parsePrice
let loadDivsAsync ticker span = loadFromUrlAsync (divUrl ticker span) parseDiv
let loadSplitsAsync ticker span = loadWebSplitAsync ticker span 0 []

let loadTickerAsync ticker span =
    async {
        let prices = loadPricesAsync ticker span
        let divs =  loadDivsAsync ticker span
        let splits = loadSplitsAsync ticker span
        let! (prices, divs, splits) = Async.Parallel3 (prices, divs, splits)
        return prices @ divs @ splits  }

let loadTickersAsync tickerSpanTuples =
    Async.Parallel
        [ for (ticker, span)  in  tickerSpanTuples ->
            async { let! obsList = loadTickerAsync ticker span
                    return (ticker, span, obsList) } ]
        
let compareObservations obs1 obs2 =
    if obs1.Date <> obs2.Date then obs2.Date.CompareTo(obs1.Date)
    else
        match obs1.Event, obs2.Event with
            | Price _, Price _ 
            | Dividend _, Dividend _ 
            | Split _, Split _  -> failwith "Two same date/ same kind observations"
            | Price _, _  -> -1
            | _, Price _  -> 1
            | _           -> 0
        
let mapAcc acc newAccF newItemF inl =
    let foldF (acc, l) x = newAccF acc x, (newItemF acc x)::l
    let _, out = inl |> List.fold foldF (acc, [])
    out

let asHappened splitFactor observations = 
    let newSplitFactor splitFactor obs =
        match obs.Event with
            | Split(factor) -> splitFactor * factor
            | _             -> splitFactor
    let newObs splitFactor obs =
        let date = obs.Date
        let event = match obs.Event with
                        | Price(p)                  -> Price(p)
                        | Dividend(amount)          -> Dividend(amount * splitFactor)
                        | Split(factor)             -> Split(factor)
        {Date = date; Event = event}
    observations
    |> List.sortWith compareObservations
    |> mapAcc splitFactor newSplitFactor newObs

let adjusted (splitFactor, lastDiv, oFact, hFact, lFact, cFact, vFact) asHappenedObs =
    let newFactor (splitFactor, lastDiv, oFact, hFact, lFact, cFact, vFact) obs =
        match obs.Event with
            | Split(split)  -> splitFactor * split, lastDiv, oFact, hFact, lFact, cFact, vFact 
            | Dividend(div) -> splitFactor, div, oFact, hFact, lFact, cFact, vFact
            | Price(p)      -> splitFactor, 0.<money>, oFact / (1. - lastDiv / p.Open), hFact / (1. - lastDiv / p.High), lFact / (1. - lastDiv / p.Low),
                                cFact / (1. - lastDiv / p.Close), vFact / (1. - lastDiv / p.Close)
    let newObs (splitFactor, lastDiv, oFact, hFact, lFact, cFact, vFact) obs =
        let date = obs.Date
        let event = match obs.Event with
                        | Price(p)          -> Price({Open = p.Open / splitFactor / oFact; High = p.High / splitFactor / hFact;
                                                      Low = p.Low / splitFactor / lFact; Close = p.Close / splitFactor / cFact; Volume = p.Volume / splitFactor / vFact })
                        | Dividend(amount)  -> Dividend (amount / splitFactor)
                        | Split(split)      -> Split(split)
        {Date = date; Event = event}
    asHappenedObs
        |> List.sortWith compareObservations
        |> mapAcc (splitFactor, lastDiv, oFact, hFact, lFact, cFact, vFact) newFactor newObs
        |> List.filter (fun x -> match x.Event with Split(_) -> false | _ -> true)        
