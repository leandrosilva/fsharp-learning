// F# Twitter Feed Sample using F# Async Programming and Event processing
//

#r "System.Web.dll"
#r "System.Windows.Forms.dll"
#r "System.Xml.dll"
#r "System.Xml.Linq.dll"

#load @"..\AsyncHelpers.fs"
#load "TwitterPassword.fs"


open System
open System.Xml
open System.Globalization
open System.IO
open System.Net
open System.Web
open System.Xml.Linq
open System.Threading
open FSharp.Tutorial
open Microsoft.FSharp.Control.WebExtensions

/// A component which listens to tweets in the background and raises an 
/// event each time a tweet is observed
type TwitterStreamSample() = 
    let ctxt = System.Threading.SynchronizationContext.Current
    let post f = 
        match ctxt with 
        | null -> f()
        | _ -> ctxt.Post((fun _ -> f()), null)
    let event = new Event<_>()   
    let credentials = NetworkCredential(userName, password)
    let streamSampleUrl = "http://stream.twitter.com/1/statuses/sample.xml?delimited=length"
    let listener = 
        async { let req = WebRequest.Create(streamSampleUrl, Credentials=credentials) 
                use! resp = req.AsyncGetResponse()
                use stream = resp.GetResponseStream()
                use reader = new StreamReader(stream)
                while not reader.EndOfStream do 
                    let size = reader.ReadLine() |> int
                    let buffer = Array.zeroCreate size
                    reader.ReadBlock(buffer,0,size)  |> ignore
                    let text = String buffer
                    post (fun () -> event.Trigger text) }

    let mutable group = new CancellationTokenSource()

    member this.StartListening() = Async.Start(listener, group.Token)
    member this.StopListening() = group.Cancel(); group <- new CancellationTokenSource()

    /// Feeds the XML one tweet at a time
    member this.NewTweet = event.Publish


let twitterStream = new TwitterStreamSample()


(*
x.NewTweet 
   |> Event.listen (fun s -> printfn "%A" s)

x.StartListening()
x.StopListening()

x.NewTweet 
   |> Event.map XElement.Parse 
   |> Event.listen (fun s -> printfn "%A" s)

x.StartListening()
x.StopListening()

*)

let xn (s:string) = XName.op_Implicit s

/// The results of the parsed tweet
type UserStatus = 
    { UserName : string; 
      ProfileImage : string; 
      Status : string; 
      StatusDate : DateTime }

/// Attempt to parse a tweet
let parseTweet (xml: string) = 

    let document = XDocument.Parse xml
    
    let node = document.Root 
    if node.Element(xn "user") <> null then
        Some { UserName     = node.Element(xn "user").Element(xn "screen_name").Value; 
               ProfileImage = node.Element(xn "user").Element(xn "profile_image_url").Value; 
               Status       = node.Element(xn "text").Value       |> HttpUtility.HtmlDecode
               StatusDate   = node.Element(xn "created_at").Value |> (fun msg -> DateTime.ParseExact(msg, "ddd MMM dd HH:mm:ss +0000 yyyy", CultureInfo.CurrentCulture)) } 
    else
        None

//x.NewTweet 
//   |> Event.map parseTweet
//   |> Event.listen (fun s -> printfn "%A" s)

let addToMultiMap key x multiMap = 
   let prev = match Map.tryFind key multiMap with None -> [] | Some v -> v 
   Map.add x.UserName (x::prev) multiMap

/// An event which triggers on every 'n' triggers of the input event
let every n (ev:IEvent<_>) = 
   let out = new Event<_>()
   let count = ref 0 
   ev.Add (fun arg -> incr count; if !count % n = 0 then out.Trigger arg)
   out.Publish

twitterStream.NewTweet 
   |> Event.choose parseTweet
   |> Event.scan (fun z x -> addToMultiMap x.UserName x z) Map.empty
   |> every 20
   |> Event.add (fun s -> printfn "#users = %d, avg tweets = %g" s.Count (s |> Seq.averageBy (fun (KeyValue(_,d)) -> float d.Length)))

twitterStream.StartListening()


// Send this to the F# Interactive to stop watching the feed
//x.StopListening()
