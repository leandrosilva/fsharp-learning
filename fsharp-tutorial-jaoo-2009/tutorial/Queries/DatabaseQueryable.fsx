#r "System.Core.dll"
#r "System.Data.Linq.dll"
#r "northwnd.dll"
#r "FSharp.PowerPack.Linq.dll"

open System
open System.IO
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Linq.Query

let sqlServerInstance = @".\SQLEXPRESS"
let connString = @"AttachDBFileName='" + __SOURCE_DIRECTORY__ + @"\NORTHWND.MDF';Server='" + sqlServerInstance + "';user instance=true;Integrated Security=SSPI;Connection Timeout=30" 

let db = new NORTHWND(connString)

db.Log <- System.Console.Out

// Define a query
let q1 = 
    query <@ seq { for i in db.Customers do 
                     for j in db.Employees do 
                       if i.Country = j.Country then 
                          yield (i.ContactName, j.FirstName, j.LastName) } @> 

// Now execute a query
q1 |> Seq.toList

let q2 = 
   query <@ seq { for c in db.Customers do 
                     for e in db.Employees do 
                         if c.Address.Contains("Jardim") && 
                            c.Address.Contains("rosas") then 
                               yield (e.LastName,c.ContactName) } @> 

// Now execute a query
q2 |> Seq.toList

let q3 = 
   query <@ seq { for c in db.Customers do 
                     for e in db.Employees do 
                         if c.Address.Contains("Jardim") && 
                            c.Address.Contains("rosas") then 
                               yield e }
            |> Seq.sortBy (fun e -> e.LastName) @>  


q3 |> Seq.toList

query <@ seq { for e in db.Employees do 
                  for c in db.Customers do
                     if e.Country = c.Country then 
                         yield e }  
         |> Seq.length  @> 

let q4 = query <@ Query.join 
                     db.Employees 
                     db.Customers 
                     (fun e -> e.Country) 
                     (fun c -> c.Country) 
                     (fun e c -> e)  
                  |> Seq.sortBy (fun e -> e.Country)
                  |> Seq.length  @> 

q4                   
