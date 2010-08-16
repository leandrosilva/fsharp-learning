// FSharp Bing Translator Sample using FSharp Async Programming
//

#r "System.Drawing.dll"
#r "System.Windows.Forms.dll"

#load @"..\AsyncHelpers.fs"  
#load @"..\show.fs"

let run x = async { do! Async.SwitchToThreadPool();  
                    return! x } |> Async.RunSynchronously

let runGui b = 
  let result = ref None
  let handle = new System.Threading.AutoResetEvent(false)
  Async.StartWithContinuations(b, (fun res -> result := Some (Choice1Of3 res); handle.Set() |> ignore),
                                  (fun res -> result := Some (Choice2Of3 res); handle.Set() |> ignore),
                                  (fun res -> result := Some (Choice3Of3 res); handle.Set() |> ignore))
  while not (handle.WaitOne(0)) do
      System.Windows.Forms.Application.DoEvents()
  match !result with 
  | None -> failwith "unexpected"
  | Some (Choice1Of3 res) -> res
  | Some (Choice2Of3 exn) -> raise exn
  | Some (Choice3Of3 exn) -> raise exn
  
  
let client = new System.Net.WebClient()
let b = 
 async { do client.DownloadStringAsync(System.Uri("http://www.google.com")) 
         let! b = Async.AwaitEvent(client.DownloadStringCompleted) 
         return b.Result  } 
 |> runGui     


open System
open System.IO
open System.Net
open System.Text
open System.Windows.Forms
open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.WebExtensions
open FSharp.Tutorial


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


/// Recall: Fetch the contents of a web page, synchronously
let http (url: string) = 
    let req = WebRequest.Create url
    use resp = req.GetResponse()
    use stream = resp.GetResponseStream() 
    use reader = new StreamReader(stream) 
    let text = reader.ReadToEnd()
    text

/// Warmup: Fetch the contents of a web page, asynchronously
let httpAsync (url:string) = 
    async { let req = WebRequest.Create url             
            use! resp = req.AsyncGetResponse()
            // rest is a callback
            use stream = resp.GetResponseStream() 
            use reader = new StreamReader(stream) 
            let text = reader.ReadToEnd() 
            return text }

let urls = 
    [ "http://www.bing.com"; 
      "http://www.yahoo.com"; 
      "http://news.yahoo.com"; 
      "http://www.google.com"; 
      "http://news.google.com"; 
      "http://www.twitter.com"; 
      "http://www.facebook.com"; 
      "http://www.wikipedia.org"] 

#time "on"

[ for url in urls -> http url ] 

Async.Parallel [ for url in urls -> httpAsync url ]
  |> run
  
[ for url in urls -> http url ] |> ignore

Async.Parallel [ for url in urls -> httpAsync url ]
  |> run
  |> ignore


Async.Parallel [ for url in urls ->
                      async { let! html = httpAsync url
                              return (url, html.Length) } ]
  |> run
  |> show



#load @"..\..\BingAppId.fs"





let detectUri       = "http://api.microsofttranslator.com/V1/Http.svc/Detect?appId=" + myAppId
let translateUri    = "http://api.microsofttranslator.com/V1/Http.svc/Translate?appId=" + myAppId + "&"
let languageUri     = "http://api.microsofttranslator.com/V1/Http.svc/GetLanguages?appId=" + myAppId
let languageNameUri = "http://api.microsofttranslator.com/V1/Http.svc/GetLanguageNames?appId=" + myAppId


// Like the 'httpAsync' functions, but read the lines, not just the text
let httpLinesAsync (uri:string) =
    async { let! text = httpAsync uri
            return text.Split([| '\n'; '\r' |], System.StringSplitOptions.RemoveEmptyEntries) }

// OK, let's get the list of supported langauges
httpLinesAsync languageUri |> run |> show
httpAsync languageUri |> run |> show

let languages =  httpLinesAsync languageUri |> run 




let detectLanguageAsync text = async { 
    let request = WebRequest.Create (detectUri, Method="Post", ContentType="text/plain")
    do! request.AsyncWriteContent text
    return! request.AsyncReadResponse() 
  }


detectLanguageAsync "Welcome to the FSharp Tutorial at UBS!" |> run |> show


// Next, let's play around with the translation API
let translateAsync (text, fromLanguage, toLanguage) = 
    async {
      try 
        let uri = sprintf "%sfrom=%s&to=%s" translateUri fromLanguage toLanguage
        let request2 = WebRequest.Create (uri, Method="Post", ContentType="text/plain")
        do! request2.AsyncWriteContent text
        let! res = request2.AsyncReadResponse()
        return res
      with _ -> 
        return sprintf "Couldn't translate into %s" toLanguage
    }


translateAsync ("Asynchronous programming is fun!", "en", "de") 
    |> run 
    |> show

// This wraps up the calls to the detection and translation APIs
let translateTextAsync (text, toLanguage) =
    async { let! fromLanguage = detectLanguageAsync text 
            let! translated = translateAsync (text, fromLanguage, toLanguage)
            return (fromLanguage, toLanguage, text, translated) }
          

let text = "Hello ! Are you ready to learn FSharp?"

detectLanguageAsync text |> run |> show
translateAsync (text, "en", "nl") |> run |> show

Async.Parallel [ for lang in languages -> translateAsync (text, "en", lang) ] 
   |> run 
   |> show
      

let form       = new Form (Visible=true, TopMost=true, Height=500, Width=600)
let textBox    = new TextBox (Width=450, Text=text)
let button     = new Button (Text="Translate", Left = 460)
let translated = new TextBox (Width = 590, Height = 400, Top = 50, ScrollBars = ScrollBars.Both, Multiline = true)

form.Controls.Add textBox
form.Controls.Add button
form.Controls.Add translated
textBox.Font  <- new System.Drawing.Font("Consolas", 14.0F)
translated.Font <- new System.Drawing.Font("Consolas", 14.0F)

button.Click.Add(fun args ->

    translated.Text <- "Translating..."

    Async.StartWithContinuations(

        // async program:
        Async.Parallel [for lang in languages -> 
                            translateTextAsync (text, lang)] ,

        // success:
        (fun results -> 
            for (fromLanguage, toLanguage, text, translatedValue) in results do
                translated.Text <- translated.Text + sprintf "\r\n%s --> %s: \"%s\"" fromLanguage toLanguage translatedValue),


        // failure:
        (fun exn -> MessageBox.Show(sprintf "An error occurred: %A" exn) |> ignore),
        // cancellation:
        (fun cxn -> MessageBox.Show(sprintf "A cancellation error ocurred: %A" cxn) |> ignore)))

