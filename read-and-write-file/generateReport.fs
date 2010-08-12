open System
open System.IO

let obtainWords (line : string) = 
    line.Split(",".ToCharArray())

let padWord (word : string) = 
    word.PadRight(25, ' ') 

let applyPadding (words : string []) = 
    Array.map (padWord) words 

let joinWords (words : string []) = 
    String.Concat(words)

let generateReport (name : string) =
  Console.WriteLine("Generation report from '{0}' file", name)
  try
    let formatedLines = File.ReadAllLines(name)
                        |> Array.map(obtainWords) 
                        |> Array.map(applyPadding) 
                        |> Array.map(joinWords)
    
    File.WriteAllLines("contact.report", formatedLines)
  with
  | :? FileNotFoundException as ex
      -> Console.WriteLine("error => {0}", ex.Message)

try
  let name = Environment.GetCommandLineArgs().GetValue(1).ToString()
  generateReport(name)
with
| :? IndexOutOfRangeException as ex
    -> Console.WriteLine("You must pass file name as argument. Try 'generateReport.exe contact.csv'.")