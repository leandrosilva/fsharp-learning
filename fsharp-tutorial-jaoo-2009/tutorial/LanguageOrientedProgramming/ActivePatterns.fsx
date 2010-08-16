//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of using extensible 
// pattern matching in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


open System

// SingleCase Active Patterns
let (|IntValue|) input = Int32.Parse(input)

let printValue str =
    match str with
    // Match a string constant like normal
    | "foobar"   -> printfn "The string is Friendly-to-Object-Oriented-Based-Automated-Recievers"
    // Match against an integer constant too - via AP
    | IntValue 0 -> printfn "str is zero"
    | IntValue 1 -> printfn "str is one"
    | IntValue 2 -> printfn "str is two"
    // Capture active pattern output
    | IntValue x -> printfn "str is %d" x


let (|WordLen|) (str : string) = str.Length

let printWordLen word = 
    match word with
    | WordLen x when x < 4 -> printfn "That word is really short."
    | WordLen 4            -> printfn "The length of the word is 4"
    | WordLen 5            -> printfn "The length of the word is 5"
    | WordLen 6            -> printfn "The length of the word is 6"
    | WordLen _            -> printfn "The word is longer than 6 letters"






// Multi-Case Active Patterns
let (|Even|Odd|Zero|) x =
    if x = 0       then Zero
    elif x % 2 = 0 then Even
    else                Odd

// Takes an int and prints its status
let printStatus x =
    match x with
    | Zero -> printfn "%d is zero" x
    | Even -> printfn "%d is even" x
    | Odd  -> printfn "%d is odd " x
    
List.iter printStatus [0..5]


let winiFile = "
; for 16-bit app support
[fonts]
[extensions]
[mci extensions]
[files]
[Mail]
MAPI=1
CMCDLLNAME32=mapi32.dll
CMC=1
MAPIX=1
MAPIXVER=1.0.0.1
OLEMessaging=1
[MCI Extensions.BAK]
m2v=MPEGVideo
mod=MPEGVideo
"

let (|Header|Assignment|Comment|Unknown|) (input : string) =
    if input.StartsWith(";") then 
        Comment(input)
    elif input.[0] = '[' && input.[input.Length - 1] = ']' then
        Header(input.Substring(1, input.Length - 2))
    else
        let i = input.IndexOf("=")
        if i = -1 then
            Unknown
        else
            Assignment(input.Substring(0, i), input.Substring(i + 1))

// Parse Wini
let parseWini (contents : string) = 
    contents.Split([| "\r"; "\n" |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.iter (function
                  | Header(header)  -> printfn "Header     - %s" header
                  | Assignment(x,y) -> printfn "Assignment - %s <- %s" x y
                  | Comment(c)      -> printfn "Comment    - %s" c
                  | Unknown         -> printfn "Unknown line")
    
parseWini winiFile


// Partial Active Pattern
let (|ToInt|_|) str = 
    let (parsed, result) = Int32.TryParse(str)
    if parsed then Some(result)
    else           None

let (|ToFloat|_|) str = 
    let (parsed, result) = Single.TryParse(str)
    if parsed then Some(result)
    else           None

// Takes a string and prints whether it is an int or float
let parseValue str =
    match str with
    | ToInt   x -> printfn "str is an int with value %d" x
    | ToFloat x -> printfn "str is a float with value %f" x
    | _         -> printfn "str is neither an int nor a float"
    

// Does the file end with the given extension?
let (|EndsWith|_|) fileExtension (input : string) = 
    if input.EndsWith(fileExtension) 
    then Some() 
    else None

open System.IO
Directory.GetFiles(__SOURCE_DIRECTORY__)
|> Array.filter (function | EndsWith ".fs" _ -> true 
                          | _ -> false)
|> Array.iter (printfn "%s")


// Regular Expression Active Patterns
open System
open System.Text.RegularExpressions

(* Returns the first match group if applicable. *)
let (|Match|_|) (pat : string) (inp : string) = 
    let m = Regex.Match(inp, pat) in
    // Note the List.tl, since the first group is always the entirety of the matched string.
    if m.Success
    then Some (List.tail [ for g in m.Groups -> g.Value ]) 
    else None

(* Expects the match to find exactly three groups, and returns them. *)
let (|Match3|_|) (pat:string) (inp:string) = 
    match (|Match|_|) pat inp with 
    | Some [fst; snd; trd] -> Some (fst, snd, trd)
    | Some [] -> failwith "Match3 succeeded, but no groups found. Use '(.*)' to capture groups"
    | Some _  -> failwith "Match3 succeeded, but did not find exactly three matches."
    | None    -> None   

// DateTime.Now.ToString() = "5/18/2008 4:12:25 PM"
match DateTime.Now.ToString() with
| Match3 "(\d*)/(\d*)/(\d*) .*" (day, month, year) 
    -> printfn "Day = %s, Month = %s, Year = %s" day month year
| _ -> failwith "Unable to find a match"