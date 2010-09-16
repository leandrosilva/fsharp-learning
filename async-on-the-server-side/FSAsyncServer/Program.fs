open System
open System.Net
open System.Net.Sockets
open System.Threading

let quote = Array.init<byte> 512 (fun _ -> 1uy)

let mutable numWritten = 0

let asyncWriteStockQuote(stream:System.IO.Stream) = async {
    do! Async.Sleep 1000 // Mock an I/O wait for 1s for the next quote
    do! stream.AsyncWrite(quote, 0, quote.Length) 
    Interlocked.Increment(&numWritten) |> ignore }

let asyncServiceClient (client: TcpClient) = async {
    use stream = client.GetStream()
    do! stream.AsyncWrite(quote, 0, 1)  // write header
    while true do
        do! asyncWriteStockQuote(stream) }

let mutable anyErrors = false
let mutable requestCount = 0
let AsyncMain() =
    let socket = new TcpListener(IPAddress.Loopback, 10003)
    socket.Start()
    let t = new Thread(ThreadStart(fun _ -> 
        while true do
            let client = socket.AcceptTcpClient()
            requestCount <- requestCount + 1
            if requestCount % 100 = 0 then 
                Console.WriteLine("{0} accepted...", requestCount)
            Async.Start (async { 
                try 
                    use _holder = client
                    do! asyncServiceClient client 
                with e -> 
                    if not(anyErrors) then
                        anyErrors <- true
                        Console.WriteLine("server ERROR")
                    raise e
                } )
    ), IsBackground = true)
    t.Start()
    while true do
        do Thread.Sleep 1000
        let count = Interlocked.Exchange(&numWritten, 0)
        Console.WriteLine("QUOTES PER SECOND: {0}", count)

AsyncMain()