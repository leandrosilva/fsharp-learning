#if INTERACTIVE
#r "WindowsBase.dll"
#r "PresentationCore.dll"
#r "PresentationFramework.dll"
#endif

open System.IO
open System.Net
open System.Text.RegularExpressions
open System.Threading
open System.Windows
open System.Windows.Controls

type Agent<'T> = MailboxProcessor<'T>

let limit = 50
let linkPattern = "href=\s*\"[^\"h]*(http://[^&\"]*)\""
let getLinks text = 
  [for link in Regex.Matches(text, linkPattern) -> link.Groups.[1].Value]

let (<--) (m:MailboxProcessor<_>) msg = m.Post(msg)

let guiContext = SynchronizationContext.Current

let window = new Window(Title="Web Crawler Example", Visibility = Visibility.Visible, Topmost = true, Height = 400., Width = 600.)
let mainPanel = new StackPanel(Orientation = Orientation.Vertical)
let startPanel = new StackPanel(Orientation = Orientation.Horizontal, Height = 20.)
let urlBlock = new TextBlock(Text="Enter start url:", Width=100.)
let urlBox = new TextBox(Width=200., Text="http://www.bing.com/news")
let searchButton = new Button(Content="Start", Width=100.)
let resultBox = new TextBox(Width = 550., Height = 300., HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto)
startPanel.Children.Add(urlBlock) |> ignore
startPanel.Children.Add(urlBox) |> ignore
startPanel.Children.Add(searchButton) |> ignore
mainPanel.Children.Add(startPanel) |> ignore
mainPanel.Children.Add(resultBox) |> ignore
window.Content <- mainPanel
resultBox.HorizontalAlignment <- HorizontalAlignment.Left

type RequestGate(n) =
  let semaphore = new Semaphore(n, n)
  member this.AsyncAcquire(?timeout) =
    async { let! ok = Async.AwaitWaitHandle(semaphore, ?millisecondsTimeout=timeout)
            if ok then
              return { new System.IDisposable with
                         member this.Dispose() =
                           semaphore.Release() |> ignore }
            else return! failwith "Couldn't acquire semaphore" }
            
let webRequestGate = RequestGate(5)

let collectLinks (url:string) =
  async {
          let! html =
            async { use! holder = webRequestGate.AsyncAcquire()
                    let request = WebRequest.Create(url, Timeout=5)
                    use! response = request.AsyncGetResponse()
                    use reader = new StreamReader(response.GetResponseStream())
                    return reader.ReadToEnd() }
          
          let links = getLinks html
          do! Async.SwitchToContext guiContext
          resultBox.Text <- sprintf "%s\r\nFinished reading %s, got %d links" resultBox.Text url (List.length links)
          
          do! Async.SwitchToThreadPool()
          return links }
          
let urlCollector = 
  Agent.Start(fun inbox ->
    let rec waitForUrl (visited:Set<string>) =
      async { if visited.Count < limit then
                let! url = inbox.Receive()
                if not <| Set.contains url visited then
                  do Async.Start ( 
                        async { let! links = collectLinks url
                                for link in links do inbox <-- link } )
                return! waitForUrl (Set.add url visited) }
                
    waitForUrl Set.empty)                

searchButton.Click.Add(
  fun args -> 
    resultBox.Text <- ""
    urlCollector <-- urlBox.Text)