//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of using the 'schema
// compilation' design pattern in F#. This design pattern is documented in 
// the book Expert F#, chapter 9.
//
// In this design pattern, type definitions are used as the description of data 
// schemas. A generic routine takes this type as a type parameter and provides
// serialization, deserialization and other services for reading, writing
// and otherwise manipulating instances of the objects, and can provide services
// related to the schema itself. 
//
// The generic routine may be a generic object type (as in this example), and
// uses reflection over the object type to analyze the input scheme, usually 
// using a combination of the System.Reflection and Microsoft.FSharp.Reflection 
// libraries. The costs of the analysis are amortized through the use of an object 
// type.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

open System
open System.IO
open System.Windows.Forms
open System.Drawing

open Microsoft.FSharp.Reflection

let form = new Form(Visible = true, Text = "A Simple F# Form", 
                    TopMost = true, Size = Size(600,600))

let textBox = 
    new RichTextBox(Dock = DockStyle.Fill, Text = "F# Programming is Fun!",
                    Font = new Font("Lucida Console",16.0f,FontStyle.Bold),
                    ForeColor = Color.DarkBlue)


form.Controls.Add(textBox)

textBox.Click.Add(fun evArgs -> printfn "Click!")

// ----------------------------
// Dumping data to a form

/// Show an arbitrary value in the text box
let show x = 
   let txt = sprintf "%A" x
   textBox.Text <- txt


type ColumnAttribute(col:int) =
    inherit Attribute()
    member x.Column = col

module Permutation = 
    let ofPairs  (mappings: seq<int * int>) = 
      let p = dict mappings 
      (fun k -> if p.ContainsKey k then p.[k] else k)

/// SchemaReader builds an object that automatically transforms lines of text
/// files in comma-separated form into instances of the given type 'Schema.
/// 'Schema must be an F# record type where each field is attributed with a
/// ColumnAttribute attribute, indicating which column of the data the record
/// field is drawn from. This simple version of the reader understands
/// integer, string and DateTime values in the CSV format.
type SchemaReader<'Schema>() =

    // Grab the object for the type that describes the schema
    let schemaType = typeof<'Schema>

    // Grab the fields from that type
    let fields =
        if FSharpType.IsRecord(schemaType) then 
            FSharpType.GetRecordFields(schemaType)
        else 
            failwithf "this schema compiler expects a record type"

    // For each field find the ColumnAttribute and compute a function
    // to build a value for the field
    let schema =
        fields 
          |> Array.toList 
          |> List.mapi (fun fldIdx fieldInfo ->
            let fieldName = fieldInfo.Name
            let fieldType = fieldInfo.PropertyType
            let fieldConverter =
                match fieldType with
                |  ty when ty = typeof<string>   -> (fun (s:string) -> box s)
                |  ty when ty = typeof<int>      -> (System.Int32.Parse >> box)
                |  ty when ty = typeof<DateTime> -> (System.DateTime.Parse >> box)
                |  _ -> failwithf "Unknown primitive type %A" fieldType
            let attrib =
                match fieldInfo.GetCustomAttributes(typeof<ColumnAttribute>,
                                                    false) with
                | [| (:? ColumnAttribute as attrib) |] ->   attrib
                | _ -> failwithf "No column attribute found on field %s" fieldName
            (fldIdx,fieldName, attrib.Column, fieldConverter))
        |> List.toArray

    // Compute the permutation defined by the ColumnAttributes indexes
    let columnToFldIdxPermutation =
      Permutation.ofPairs (schema |> Array.map (fun (fldIdx,_,colIdx,_) -> (colIdx,fldIdx)))

    // Drop the parts of the schema we don't need
    let schema =
      schema |> Array.map (fun (_,fldName,_ ,fldConv) -> (fldName,fldConv))

    // Compute a function to build instances of the schema type. This uses an
    // F# library function.
    let objectBuilder = FSharpValue.PreComputeRecordConstructor(schemaType)

    // OK, now we're ready to implement a line reader
    member reader.ReadLine(textReader: TextReader) =
        let line = textReader.ReadLine()
        let words = line.Split([|','|]) |> Array.map(fun s -> s.Trim())
        if words.Length <> schema.Length then
            failwith "unexpected number of columns in line %s" line
        let words = words |> Array.permute columnToFldIdxPermutation

        let convertColumn colText (fieldName, fieldConverter) =
           try fieldConverter colText
           with e ->
               failwithf "error converting '%s' to field '%s'" colText fieldName

        let obj = objectBuilder (Array.map2 convertColumn words schema)

        // OK, now we know we've dynamically built an object of the right type
        let res = unbox<'Schema>(obj)
        res

    // OK, this read an entire file
    member reader.ReadFile(file) =
        seq { use textReader = File.OpenText(file)
              while not textReader.EndOfStream do
                  yield reader.ReadLine(textReader) }

module Example1 = 
    type CheeseClub =
        { [<Column(0)>] Name            : string
          [<Column(2)>] FavouriteCheese : string
          [<Column(1)>] LastAttendance  : System.DateTime }
    
    let reader = new SchemaReader<CheeseClub>()
    
    
    System.IO.File.WriteAllLines
        ("data.txt", [| "Steve, 12/03/2009, Cheddar";
                        "Sally, 18/02/2009, Brie"; |])
    
    reader.ReadFile("data.txt") |> show
    

module Example2 = 
    type PageImpression =
        { [<Column(0)>] URL         : string
          [<Column(1)>] DisplayedAd : string
          [<Column(2)>] Time        : System.DateTime }
    
    let reader = new SchemaReader<PageImpression>()
    
    
    System.IO.File.WriteAllLines
        ("date.txt", [| "www.bing.com, Wimbeldon#101, 12/03/2009";
                        "www.bing.com, Wimbeldon#101, 12/03/2009";
                        "www.bing.com, Wimbeldon#101, 12/03/2009";
                        "www.bing.com, Wimbeldon#101, 12/03/2009";
                        "news.bing.com, NationalOpera#102, 12/03/2009"; |])
    
    let result = reader.ReadFile("date.txt") 
    result
