#light
open System
open System.Collections.Generic
open System.Text
open System.IO
open System.IO.IsolatedStorage
open Xunit
open Common
open Loader
open Algorithms
open Persistence

let now = DateTime.Now

let loadAdjustedPrice ticker (date:DateTime) =
    let parsePrice (line: string) =
        let tokens = line.Split([|','|])
        money (Double.Parse(tokens.[6]))
    Async.Run(loadFromUrlAsync (priceUrl ticker (span date.Year date.Month date.Day date.Year date.Month date.Day)) parsePrice)
        |> List.first someIdem |> Option.get

let tickers =
    let dict = new Dictionary<string, Observation list>()
    let tickersToLoad = [("ko", span 1986 3 13 now.Year now.Month now.Day); ("msft", span 1999 1 1 now.Year now.Month now.Day)]
    let results = Async.Run(loadTickersAsync tickersToLoad)
    results |> Array.iter (fun (ticker,_,  observations) -> dict.[ticker] <- observations)
    (fun ticker -> dict.[ticker])
let ticker = tickers

let floatEqual ((actual:float<_>), (expected:float<_>)) =
    let precision = 0.00000001<_>
    Assert.InRange(actual, expected - precision, expected + precision)
    
[<Fact>]
let I_can_download_prices_divs_splits () =
    let price = ticker("ko") |> getClose (date 1986 5 12)
    Assert.InRange(price, 108.99<money>, 109.1<money>)
    let price = ticker("msft") |> getClose (date 2008 5 30)
    Assert.InRange(price, 28.3199<money>, 28.321<money>)
    
[<Fact>]
let I_can_convert_prices_divs_splits_to_their_original_form () =
    let div = ticker("ko") |> asHappened 1. |> getDiv (date 1992 3 9)
    floatEqual(div, 4.0 * 0.07<money>)
    
[<Fact>]
let I_can_calculate_adjusted_prices () =
    let adj =
        ticker("ko")
        |> asHappened 1.
        |> adjusted adjStart
        |> getClose (date 1986 3 13)
    Assert.Equal(loadAdjustedPrice "ko" (date 1986 3 13), money (Math.Round(float adj, 2)))
    let adj = ticker("msft") |> asHappened 1. |> adjusted adjStart |> getClose (date 1999 4 1)
    Assert.Equal(loadAdjustedPrice "msft" (date 1999 4 1), money (Math.Round(float adj, 2)))

[<Fact>]
let I_can_calculate_compound_yearly_return () =
    let ret = compoundYearlyReturn (now.AddDays(-365.), 10.<money>) (now, 20.<money>)
    floatEqual(rateOfReturn (20./10. - 1.), ret)

[<Fact>]
let I_can_persist_and_parse_a_ticker () =
    let asH = ticker("ko") |> List.filter (fun obs -> obs.Date >= date 2006 1 1 && obs.Date <= date 2007 3 3) |> asHappened 1.
    let b = (new StringBuilder()) |> persistObservations asH
    let asH1 = b.ToString() |> parseObservations
    Assert.Equal(asH, asH1)
    Assert.NotEqual(asH, asH1 |> List.tl)
    
[<Fact>]
let I_can_save_and_load_observations_from_a_file () =
    let asH = ticker("ko") |> List.filter (fun obs -> obs.Date >= date 2006 1 1 && obs.Date <= date 2007 3 3) |> asHappened 1.
    let is = IsolatedStorageFile.GetUserStoreForAssembly ()
    let isfs = new IsolatedStorageFileStream ("ko.dat", FileMode.Create, FileAccess.Write, is)
    Async.Run(saveObservationsToStreamAsync isfs asH)
    isfs.Flush ()
    isfs.Close ()
    let isfs = new IsolatedStorageFileStream ("ko.dat", FileMode.Open, FileAccess.Read, is)
    let asH1 = Async.Run(loadObservationsFromStreamAsync isfs)
    isfs.Close ()
    is.DeleteFile("ko.dat")
    Assert.Equal(asH, asH1)

[<Fact>]
let I_can_download_prices_through_wrapper () =
    let prices = BackTest.Funcs.DownloadPrices("msft", date 2007 3 3, date 2007 3 30)
    Assert.NotEmpty(prices)

       