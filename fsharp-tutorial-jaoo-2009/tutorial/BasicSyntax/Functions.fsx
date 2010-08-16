//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of type inference in F#
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// Type inference - automatic generics


let f x = (x,x)

let g x = x + x

let h x = x + x + 0.0

// Generic functions
let createList x = [ x ]







//----------------------------

//let x1,y1,z1 = 1,2,"3"

let xy = 
    printfn "hello"; 
    (1,2)

let x,y = 
    printfn "hello"; 
    (1,2)


let xy2 = lazy (1+1, 2+2)

xy2
xy2.Force()


let xy3 = 
   lazy 
      (1+1, 2+2)

xy3

xy3.Force()
xy3.Force()








//----------------------------

// Higher-order functions
let applyFunction f x = f x

let rec functionalForLoop lb ub f =
    if lb < ub then
        f()
        functionalForLoop (lb + 1) ub f
    else
        ()   
        
       
// Symbolic functions
let (!=)  x y = (x <> y)
let (=/=) x y = (x <> y)
