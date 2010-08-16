//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of using extensible 
// pattern matching in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


#nowarn 

// Example from "Extensible Pattern Matching via a Lightweight Language Extension"
// Don Syme, Gregory Neverov, and James Margetson. ICFP 2007

open System
open System.Xml

// Concrete language
let xmlDoc = 
    let temp = new System.Xml.XmlDocument()  
    let superHerosXmlDoc = 
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>
        <Scene>
            <Sphere r='3' x='4' y='3' z='0' />
            <Intersect>
                <Sphere r='2' x='1' y='0' z='0'/>
                <Intersect>
                    <Sphere r='2' x='4' y='0' z='0'/>
                    <Cube d='1' x='6' y='7' z='8' />
                    <Sphere r='2' x='-3' y='0' z='0'/>
                </Intersect>
                <Cube d='2' x='-2' y='1' z='0'/>
            </Intersect>
        </Scene>
        "
    temp.LoadXml(superHerosXmlDoc)
    temp


// Abstract representation
type GeometricScene =
| Cube      of float * float * float * float
| Sphere    of float * float * float * float
| Intersect of GeometricScene list







// Mach an XML element
let (|Elem|_|) name (inp: #XmlNode) =
    if inp.Name = name then Some(inp)
    else                    None

// Get the attributes of an element
let (|Attributes|) (inp: #XmlNode) = inp.Attributes

// Match a specific attribute
let (|Attr|) attrName (inp: XmlAttributeCollection) =
    match inp.GetNamedItem(attrName) with
    | null -> failwith (attrName + " not found")
    | attr -> attr.Value

// Convert a string to a float
let (|Float|) (s : string) = float s

// Parses a vector out of an attribute collection
let (|Vector|) inp =
    match inp with
    | (Attr "x" (Float x) & 
       Attr "y" (Float y) & 
       Attr "z" (Float z)) 
        -> (x,y,z)

// Parses a GeometricScene from an XML node
let rec (|ShapeElem|_|) inp =
    match inp with
    | Elem "Sphere" (Attributes (Attr "r" (Float r) & Vector (x,y,z)))
        -> Some (Sphere (r,x,y,z))
    
    | Elem "Intersect" (ShapeElems(objs))
        -> Some (Intersect objs)
    
    | Elem "Cube" xmlElement
        -> match xmlElement with
           | Attributes xmlElementsAttributes 
               -> match xmlElementsAttributes with
                  | Attr "d" dAttrib & Vector (x, y, z)
                      -> match dAttrib with
                         | Float d -> Some(Cube(d, x, y, z))

    // Did not recognize XmlNode as Shape element
    | _ -> None 
and (|ShapeElems|) inp =
    [ for elem in inp.ChildNodes do
       match elem with 
       | ShapeElem y -> yield y 
       | _ -> () ]

let parse inp =
    match inp with
    | Elem "Scene" (ShapeElems elems) -> elems
    | _ -> failwith "not a scene graph"

let printScene scene = 
    let rec printSceneTabbed tabs scene  =
        match scene with
        | Sphere(r, x, y, z)       -> printfn "%s Sphere(%f) at [%f, %f, %f]" tabs r x y z
        | Intersect (nestedScenes) -> List.iter (printSceneTabbed (tabs + "    ")) nestedScenes
        | Cube(d, x, y, z)         -> printfn "%s Cube(%f) at [%f, %f, %f]" tabs d x y z
    printSceneTabbed "" scene
        
let parsedScene = Intersect(parse (xmlDoc.DocumentElement.CloneNode(true)))

printScene parsedScene