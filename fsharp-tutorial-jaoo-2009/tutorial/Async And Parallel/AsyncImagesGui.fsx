//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of doing in-memory, parallel
// image processing using asynchronous I/O.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

#time

open System.IO
open System.Net
open System.Drawing
open System.Windows.Forms
open System.Text.RegularExpressions
open System.Collections.Generic

let numImages = 30
let size = 512 
let numPixels = size * size

/// Make a set of dummy image files
let MakeImageFiles() =
    printfn "making %d %dx%d images... " numImages size size
    let pixels = Array.init numPixels (fun i -> byte i)
    for i = 1 to numImages do 
        System.IO.File.WriteAllBytes(sprintf "Image%d.tmp" i, pixels)
    printfn "done."

MakeImageFiles()

let processImageRepeats = 50

let rec RepeatN n f x = if n = 0 then x else RepeatN (n-1) f (f x)

/// Do some CPU-intensive transformation on an image
let TransformImage(pixels, i) = 
    printfn "TransformImage %d" i;
    // Perform some CPU-intensive operation on the image.
    pixels |> RepeatN (processImageRepeats*(i%5)) (Array.map (fun b -> b + 1uy)) 


/// Process an image asynchronously
let ProcessImageAsync i =
    async { // open the file
            use  inStream =  File.OpenRead(sprintf "Image%d.tmp" i)
            // read it asynchronously 
            let! pixels = inStream.AsyncRead(numPixels)
            // transform the image, synchronously
            let  pixels' = TransformImage(pixels,i)
            // open the file for writing, synchronously
            use  outStream =  File.OpenWrite(sprintf "Image%d.done" i)
            // write the result, asynchronously
            do  outStream.Write(pixels',0,pixels'.Length)  } 
            

#load "ParallelWorkerAgent.fs"
open Demo.ParallelWorkers




/// Process images asynchronously
///
/// Note how close the code is to the following code from the synchronous
/// version: 
///     Async.Parallel [ for i in 1 .. numImages -> ProcessImageAsync i ]

let worker = 
    BackgroundParallelWorker 
       [ for i in 1 .. numImages -> ProcessImageAsync i ]




    

let form = new Form(Visible = true, Text = "A Simple F# Form", 
                    TopMost = true, Size = Size(600,800))

let data = new DataGridView(Dock = DockStyle.Fill, 
                            Text = "F# Programming is Fun!",
                            Font = new Font("Lucida Console",10.0f),
                            ForeColor = Color.DarkBlue)

form.Controls.Add(data)

type Row = 
    { ImageNumber: int; 
      Status: string }

data.DataSource <- [| {ImageNumber=1; Status = ""} |]
data.Columns.[0].Width <- 400

// Add an event handler to update the GUI whenever the result set changes
worker.ResultSetChanged.Add(fun results -> 
    data.DataSource <- 
        [| for KeyValue(i,v) in results do 
              let status = 
                  match v with 
                  | None -> "working" 
                  | Some(res) -> "done"
              yield {ImageNumber=i; Status=status } |])


worker.Start()

//Async.CancelDefaultGroup()
