

open System
open System.IO

let rootPath = @"C:\Users\chrsmith\Desktop\DEFUN08\Scratch\NetFlixPrizeData\NetFlixPrizeData\training_set\"
let ratedMovies = Directory.GetFiles(rootPath, "mv_*.txt")

let processRatingsFile (filePath : string) =
    seq {
        use fileReader = new StreamReader(filePath)
        let movieID = fileReader.ReadLine().Replace(":", "")
        
        while not fileReader.EndOfStream do
            yield sprintf "%s,%s" movieID (fileReader.ReadLine()) 
    }

let combinedLog = File.CreateText(Path.Combine(rootPath, "Combined.txt"))

ratedMovies
|> Array.map processRatingsFile
|> Seq.concat
|> Seq.iter combinedLog.WriteLine

combinedLog.Close()