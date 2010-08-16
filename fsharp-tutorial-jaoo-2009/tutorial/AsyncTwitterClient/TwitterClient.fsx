// F# Twitter Client Sample using F# Asynchronous Programming
//

#r "System.Web.dll"
#r "System.Windows.Forms.dll"
#r "System.Xml.dll"
#r "System.Xml.Linq.dll"


#load @"..\AsyncHelpers.fs"
#load @"TwitterPassword.fs"

open System
open System.Xml
open System.Globalization
open System.IO
open System.Net
open System.Xml.Linq
open System.Web
open System.Windows.Forms
open FSharp.Tutorial


let timelineUrl = "http://twitter.com/statuses/friends_timeline.xml"
let statusUrl = "http://twitter.com/statuses/update.xml"

// OK, let's read the user's timeline...
let readTimeLineAsync () = 
    async { let credentials = NetworkCredential (userName, password)
            let request = WebRequest.Create (timelineUrl, Credentials=credentials)
            let xml = request.ReadResponse ()
            return xml }

// Test it out....
let xml = readTimeLineAsync () |> Async.RunSynchronously

#load @"..\show.fs"
// Let's look at the XML a little...
show xml

// OK, we're ready to parse the XML. This is a type to hold the data extracted 
// per user from the twitter service.
type UserStatus = 
    { UserName : string; 
      ProfileImage : string; 
      Status : string; 
      StatusDate : DateTime }

// A helper for parsing XML
let xn (s:string) = XName.op_Implicit s

// This parses the xml...
let parseTweetXml (xml: string) = 

    let document = XDocument.Parse xml
    
    [ for node in document.Root.Descendants() do
         if node.Element(xn "user") <> null then
             yield { UserName     = node.Element(xn "user").Element(xn "screen_name").Value; 
                     ProfileImage = node.Element(xn "user").Element(xn "profile_image_url").Value; 
                     Status       = node.Element(xn "text").Value       |> HttpUtility.HtmlDecode
                     StatusDate   = node.Element(xn "created_at").Value |> (fun msg -> DateTime.ParseExact(msg, "ddd MMM dd HH:mm:ss +0000 yyyy", CultureInfo.CurrentCulture)) } ]

// Test it out...
parseTweetXml xml |> show

// Let's put this together and make an asynchronous version to get the user's time line
let getUserTimeLine () =
    async { let credentials = NetworkCredential (userName, password)
            let request = WebRequest.Create (timelineUrl, Credentials=credentials)
            let! xml = request.AsyncReadResponse()
            return parseTweetXml xml }

// Let's test it out...
getUserTimeLine () |> Async.RunSynchronously  |> show
getUserTimeLine () |> Async.RunSynchronously  |> List.map  (fun x -> x.Status) |> show
getUserTimeLine () |> Async.RunSynchronously |> List.length |> show

// Likewise, we can submit a status tweet...
let submitStatusTweet (tweet:string) =
    async { let credentials = NetworkCredential (userName, password)

            let request = WebRequest.Create (statusUrl, Credentials=credentials, Method="POST")
            let httpRequest = (request :?> HttpWebRequest)
                                 
            httpRequest.ServicePoint.Expect100Continue <- false
            
            do! request.AsyncWriteContent (sprintf "status=%s" (HttpUtility.UrlEncode tweet))
            
            let! xml = request.AsyncReadResponse ()

            let document = XDocument.Parse xml

            return document.Root.Element (xn"id") }

// Test it out... 
submitStatusTweet "OK, I'm at JAOO 2009, finishing up the F# Tutorial tomorrow" |> Async.RunSynchronously |> show

// See if the timeline now includes the tweet?
getUserTimeLine () 
  |> Async.RunSynchronously 
  |> List.length 
  |> show

// Create winform to hold our data
let form = new Form(Visible=true, TopMost = true, Height=450, Width = 700)

let txtTweet = new TextBox(Width=600, Height=30, 
                            Font = new System.Drawing.Font("Consolas", 14.0F))

let txtStatus = new TextBox(Multiline = true, 
                            Width=600, Height=400, Top=32,
                            Font = new System.Drawing.Font("Consolas", 14.0F),
                            ScrollBars=ScrollBars.Both)

let btnRefresh = new Button(Text="Refresh", Left=610, Top=10)
let btnTweet = new Button(Text="Tweet", Left=610, Top=40)

form.Controls.Add txtTweet
form.Controls.Add txtStatus
form.Controls.Add btnRefresh
form.Controls.Add btnTweet

// Add handler for async reload
btnRefresh.Click.Add (fun args ->
      txtStatus.Text <- "loading..."
      Async.StartWithContinuations(
        getUserTimeLine (),
        (fun items -> 
            txtStatus.Text <- ""
            for item in List.rev items do 
                txtStatus.Text <- sprintf "%A %s - %s\r\n\r\n%s " item.StatusDate item.UserName item.Status txtStatus.Text),
        (fun exn -> MessageBox.Show(sprintf "An error occurred: %A" exn) |> ignore),
        (fun cxn -> MessageBox.Show(sprintf "A cancellation error ocurred: %A" cxn) |> ignore)))
        
// Add handler for async send
btnTweet.Click.Add (fun args ->
      txtStatus.Text <- ""
      Async.StartWithContinuations(
        submitStatusTweet ("Tweet! " + txtTweet.Text + " (at " + string System.DateTime.Now + ")"),
        (fun res -> txtStatus.Text <- sprintf "Tweet ID: %A" res),
        (fun exn -> MessageBox.Show(sprintf "An error occurred: %A" exn) |> ignore),
        (fun cxn -> MessageBox.Show(sprintf "A cancellation error ocurred: %A" cxn) |> ignore)))


