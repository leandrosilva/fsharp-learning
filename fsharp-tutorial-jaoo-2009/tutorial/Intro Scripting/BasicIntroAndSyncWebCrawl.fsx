//---------------------------------------------------------------------------
// This is a demonstration script for showing some simple uses of F# to generate,
// acquire and display data from a variety of sources.
//
// It is useful for orienting an audience towards the use of F# as a data-oriented 
// general purpose programming language with an orientation towards functional
// programming.

open System
open System.IO
open System.Net
open System.Windows.Forms
open System.Drawing
open System.Xml
open System.Collections.Generic


//---------------------------------------------------------------------------
// Part 0. Warmup

System.Console.WriteLine "Hello World"

printfn "Hello again!"




//---------------------------------------------------------------------------
// Part I. Web requests
//
// This part of the tutorial script shows how to acquire the contents of web pages.
// Later elements of the tutorial show how to do this asynchronously.

// ok, now fetch a page.  Create the web request, wait for the response, read off the response.
let req    = WebRequest.Create "http://www.yahoo.com"
let stream = req.GetResponse().GetResponseStream()
let reader = new StreamReader(stream)
let html   = reader.ReadToEnd()


html.Length

/// Fetch the contents of a web page
let http (url: string) = 
    let req    = System.Net.WebRequest.Create url 
    use resp   = req.GetResponse()         // note 'use' = C# 'using'
    use stream = resp.GetResponseStream() 
    use reader = new StreamReader(stream) 
    let html   = reader.ReadToEnd()
    html

/// Fetch a few particular web pages
let bing   = http "http://www.bing.com"
let google = http "http://www.google.com"
let smh    = http "http://www.smh.com.au"
let nytRSS = http "http://www.nytimes.com/services/xml/rss/nyt/HomePage.xml"


// ----------------------------
// Windows Forms
//
// This part of the tutorial script shows how to display data using windows forms.


/// This is a form used to hold displayed data
let form = new Form(Visible = true, Text = "A Simple F# Form", 
                    TopMost = true, Size = Size(600,600))

/// This is a text box used to hold displayed data
let textBox = 
    new RichTextBox(Dock = DockStyle.Fill, Text = "F# Programming is Fun!",
                    Font = new Font("Lucida Console",16.0f,FontStyle.Bold),
                    ForeColor = Color.DarkBlue)


form.Controls.Add textBox


/// This function formats an arbitrary value as test and displays it in the text box of the form
let show x = 
   textBox.Text <- sprintf "%30A" x


// OK, let's show the data to the form...

show (1,2)
show [ 0 .. 100 ]
show [ 0.0 .. 2.0 .. 100.0 ]

// OK, let's pipeline the data to the form...

//let (|>) x f = f x

//let (|>) someObject f = f someObject

(1,2,3) |> show

[ 0 .. 99 ] |> show



[ for i in 0 .. 99 -> (i, i*i) ] |> show

[ for i in 0 .. 99 do
       yield (i, i*i) ] |> show

[ for i in 0 .. 99 do
    for j in 0 .. 99 do
       if i = j then 
          yield (i, j, i*j) ] |> show

nytRSS |> show



// ----------------------------
// Scan RSS for news titles


/// We load XML fragments into this document
let xdoc = new XmlDocument()

xdoc.LoadXml(nytRSS)

[ for node in xdoc.SelectNodes("//title") do 
       yield node.InnerText ] 
    
    |> show

[ for node in xdoc.SelectNodes("//title") do 
      if node.InnerText.Contains "Obama" then 
          yield node.InnerText ]       
  |> show


// ----------------------------
// Search for URLs in HTML

open System.Text.RegularExpressions

/// Collect the links from a URL
let collectLinks url = 
    let html = try http url with _ -> ""
    [ for m in Regex.Matches(html,"href=\s*\"[^\"h]*(http://[^&\"]*)\"")  -> m.Groups.[1].Value ]

collectLinks "http://www.wired.com" |> show
collectLinks "http://news.live.com" |> show
collectLinks "http://www.smh.com.au" |> show



// ----------------------------
// Crawling (Synchronous)

/// Performs a depth-first crawl using 'visitUrl' at each step. Stops after
// 'limit' nodes have been processed.
let depthFirstCrawl visitUrl limit url = 
    let rec searchNode (visited: Map<'Url, 'Url list>)  url = 
        // Are we done?
        if visited.Count >= limit || visited.ContainsKey url then 
            // Return the accumulated set of visited links
            visited
        else 
            // Visit the URL in some way...
            let links = visitUrl visited url 
            // Add the URL to the visited set...
            let visited = visited.Add(url, links)
            // Depth first search...
            Seq.fold searchNode visited links
    searchNode Map.empty url

let limit = 12


/// A synchronous web crawler. Do a depth-first search of the WWW from a particular URL.
///   visited: the set of URLs visited so far
///   url: the URL to visit
let crawlAndPrint url =
    depthFirstCrawl 
        (fun visited url -> 
            // Show the URL...
            textBox.AppendText ("crawling " + url + "\n")
            // DoEvents() needed for a synchronous crawl when running in the GUI thread
            Application.DoEvents(); 
            // Get the new links...
            collectLinks url)
        limit
        url
    
    
textBox.Clear()

crawlAndPrint "http://news.google.com"




// ------------------------------------------
/// A sample showing how to feed the web crawl into a GUI display

module SyncGridWebCrawl = 
    let form = new Form(Visible = true, Text = "A Simple F# Form", 
                        TopMost = true, Size = Size(600,600))

    let data = new DataGridView(Dock = DockStyle.Fill, Text = "F# Programming is Fun!",
                                Font = new Font("Lucida Console",12.0f),
                                ForeColor = Color.DarkBlue)


    form.Controls.Add(data)

    data.DataSource <- [| ("http://www.bing.com",0) |]
    data.Columns.[0].Width <- 400
    
    let limit = 30

    /// A synchronous web crawler, with output to grid
    let crawlAndShow url =
        depthFirstCrawl 
            (fun visited url -> 
                  // show the URL...
                  data.DataSource <- 
                      [| for KeyValue(key,value) in visited -> (key,value.Length) |]
                  Application.DoEvents(); 

                  // get the new links...
                  collectLinks url)
            limit
            url
        
    crawlAndShow "http://news.google.com"
    
