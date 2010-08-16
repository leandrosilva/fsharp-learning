//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of inheritance and 
// subtyping in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// Accessibility annotations
type CustomType(x:int) =
    // executed on construction
    let z = x + 1

    // not executed on construction
    let f () = z + x
    
    // Public: Anybody can see/call
    member public    this.IsPublic()   = ()
    // Internal: Only types in the same Assembly can view
    member internal  this.IsInternal() = ()
    // Private: Only this type can view it (nobody else, even derived classes)
    member private   this.IsPrivate()  = ()
    
    // default to 17
    new () = CustomType(17)
    
// Inheritance in F#
[<AbstractClass>]
type Animal() =
    abstract Speak : unit -> unit
    abstract Genus : string
    
type Cat() =
    inherit Animal()
    override this.Speak() = printfn "Meow"
    override this.Genus = "Felis"
    
    member this.Lives = 9
    
type Dog() =
    inherit Animal()
    override this.Speak() = printfn "Arrf"
    override this.Genus = "Canis"
    
    member this.BestFriend() = printfn "man"

// Type casting
let gob = new Dog()
let gobAsAnimal = gob :> Animal

let steve = new Dog()
let steveAsObj = steve :> obj
let steveAsCat = steveAsObj :?> Cat
    
    
// A sealed type
[<Sealed>]
type Pomeranian() =
    inherit Dog()
    override this.Genus = sprintf "A very furry %s" base.Genus
    
