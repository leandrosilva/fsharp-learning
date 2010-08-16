//---------------------------------------------------------------------------
// This is a demonstration script showing the famous 'visitor' pattern when
// coded in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// AST
type Vector3 = 
    | Vector3 of float * float * float

    override this.ToString() = 
        let ( Vector3(x, y, z) ) = this
        sprintf "[%f, %f, %f]" x y z



type Circle = float * Vector3

type Line = Vector3 * Vector3

type SceneGraph =
    | Circle      of Circle
    | Line        of Line
    | ChildScenes of SceneGraph list    

let circleMiddle = Vector3(10.0,10.0,0.0)
let circleEdge = Vector3(13.0,10.0,0.0)

let graph1 = Circle(3.0, circleMiddle) 
let graph2 = Line(circleMiddle, circleEdge) 

let (++) sg1 sg2 = ChildScenes [sg1; sg2] 

let compositeSceneGraph = graph1 ++ graph2

graph1.ToString()

    
let rec printGraph graph tabDepth =

    let padding = "\t\t\t\t\t\t\t\t\t\t".Substring(0, tabDepth)

    match graph with
    | Circle(rad, pos) -> 
        printfn "%sCircle [rad=%f,pos=%s]" padding rad (pos.ToString())

    | Line(p1, p2)     -> 
        printfn "%sLine [from=%s, to=%s" padding (p1.ToString()) (p2.ToString())

    | ChildScenes(cs)  -> 
        printfn "%sChildScenes..." padding
        cs |> List.iter (fun scene -> printGraph scene (tabDepth + 1))

printGraph graph1  0                         
printGraph compositeSceneGraph 0                         

let scene = 
    ChildScenes
        [
            Circle (0.5, Vector3 (0.0, 0.0, 0.0))
            Line (Vector3 (1.0, 2.0, 3.0), Vector3 (-3.5, 0.0, 42.0))
            ChildScenes
                [
                    Circle (1.0, Vector3 (5.0, 5.0, 5.0))
                    Circle (1.0, Vector3 (6.0, 5.0, 5.0))
                ] 
        ] 
        
printGraph scene 0


let rec visitGraph fCircle fLine graph =

    match graph with
    | Circle(circle) -> 
        fCircle circle

    | Line line     -> 
        fLine line
        
    | ChildScenes(cs)  -> 
        cs |> List.iter (fun scene -> visitGraph fCircle fLine scene)


compositeSceneGraph |> 
    visitGraph 
        (fun circle -> 
             let msg = sprintf "circle = %A!" circle
             System.Windows.Forms.MessageBox.Show msg |> ignore)                          
        (fun line -> 
             let msg = sprintf "line = %A!" line
             System.Windows.Forms.MessageBox.Show msg |> ignore)                          

let rec mapGraph fCircle fLine graph =

    match graph with
    | Circle(circle) -> 
        fCircle circle

    | Line line     -> 
        fLine line
        
    | ChildScenes(cs)  -> 
        ChildScenes(cs |> List.map (fun scene -> mapGraph fCircle fLine scene))
                          

compositeSceneGraph |> 
   mapGraph 
       (fun (radius,center)  ->  Circle(radius * 2.0, center))
       (fun (p1,p2) -> Circle(10.0,p1))
    


