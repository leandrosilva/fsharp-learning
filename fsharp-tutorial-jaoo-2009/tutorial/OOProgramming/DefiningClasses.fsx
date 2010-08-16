//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of class types in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

type Vector(x : float, y : float, z : float) =
    member this.X = x
    member this.Y = y
    member this.Z = z
    
    // This is a property
    member this.Length = 
        let sqr x = x * x
        sqrt <| sqr this.X + sqr this.Y + sqr this.Z
    
    // This is a method
    member this.Add(v : Vector) = 
        new Vector(this.X + v.X, this.Y + v.Y, this.Z + v.Z)
    
    // Static method
    static member Distance (v1 : Vector) (v2 : Vector) =
        let v = new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z)
        v.Length

    // Operator overloading
    static member (+) (v1 : Vector, v2 : Vector) =
        v1.Add(v2)
        
    // System.Object methods
    override this.ToString() = 
        sprintf "%f, %f, %f" this.X this.Y this.Z
        
let a = new Vector(1.0, 1.0, 1.0)
let b = new Vector(-1.0, 2.0, 5.0)
let c = a.Add(b)
let d = a + b
let e = Vector.Distance a b

