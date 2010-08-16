namespace BackTest

open System
open System.Collections.Generic
open StockTicker.Common
open StockTicker.Loader
open StockTicker.Persistence

type MyPrice(d : DateTime, o, l, h, c, v) =
    let date = d
    let p = {Open = o; Low = l; High = h; Close = c; Volume = v}
    member x.Date = date
    member x.Open = p.Open
    member x.Low = p.Low
    member x.High = p.High
    member x.Close = p.Close
    member x.Volume = p.Volume
    
type MyDividend(d : DateTime, amount: float<money>) =
    let date = d
    let amount = amount
    member d.Date = date
    member d.Amount = amount
    
type MySplit(d: DateTime, splitFactor: float) =
    let date = d
    let splitFactor = splitFactor
    member s.Date = date
    member s.SplitFactor = splitFactor

type TickerData (ticker, startDate: DateTime, endDate: DateTime) =
    let ticker = ticker
    let startDate = startDate
    let endDate = endDate
    let prices = new ResizeArray<MyPrice> ()
    let divs = new ResizeArray<MyDividend> ()
    let splits = new ResizeArray<MySplit> ()
    member internal x.IPrices = prices
    member internal x.IDividends = divs
    member internal x.ISplits = splits
    member x.Ticker = ticker
    member x.StartDate = startDate
    member x.EndDate = endDate
    member x.Prices = prices :> IList<MyPrice>
    member x.Dividends = divs :> IList<MyDividend>
    member x.Splits = splits :> IList<MySplit>

type StatusReporter (reportFunction:Action<float, string>) =
    let fromDelegate f s = reportFunction.Invoke(f, s)
    let reporter = new Reporter(fromDelegate)
    member internal r.Reporter = reporter 

[<ReferenceEquality>]
type TickerWebRequest = { WebTicker : string; StartDate : DateTime; EndDate : DateTime }
[<ReferenceEquality>]
type TickerFileRequest = { FileTicker : string; FileName : string }
[<ReferenceEquality>]
type FileNameTickerData = { FileName: string; TickerData : TickerData }

module internal Utilities =    
    let obsToPrice obs =        
        match obs.Event with
            | Price(p) -> MyPrice(obs.Date, p.Open, p.Low, p.High, p.Close, p.Volume)
            | _        -> failwith "Not a Price"

    let obsToDividend obs =        
        match obs.Event with
            | Dividend(amount) -> MyDividend(obs.Date, amount)
            | _        -> failwith "Not a Dividend"

    let obsToSplit obs =        
        match obs.Event with
            | Split(splitFactor) -> MySplit(obs.Date, splitFactor)
            | _        -> failwith "Not a Split"
    let isPrice o = match o.Event with Price(_) -> true | _ -> false
    let isDiv o = match o.Event with Dividend _ -> true | _ -> false
    let isSplit o = match o.Event with Split _ -> true | _ -> false
    
    let toTickerData ticker span observations =
        let td = new TickerData (ticker, span.Start, span.End)
        td.IPrices.AddRange (
            observations
                |> List.filter isPrice
                |> List.map obsToPrice)
        td.IDividends.AddRange (
            observations
                |> List.filter isDiv
                |> List.map obsToDividend)
        td.ISplits.AddRange (
            observations
                |> List.filter isSplit 
                |> List.map obsToSplit)
        td
        
    let toObservations (tickerData: TickerData) =
        [ for p in tickerData.IPrices do 
              yield { Date = p.Date; Event = Price { Open = p.Open; High = p.High; Low = p.Low; Close = p.Close; Volume = p.Volume } }
          for d in tickerData.IDividends do 
              yield { Date = d.Date; Event = Dividend(d.Amount)}
          for s in tickerData.ISplits do 
              yield { Date = s.Date; Event = Split(s.SplitFactor)} ]

module public Funcs =
    open Utilities
    open StockTicker.Common

    let DownloadPrices (ticker, startDate, endDate) =
        let aSpan = { Start = startDate; End = endDate }
        Async.RunSynchronously( loadPricesAsync ticker aSpan) |> List.map obsToPrice |> List.toSeq
    let DownloadDividends (ticker, startDate, endDate) =
        let aSpan = { Start = startDate; End = endDate }
        Async.RunSynchronously( loadDivsAsync ticker aSpan) |> List.map obsToDividend |> List.toSeq        
    let DownloadSplits (ticker, startDate, endDate) =
        let aSpan = { Start = startDate; End = endDate }
        Async.RunSynchronously( loadSplitsAsync ticker aSpan) |> List.map obsToSplit |> List.toSeq
            
    let DownloadTickerData (ticker, startDate, endDate) =
        let aSpan = { Start = startDate; End = endDate }
        Async.RunSynchronously( loadTickerAsync ticker aSpan) |> toTickerData ticker aSpan 
                
    let DownloadTickersData (tickerRequests) =
        let tickerDatas =
            Async.RunSynchronously(
                tickerRequests
                    |> Seq.map (fun req -> req.WebTicker, { Start = req.StartDate ; End = req.EndDate })
                    |> Seq.toList
                    |> loadTickersAsync)
                |> Array.map (fun (ticker, span, obs) -> toTickerData ticker span obs)
        let td = new ResizeArray<TickerData> ()
        td.AddRange(tickerDatas)
        td :> seq<TickerData>
   
    let LoadTickerData (ticker, fileName) =
        Async.RunSynchronously(fileName |> loadObservationsFromFileAsync) |> toTickerData ticker {Start = DateTime.MinValue; End = DateTime.MaxValue}
        
    let LoadTickersData (tickersFileNames: seq<TickerFileRequest>) =
        let fileNames = tickersFileNames |> Seq.map (fun tfr -> tfr.FileName)
        let observationsList = Async.RunSynchronously(fileNames |> Seq.toList |> loadMultipleObservationsFromFileAsync)
        let tickers = tickersFileNames |> Seq.map (fun tfr -> tfr.FileTicker)
        tickers
            |> Seq.zip observationsList
            |> Seq.map (fun (observations, ticker) -> toTickerData ticker {Start = DateTime.MinValue; End = DateTime.MaxValue} observations)

     
    let SaveTickerData (fileName, tickerData: TickerData) =
        Async.RunSynchronously( tickerData |> toObservations |> saveObservationsToFileAsync fileName)
    
    let SaveTickersData (fileNamesTickerDatas) =
        let asyncs = fileNamesTickerDatas
                        |> Seq.map (fun fntd -> fntd.FileName, fntd.TickerData |> toObservations)
                        |> Seq.toList
                        |> saveMultipleObservationsToFileAsync
        Async.RunSynchronously(asyncs) |> ignore
        
    let MakeAsHappened tickerData =
        tickerData |> toObservations |> asHappened 1.0
            |> toTickerData tickerData.Ticker { Start = tickerData.StartDate; End = tickerData.EndDate}

    let MakeAdjusted asHappenedTickerData =
        asHappenedTickerData |> toObservations |> adjusted adjStart
            |> toTickerData asHappenedTickerData.Ticker { Start = asHappenedTickerData.StartDate; End = asHappenedTickerData.EndDate}
