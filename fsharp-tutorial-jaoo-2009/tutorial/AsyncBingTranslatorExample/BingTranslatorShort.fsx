// F# Bing Translator Sample using F# Async Programming
//

#r "System.Drawing.dll"
#r "System.Windows.Forms.dll"

#load @"..\AsyncHelpers.fs"  
#load @"..\show.fs"

open System.IO
open System.Net
open System.Windows.Forms
open Microsoft.FSharp.Control.WebExtensions
open FSharp.Tutorial

let myAppId : string = 
    System.Windows.Forms.MessageBox.Show "please set your Bing AppId in the source file" |> ignore
    failwith "please set your Bing AppId here"

let detectUri       = "http://api.microsofttranslator.com/V1/Http.svc/Detect?appId=" + myAppId
let translateUri    = "http://api.microsofttranslator.com/V1/Http.svc/Translate?appId=" + myAppId + "&"
let languageUri     = "http://api.microsofttranslator.com/V1/Http.svc/GetLanguages?appId=" + myAppId
let languageNameUri = "http://api.microsofttranslator.com/V1/Http.svc/GetLanguageNames?appId=" + myAppId

let run x = Async.RunSynchronously x

// Like the 'httpAsync' functions, but read the lines, not just the text
let httpLines (uri:string) =
  async { 
    let request = WebRequest.Create uri 
    use! response = request.AsyncGetResponse()          
    use stream = response.GetResponseStream()
    use reader = new StreamReader(stream)
    let! lines = reader.AsyncReadAllLines() 
    return lines 
  }

// Find out which languages are supported
httpLines languageUri |> run |> show

let languages =  httpLines languageUri |> run 


// Now, an asynchronous version of the same
let detectLanguage text = 
  async { 
    let request = WebRequest.Create (detectUri, Method="Post", ContentType="text/plain")
    do! request.AsyncWriteContent text
    return! request.AsyncReadResponse() 
  }

detectLanguage "Welcome to F# at JAOO!" |> run |> show

// Next, let's play around with the translation API
let translateText (text, fromLang, toLang) = 
  async {
    let uri = sprintf "%sfrom=%s&to=%s" translateUri fromLang toLang
    let request2 = WebRequest.Create (uri, Method="Post", ContentType="text/plain")
    do! request2.AsyncWriteContent text
    return! request2.AsyncReadResponse()
  }

// This wraps up the calls to the detection and translation APIs
let detectAndTranslate (text, toLang) = 
  async { 
    let! fromLang = detectLanguage text 
    let! translated = translateText (text, fromLang, toLang)
    return (fromLang, toLang, text, translated) 
  }
          

let text = "Hello JAOO, are you ready to learn F#?"

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

    let task = 
        Async.Parallel 
           [for lang in languages -> 
               detectAndTranslate (text, lang)]

    Async.StartWithContinuations(  
        task,
        (fun results -> 
            for (fromLang, toLang, text, translatedValue) in results do
                translated.Text <- translated.Text + sprintf "\r\n%s --> %s: \"%s\"" fromLang toLang translatedValue),
        (fun exn -> MessageBox.Show(sprintf "An error occurred: %A" exn) |> ignore),
        (fun cxn -> MessageBox.Show(sprintf "A cancellation error ocurred: %A" cxn) |> ignore)))
            
  