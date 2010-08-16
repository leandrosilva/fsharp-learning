//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of doing parallel
// web crawling using a background worker component encapsulating an F#
// agent.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

open Microsoft.FSharp.Control.WebExtensions

#load "..\show.fs"

let run x = async { do! Async.SwitchToThreadPool();  
                    return! x } |> Async.RunSynchronously



//----------------------------------------------------------------------------
// First some async warmup....


let prog1 = 
   async { return 1 + 1 }

prog1 |> run  |> show

let prog2 = 
    async { printfn "hello world"
            return 300 + 400 }

prog2 |> run |> show

let prog12 = 
   Async.Parallel [ prog1; prog2 ] 

prog12 |> run |> show












let makeJob x = async { return 11 * x }

Async.Parallel [ makeJob 11; makeJob 12 ] |> run |> show

let jobs = 
    [ for i in 0 .. 100 -> makeJob i ]

Async.Parallel jobs |> run |> show











//----------------------------------------------------------------------------
// Now some async web requests....

open System.IO
open System.Net

/// Fetch the contents of a web page, synchronously
let http (url: string) = 
    let req = WebRequest.Create(url) 
    use resp = req.GetResponse()
    use stream = resp.GetResponseStream() 
    use reader = new StreamReader(stream) 
    reader.ReadToEnd()

http "http://www.google.com" 
   |> String.length 
   |> show






/// Fetch the contents of a web page, asynchronously
let httpAsync (url:string) = 
    async { let req = WebRequest.Create(url)             
            let! resp = req.AsyncGetResponse()
            // rest is a callback
            use stream = resp.GetResponseStream() 
            use reader = new StreamReader(stream) 
            let text = reader.ReadToEnd() 
            return text }













httpAsync "http://www.bing.com"
   |> run 
   |> show


Async.Parallel [ httpAsync "http://www.bing.com"; 
                 httpAsync "http://www.google.com" ] 
   |> run                     
   |> show


Async.Parallel [ httpAsync "http://www.yahoo.com"; 
                 httpAsync "http://www.bing.com"; 
                 httpAsync "http://www.google.com" ] 
   |> run 
   |> Array.map (fun html -> html.Length)  
   |> show

let urls = 
    [ "http://www.bing.com"; 
      "http://www.yahoo.com"; 
      "http://news.yahoo.com"; 
      "http://www.google.com"; 
      "http://news.google.com"; ] 


Async.Parallel [ for url in urls -> httpAsync url ]
  |> run
  |> Array.map (fun html -> float html.Length)
  |> Array.average                           
  |> show



//----------------------------------------------------------------------------
// In this example, we run an agent in the background which manages
// the parallel web requests and notifies us whenever the overall
// result set has changed.

open System.Windows.Forms
open System.Collections.Generic
open System.Drawing
open System.Text.RegularExpressions


#load "ParallelWorkerAgent.fs"

open Demo.ParallelWorkers


module CrawlInBackground = 
    let linkPat = "href=\s*\"[^\"h]*(http://[^&\"]*)\""
    let getLinksAsync (url:string) =  
       async {
           let! html = httpAsync url 
           let links = [ for m in Regex.Matches(html,linkPat)  -> m.Groups.[1].Value ]
           return (links,links.Length)
       }

    
    let crawler = 
        new BackgroundParallelCrawl<string,int>
                             (limit=35,
                              visitor = (fun url -> 
                                          getLinksAsync url))

    
    
            
    let form = new Form(Visible = true, Text = "A Simple F# Form", 
                        TopMost = true, Size = Size(600,900))

    let data = new DataGridView(Dock = DockStyle.Fill, Text = "F# Programming is Fun!",
                                Font = new Font("Lucida Console",12.0f),
                                ForeColor = Color.DarkBlue)

    form.Controls.Add(data)

    type Row = { URL: string; Links: string }

    data.DataSource <- [| {URL="";Links="?"} |]
    data.Columns.[0].Width <- 400    
    data.Columns.[1].Width <- 200    


    
    
    // Add an event handler to update the GUI whenever the result set changes
    crawler.CrawlSetChanged.Add(fun visited -> 
        data.DataSource <- [| for KeyValue(url,links) in visited -> 
                                  let count = match links with None -> "<requesting>" | Some(links) -> links.Length.ToString()
                                  { URL=url; Links=count} |])

    crawler.Start("http://news.google.com")
    crawler.Start("http://www.google.com")
    crawler.Start("http://www.bing.com")
    crawler.Start("http://www.yahoo.com")
    crawler.Start("http://news.yahoo.com")
    crawler.Start("http://www.smh.com.au")


