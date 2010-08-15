open System
open System.Net
open System.IO

open Microsoft.FSharp.Control.WebExtensions

// (1) "async { ... }" is used to write tasks which include some asynchronous I/O.
//
// (2) These are composed in parallel using the fork-join combinator Async.Parallel
//
// (3) In this case, the composition is executed using Async.RunSynchronously, which
//     synchronously waits for the overall result
//
// (4) Using let! (or its resource-disposing equivalent use!) is one basic way of composing
//     asyncs. A line such as
//
//         let! resp = req.AsyncGetResponse()
//
//     causes a "reaction" to occur when a response to the HTTP GET occurs. That is,
//     the rest of the async { ... } runs when the AsyncGetResponse operation completes.
//     However, no .NET or operating system thread is blocked while waiting for this
//     reaction: only active CPU computations use an underlying .NET or O/S thread. In
//     contrast, pending reactions (for example, callbacks, event handlers and agents)
//     are relatively cheap, often as cheap as a single registered object. As a result
//     you can have thousands or even millions of pending reactions. For example, a typical
//     GUI application has many registered event handlers, and a typical web crawler has a
//     registered handler for each outstanding web request.
//
//     In the above, "use!" replaces "let!" and indicates that the resource associated with
//     the web request should be disposed at the end of the lexical scope of the variable.
let http url =
    async { let req =  WebRequest.Create(Uri url)
            use! resp = req.AsyncGetResponse()
            use stream = resp.GetResponseStream()
            use reader = new StreamReader(stream)
            let contents = reader.ReadToEnd()
            return contents }
 
let sites = ["http://www.bing.com";
             "http://www.google.com";
             "http://www.yahoo.com";
             "http://www.search.com"]
 
let htmlOfSites =
    Async.Parallel [for site in sites -> http site ]
    |> Async.RunSynchronously

printfn "htmlOfSites = %A" htmlOfSites