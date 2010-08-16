module StockTicker.Persistence

open System
open System.Text
open System.IO
open StockTicker.Common

let appendDate (d:DateTime) (builder: StringBuilder) = builder.Append(d.ToShortDateString())
let append (o:obj) (builder:StringBuilder) = builder.Append(o)
let comma (builder: StringBuilder) = builder.Append(',')
let priceSym (builder: StringBuilder) = builder.Append('P')
let divSym (builder: StringBuilder) = builder.Append('D')
let splitSym (builder: StringBuilder) = builder.Append('S')
let newline (builder: StringBuilder) = builder.Append('\n')

let split (c : char list) (s:string) =
    let ca = Array.ofList c 
    s.Split(ca, System.StringSplitOptions.RemoveEmptyEntries) |> Array.toList

let persistObservation builder (obs: Observation) =
    match (obs.Event) with
        | Price(p)      -> builder  |> priceSym |> comma
                                    |> appendDate obs.Date |> comma
                                    |> append p.Open |> comma
                                    |> append p.High |> comma
                                    |> append p.Low |> comma
                                    |> append p.Close |> comma
                                    |> append p.Volume |> newline
        | Dividend(amount)  -> builder  |> divSym |> comma |> appendDate obs.Date |> comma|> append amount |>newline
        | Split(factor) -> builder  |> splitSym |> comma |> appendDate obs.Date |> comma|> append factor |> newline
 
let persistObservations observations builder =
    observations |> List.fold persistObservation builder
    
let parseObservation (line:string) =
    let parseObs (tokens: string list) =
        let date = DateTime.Parse(tokens.[1])
        match tokens.[0] with
            | "P"   -> {Date = date; Event = Price({Open = money (float tokens.[2]); High = money (float tokens.[3]);
                                                    Low = money(float tokens.[4]); Close = money(float tokens.[5]);
                                                    Volume = volume(float tokens.[6])})}
            | "D"   -> {Date = date; Event = Dividend(money (float tokens.[2]))}
            | "S"   -> {Date = date; Event = Split(float tokens.[2])}
            | _     -> failwith "Shouldn't get this char as first char in the line" 
    line |> split [','] |> parseObs

let parseObservations text =
    text |> split ['\n'] |> List.map parseObservation

let strToBytes (s:string) =
    let  encoding = UnicodeEncoding()
    encoding.GetBytes(s);

let bytesToStr (b:byte[]) =
    let  encoding = UnicodeEncoding()
    encoding.GetString(b)

let saveObservationsToFileAsync fileName observations =
     async {
        use outStream = File.OpenWrite (fileName)
        let observationsText = (persistObservations observations (new StringBuilder())).ToString()
        let bytes = strToBytes observationsText
        do! outStream.AsyncWrite(strToBytes observationsText, 0, bytes.Length)}
  
let saveMultipleObservationsToFileAsync fileNameObservationsTuples =
    Async.Parallel [ for (fName, obsList) in fileNameObservationsTuples -> saveObservationsToFileAsync fName obsList ] 
        
let loadObservationsFromFileAsync fileName =
    async { 
        use inStream = File.OpenRead(fileName)
        let len = inStream.Length
        let! bytes = inStream.AsyncRead(int len)
        let text = bytesToStr bytes
        return parseObservations text }
    
let loadMultipleObservationsFromFileAsync fileNames =
    Async.Parallel [ for fName in fileNames -> loadObservationsFromFileAsync fName ]
