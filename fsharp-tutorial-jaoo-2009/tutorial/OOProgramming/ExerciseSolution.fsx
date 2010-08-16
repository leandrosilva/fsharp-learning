//---------------------------------------------------------------------------
// This is a demonstration script showing the solution to an exercise to 
// build an object type that analyzes a file and reports a range of 
// statistics associated with the analysis.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 



open System.IO
open System.Collections.Generic

/// An object type to contain the results of an analysis of a file.
type LineAnalysis (filePath : string) =

    let lineCounts =
        let dict = new Dictionary<string, int>()
        use file = new StreamReader(filePath)
        while not file.EndOfStream do
            let line = file.ReadLine()
            if not (dict.ContainsKey(line)) then
                dict.[line] <- 1
            else
                dict.[line] <- dict.[line] + 1
        dict
        
    /// Return the number of times a particular line occurs in the file.
    member x.LineCount(line) = 
       if lineCounts.ContainsKey(line) then lineCounts.[line] else 0

    /// Return the number of unique lines in the file.
    member x.UniqueLines = 
       lineCounts.Keys |> Seq.sort
    
    /// Return the average length of the lines in a file.
    member x.AverageLineLength = 
        lineCounts 
           |> Seq.map (fun (KeyValue(k,v)) -> k.Length * v) 
           |> Seq.map float 
           |> Seq.average

    /// Return the average nunmber of times each line occurs in a file.
    member x.AverageLineFrequency = 
        lineCounts 
           |> Seq.averageBy (fun (KeyValue(k,v)) -> float v) 

/// Perform an analysis of a common file
let analysis = LineAnalysis @"c:\windows\win.ini"

analysis.UniqueLines
analysis.AverageLineLength
analysis.AverageLineFrequency


/// Perform an analysis of this file
let analysis2 = LineAnalysis (__SOURCE_DIRECTORY__ + @"\" + __SOURCE_FILE__)

analysis2.UniqueLines
analysis2.AverageLineLength
analysis2.AverageLineFrequency

