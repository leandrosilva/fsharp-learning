//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of using some less common
// .NET types in F#, notably enums, structs and delegates.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


/// An enum-like union type in F#. 
type Options =
    | FlagA
    | FlagB
    | FlagC
    | FlagD
    
let f1 (flags : Options list) = ()
f1 [FlagA; FlagC]

/// An enum type in F#. An enum is like a discriminated union but each case is associated
/// with a fixed integer value. Enum types also support bitwise operations. The underlying
/// value of an enum can be any integer, so the type is not restricted to the 4 cases listed.
type EnumOptions =
    | FlagA = 0
    | FlagB = 1
    | FlagC = 3
    | FlagD = 7
    
let f2 (flags : EnumOptions) = ()
f2 (EnumOptions.FlagA ||| EnumOptions.FlagC)


/// A struct type in F#, defined without a primary constructor.
[<Struct>]
type StructPoint =
    val m_x : int
    val m_y : int
    new(x, y) = { m_x = x; m_y = y }
    
let test1 = new StructPoint()
let test2 = new StructPoint(10, 20)


/// A struct type in F#, defined using a primary constructor.
///
/// However, you may not use 'let' bindings in the primary constructor.
[<Struct>]
type StructPoint2(x:int,y:int) =
    member this.X = x
    member this.Y = y
    
let test3 = new StructPoint2()
let test4 = new StructPoint2(10, 20)
let result = test3.X + test3.Y + test4.X + test4.Y

type MyDelegate = delegate of int * int -> string

// NOTE: constructing multi-argument delegate types uses a _curried_ function. When invoked
// you pass arguments as if they were tupled. This is a non-orthogonality dating from an 
// early F# design decision.
let delegate1 = MyDelegate (fun n1 n2 -> string (n1 + n2))

// Invoke the delegate. Note you have to explicity call 'Invoke'
let result2 = delegate1.Invoke(3,4)
