#light

(*
  Sample code from the article "Asynchronous C# and F# (I.): Simultaneous introduction",
  avaliable at:
    
  http://tomasp.net/blog/csharp-fsharp-async-intro.aspx
*)

open System
open System.Net
open System.IO
open System.Text.RegularExpressions

let regTitle = new Regex(@"\<title\>([^\<]+)\</title\>")

(* Asynchronously downloads a web page and returns
   title of the page and size in bytes *)
let downloadPage(url:string) = async {
  let request = HttpWebRequest.Create(url)
  (* Asynchronously get response and dispose it when we're done *)
  use! response = request.AsyncGetResponse()
  use stream = response.GetResponseStream()
  let temp = new MemoryStream()
  let buffer = Array.zeroCreate 4096

  (* Loop that downloads page into a buffer (could use 'while' 
     but recursion is more typical for functional language) *)
  let rec download() = async { 
    let! count = stream.AsyncRead(buffer, 0, buffer.Length)
    do! temp.AsyncWrite(buffer, 0, count)
    if count > 0 then return! download() }
  
  (* Start the download asynchronously and handle results *)
  do! download()
  temp.Seek(0L, SeekOrigin.Begin) |> ignore
  let html = (new StreamReader(temp)).ReadToEnd()
  return regTitle.Match(html).Groups.[1].Value, html.Length }
  
(* Downloads pages in parallel and prints all results *)
let comparePages = async {
  let! results = 
    [| "http://www.google.com"; "http://www.bing.com"
       "http://www.yahoo.com" |]
    |> Array.map downloadPage
    |> Async.Parallel
  for title, length in results do
    Console.WriteLine("{0} (length {1})", title, length) }

(* Start the computation on current thread *)
do comparePages |> Async.RunSynchronously