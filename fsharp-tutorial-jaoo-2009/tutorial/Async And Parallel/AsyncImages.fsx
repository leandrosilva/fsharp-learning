//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of doing in-memory, parallel
// image processing using asynchronous I/O.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

open System.IO
open System.Net
open System.Drawing
open System.Windows.Forms
open System.Text.RegularExpressions
open System.Collections.Generic

let run x = async { do! Async.SwitchToThreadPool();  
                    return! x } |> Async.RunSynchronously

let numImages = 200
let size = 512 
let numPixels = size * size

let MakeImageFiles() =
    printfn "making %d %dx%d images... " numImages size size
    let pixels = Array.init numPixels (fun i -> byte i)
    for i = 1 to numImages do 
        System.IO.File.WriteAllBytes(sprintf "Image%d.tmp" i, pixels)
    printfn "done."

MakeImageFiles()


let processImageRepeats = 6

/// Repeat the application of a function 'n' times
let rec RepeatN n f x = if n = 0 then x else RepeatN (n-1) f (f x)

/// Transform an image
let TransformImage (pixels, i) = 
    printfn "TransformImage %d" i;
    // Perform some CPU-intensive operation on the image.
    pixels |> RepeatN (processImageRepeats*(i%5)) (Array.map (fun b -> b + 1uy)) 



/// An asynchronous task to process an image
let ProcessImageAsync i =
    async { // Open the file
            try 
                use  inStream =  File.OpenRead(sprintf "Image%d.tmp" i)
                
                // Read it asynchronously 
                let! pixels = inStream.AsyncRead(numPixels)
                
                // Transform the image, synchronously
                let  pixels2 = TransformImage(pixels,i)
                
                // Open the file for writing, synchronously
                use  outStream =  File.OpenWrite(sprintf "Image%d.done" i)
                failwith "We failed!"
                
                // Write the result, synchronously
                do!  outStream.AsyncWrite(pixels2,0,pixels2.Length) 
            with err -> 
                printfn "something went wrong during the async operation: %s" err.Message } 


run (ProcessImageAsync 1)





            
            
/// Run 'numImages' asynchronous image processing
/// tasks in parallel and wait for the results
let ProcessImagesAsync() =
     
    Async.Parallel 
       [ for i in 1 .. numImages -> ProcessImageAsync i ] 

    |> Async.RunSynchronously

    |> ignore






/// The version using normal synchronous techniques. Runs slower.
let ProcessImageSync i =

    use inStream =  File.OpenRead(sprintf "Image%d.tmp" i)

    let pixels = Array.zeroCreate numPixels

    let nPixels = inStream.Read(pixels,offset=0,count=numPixels);

    let pixels' = TransformImage(pixels,i)

    use outStream =  File.OpenWrite(sprintf "Image%d.done" i)

    outStream.Write(pixels',offset=0,count=numPixels) 





let ProcessImagesSync() =
    for i in 1 .. numImages do
        ProcessImageSync(i)



#time

MakeImageFiles()

ProcessImagesSync()  
ProcessImagesAsync() 





/// This example shows how to take the above parallel algorithm and convert it to be a parallel,
/// background worker component that has a friendly, event-based .NET interface and reports results to the 
/// user interface.
type BackgroundImageProcessor() = 
    let jobStarted = new Event<_>()
    let dataRead = new Event<_>()
    let jobFinished = new Event<_>()
    let allJobsFinished = new Event<_>()
    let ctxt = System.Threading.SynchronizationContext.Current
    
    let processOneImage jobNumber = 
         async { // Open the file
            try 
                ctxt.Post((fun _ -> jobStarted.Trigger(jobNumber)), null)
                use  inStream =  File.OpenRead(sprintf "Image%d.tmp" jobNumber)
                
                // Read it asynchronously 
                let! pixels = inStream.AsyncRead(numPixels)
                ctxt.Post((fun _ -> dataRead.Trigger(jobNumber, pixels.Length)), null)
                
                // Transform the image, synchronously
                let  pixels2 = TransformImage(pixels,jobNumber)
                
                // Open the file for writing, synchronously
                use  outStream =  File.OpenWrite(sprintf "Image%d.done" jobNumber)
                if jobNumber = 10 then 
                    failwith "We failed!"
                
                // Write the result, synchronously
                do!  outStream.AsyncWrite(pixels2,0,pixels2.Length) 
                ctxt.Post((fun _ -> jobFinished.Trigger(jobNumber, "done")), null)
            with err -> 
                ctxt.Post((fun _ -> jobFinished.Trigger(jobNumber, "failed")), null)

                printfn "something went wrong during the async operation: %s" err.Message } 
                        
    member this.StartProcessing(jobNumbers:int list) = 
      async { let! allFinished = Async.Parallel [ for jobNumber in jobNumbers -> processOneImage jobNumber ]
              ctxt.Post((fun _ -> allJobsFinished.Trigger()), null) }
           |> Async.Start

    [<CLIEvent>]
    member this.JobStarted : IEvent<int> = jobStarted.Publish
    [<CLIEvent>]
    member this.DataRead : IEvent<int * int> = dataRead.Publish
    [<CLIEvent>]
    member this.JobFinished : IEvent<int * string> = jobFinished.Publish
    [<CLIEvent>]
    member this.AllJobsFinished : IEvent<unit> = allJobsFinished.Publish


let backgroundProcessor = BackgroundImageProcessor() 
backgroundProcessor.DataRead.Add(fun _ -> printfn "the data got read")
backgroundProcessor.JobStarted.Add(fun _ -> printfn "the job has been started")
backgroundProcessor.JobFinished.Add(fun (jobNumber, message) -> printfn "job %d finished with status %s" jobNumber message)
backgroundProcessor.AllJobsFinished.Add(fun () -> printfn "all jobs finished at %A" System.DateTime.Now)


backgroundProcessor.StartProcessing [ 0 .. 20 ]

