//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of doing COM-interop
// programming using the .NET Primary Interop Assemblies for Microsoft Word.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


//#I @"C:\Program Files\Microsoft Visual Studio 9.0\Visual Studio Tools for Office\PIA\Office12\"
#r "Office.dll"
#r "stdole.dll"
#r "Microsoft.Office.Interop.Word.dll"

open Microsoft.Office.Interop.Word

let private m_word : ApplicationClass option ref = ref None

let OpenWord()        = m_word := Some(new ApplicationClass())
let GetWordInstance() = Option.get !m_word
let CloseWord()       = (GetWordInstance()).Quit()

let OpenDocument filePath = 
    printfn "Opening %s..." filePath
    
    let mutable filePath = box filePath
    GetWordInstance().Documents.Open(&filePath)

let PrintDocument (doc : Document) =
    
    printfn "Printing %s..." doc.Name

    let comRef x = ref (box x)

    doc.PrintOut(
        Background  = comRef true, 
        Range       = comRef WdPrintOutPages.wdPrintAllPages,
        Copies      = comRef 1, 
        PageType    = comRef WdPrintOutRange.wdPrintAllDocument,
        PrintToFile = comRef false,
        Collate     = comRef true, 
        ManualDuplexPrint = comRef false,
        PrintZoomColumn = comRef 2,             // Pages 'across'
        PrintZoomRow    = comRef 2)             // Pages 'up down'

let CloseDocument (doc : Document) =
    printfn "Closing %s..." doc.Name
    let mutable falseValue = box false
    doc.Close(SaveChanges = &falseValue)

// -------------------------------------------------------------

let currentFolder = __SOURCE_DIRECTORY__

open System
open System.IO

try
    OpenWord()

    printfn "Printing all files in [%s]..." currentFolder

    Directory.GetFiles(currentFolder, "*.docx")
    |> Array.iter 
        (fun filePath -> 
            let doc = OpenDocument filePath
            PrintDocument doc
            CloseDocument doc)
finally
    CloseWord()

printfn "Press any key..."
Console.ReadKey(true) |> ignore