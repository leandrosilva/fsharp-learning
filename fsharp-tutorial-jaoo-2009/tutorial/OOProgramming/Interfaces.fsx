//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of interfaces and 
// interface-based subtyping in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// Defining a new interface
// - Type inference knows its an interface
type ITitle =
    abstract Title : string 
    
// Implementing an interface
type Person(first, last) =
    member this.First = first
    member this.Last  = last

type Elvis() =
    inherit Person("Elvis", "Presley")
    interface ITitle with
        member this.Title = "The King"
        
type DonSyme() =
    inherit Person("Don", "Syme")
    interface ITitle with
        member this.Title = "The Crusher"

type Texas() =
    interface ITitle with
        member this.Title = "The Lone Star State"

let printTitle (x : 'a when 'a :> Person and 'a :> ITitle) =
    let person = x :> Person
    let title  = x :> ITitle
    printfn "%s '%s' %s" person.First title.Title person.Last
    
// IDisposable, the savior of unmanaged resources!