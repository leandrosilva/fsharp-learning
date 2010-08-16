//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of piplining
// and file scripting in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


open System
open System.IO

// Gets all files under a given folder
let rec filesUnderFolder rootFolder = 
    seq {
        for file in Directory.GetFiles(rootFolder) do
            yield file
        for dir in Directory.GetDirectories(rootFolder) do
            yield! filesUnderFolder dir 
    }

// Gets the information about a file
let fileInfo filename = new FileInfo(filename)

// Gets the file size from a FileInfo object
let fileSize (fileinfo : FileInfo) = fileinfo.Length

// Converts a byte count to MB
let bytesToMB (bytes : int64) = bytes / (1024L * 1024L)

// Imperative style
let sizeOfFilesInFolder1 folder =
    
    let allFiles = filesUnderFolder folder
    
    let mutable totalSizeMB = 0L
    for file in allFiles do
        let fileInfo = FileInfo(file)
        let fileSize = fileSize (fileInfo)
        totalSizeMB <- bytesToMB fileSize
        
    totalSizeMB
    
// Functional style
let sizeOfFilesInFolder2 folder =
    
    filesUnderFolder folder
    |> Seq.map fileInfo
    |> Seq.map fileSize
    |> Seq.fold (+) 0L

let divideBy x y =
    if y <> 0 then x / y
    else
        // Use pipe-backwards operator here
        failwith <| sprintf "Cannot divide %d by zero" x
        //failwith (sprintf "Cannot divide %d by zero" x)

divideBy 3 0        

