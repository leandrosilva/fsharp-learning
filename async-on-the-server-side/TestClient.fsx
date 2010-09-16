#r "System.Net"
open System.Net
open System.Net.Sockets

let quoteSize = 512

type System.Net.Sockets.TcpClient with
    member client.AsyncConnect(server, port) = 
        Async.FromBeginEnd(server, port,(client.BeginConnect : IPAddress * int * _ * _ -> _), client.EndConnect)

let clientRequestQuoteStream (clientIndex, server, port:int) =
    let n = ref 120  // quit after 2 minutes, in case something goes wrong and demo machine freezes under load
    async { 
        let client = new System.Net.Sockets.TcpClient()
        do!  client.AsyncConnect(server,port)
        let stream = client.GetStream()
        let! _ = stream.AsyncRead 1 // read header
        while !n > 0 do 
            decr n
            let! bytes = stream.AsyncRead quoteSize
            if Array.sumBy int bytes <> quoteSize then 
                printfn "client incorrect checksum" 
    }

let myLock = new obj()
let mutable anyErrors = false

let clientAsync clientIndex = 
    async { 
        do! Async.Sleep(clientIndex*5)
        if clientIndex % 100 = 0 then
            lock myLock (fun() -> printfn "%d clients..." clientIndex)       
        try 
            do! clientRequestQuoteStream (clientIndex, IPAddress.Loopback, 10003)
        with e -> 
            if not anyErrors then
                anyErrors <- true
                printfn "CLIENT ERROR: %A" e
            else
                printf "err..."
            raise e
    }
   
Async.Parallel [ for i in 1 .. 1000 -> clientAsync i ] 
    |> Async.Ignore 
    |> Async.Start 
