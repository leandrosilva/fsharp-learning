
#load "common.fs"
#load "html.fs"
#load "loader.fs"
#load "persistence.fs"
#load "algorithms.fs"
#load "dotNetWrapper.fs"
#load @"..\..\show.fs"

let run x = x |> Async.RunSynchronously



open StockTicker.Common
open StockTicker.Loader
open StockTicker.Algorithms
open StockTicker.Persistence

let date s = System.DateTime.Parse s


let data = 
    [ for ticker in [ "ubs"; "msft" ] ->  
          ticker, {Start = date "13/11/2008" ; End = date "13/11/2009"} ]
       |> loadTickersAsync
       |> run

data |> show


[ for ticker in [ "ubs"; "msft" ] ->  
      ticker, {Start = date "13/11/2008" ; End = date "13/11/2009"} ]
   |> calculateYearlyReturnsAsync 
   |> Async.RunSynchronously 

