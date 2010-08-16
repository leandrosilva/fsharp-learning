//The assembly FSharp.PowerPack.Linq.dll contains support for executing F# quotation expressions as queries under the LINQ IQueryable paradigm. For example:
#r "System.Core.dll"
#r "FSharp.PowerPack.Linq.dll"

open Microsoft.FSharp.Linq.Query

type Customer = 
    { Name:string; 
      UniqueID: int; 
      Weighting:float; 
      Preferences: int list }

let c1 = { Name="Don";    UniqueID=6; Weighting=6.2; Preferences=[1;2;4;8] }
let c2 = { Name="Peter";  UniqueID=7; Weighting=4.2; Preferences=[10;20;30;40] }
let c3 = { Name="Freddy"; UniqueID=8; Weighting=9.2; Preferences=[11;12;14] }
let c4 = { Name="Freddi"; UniqueID=8; Weighting=1.0; Preferences=[21;22;29;42] }

let data = [c1;c2;c3;c4]
open System.Linq
let db = Queryable.AsQueryable<Customer>(data)

let q1 = 
    seq { for i in db do 
            for j in db do 
              if i.Name.Length = j.Name.Length && i.Name <> j.Name then
                 if i.Name.Length = j.Name.Length then 
                   yield (i.Name,j.Name) } 

let q2 = 
    query <@ seq { for i in data do 
                      if i.Name.Length = 6 then
                         yield (i.Name,i.UniqueID) }  @>

q1 |> Seq.length

q1 |> Seq.toList
// Define a query
// Now execute a query
q1 |> Query.query
