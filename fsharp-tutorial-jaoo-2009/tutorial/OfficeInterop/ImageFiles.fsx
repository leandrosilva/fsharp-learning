//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of doing COM-interop
// programming using the .NET Primary Interop Assemblies for Microsoft Excel.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


#r "Microsoft.Office.Interop.Excel"

open System
open System.IO
open System.Reflection
open Microsoft.Office.Interop.Excel

let app = ApplicationClass(Visible = true)

let sheet = app.Workbooks
               .Add()
               .Worksheets.[1] :?> _Worksheet

let setCellText (x : int) (y : int) (text : string) = 
    let range = sprintf "%c%d" (char (x + int 'A')) (y+1)
    sheet.Range(range).Value(Missing.Value) <- text


let printCsvToExcel rowIdx (csvText : string) =
    csvText.Split([| ',' |])
    |> Array.iteri (fun partIdx partText -> setCellText partIdx rowIdx partText)
    

let rec filesUnderFolder basePath = 
    seq {
        yield! Directory.GetFiles(basePath)
        for subFolder in Directory.GetDirectories(basePath) do
            yield! filesUnderFolder subFolder 
    }
            
filesUnderFolder (Environment.GetFolderPath(Environment.SpecialFolder.MyPictures))
|> Seq.map (fun filename -> new FileInfo(filename))
|> Seq.map (fun fileInfo -> sprintf "%s, %s, %d, %s" 
                                fileInfo.DirectoryName fileInfo.Name 
                                fileInfo.Length (fileInfo.CreationTime.ToShortDateString()))
|> Seq.iteri printCsvToExcel

