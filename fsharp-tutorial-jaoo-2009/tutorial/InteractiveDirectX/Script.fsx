//---------------------------------------------------------------------------
// This is a demonstration script showing a 3D interactive visualization using
// the Microsoft Managed DirectX API.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// Adjust these as needed for your latest installed version of ManagedDirectX
#I @"C:\WINDOWS\Microsoft.NET\DirectX for Managed Code\1.0.2902.0" 

#r @"System.Core.dll"
#r @"Microsoft.DirectX.dll"
#r @"Microsoft.DirectX.Direct3D.dll" 
#r @"Microsoft.DirectX.Direct3Dx.dll" 

#load @"dxlib.fs"

open System
open System.Drawing
open System.Windows.Forms
open Microsoft.DirectX
open Microsoft.DirectX.Direct3D
open Microsoft.FSharp.Control.CommonExtensions
open Sample.DirectX
open Sample.DirectX.MathOps
open Sample.DirectX.VectorOps

// Create a form and set some properties

let form = new SmoothForm(Visible = true, TopMost = true, 
                          Text = "F# surface plot",
                          ClientSize = Size(600,400),
                          FormBorderStyle=FormBorderStyle.FixedSingle)

// Host a DirectX renderer for the unit cube on the form

let renderer = new DirectXRenderer(form)

renderer.DrawScene.Add(fun _ -> renderer.DrawCubeAxis())

renderer.DrawScene.Add(fun _ -> renderer.SetupLights())

// Set the view

renderer.View <- 
   { YawPitchRoll   = Matrix.RotationYawPitchRoll(0.0f,0.0f,0.0f);
     Focus          = scale 0.5f (X1 + Y1 + Z1);
     Zoom           = 4.0 }

// Adjust the view in responce to mouse movements

form.MousePositions.Add (fun (event1, event2) -> 
    renderer.View <- 
        let dx = event2.X - event1.X
        let dy = event2.Y - event1.Y
        match event2.Button, Form.ModifierKeys with 
        | MouseButtons.Left, Keys.Shift -> renderer.View.AdjustZoom(dx,dy)
        | MouseButtons.Left, _          -> renderer.View.AdjustYawPitchRoll(dx,dy)
        | _                             -> renderer.View.AdjustFocus(dx,dy))

//----------------------------------------------------------------------------
// Draw a surface
//----------------------------------------------------------------------------

renderer.Surface <- (fun t x y -> x * (1.0 - y))

renderer.Surface <- (fun t x y -> sqr (x - 0.5) * sqr (y - 0.5) * 16.0)

renderer.Surface <- (fun t x y -> 0.5 * sin(x * 4.5 + t / 2.0) * cos(y * 8.0) * x + 0.5)

renderer.Mesh <- BaseMesh.Grid(50, 50)
renderer.Mesh <- BaseMesh.Grid(20, 20)
    
let ripple t x y =
   let x,y = x - 0.5,y - 0.5 
   let r = sqrt (x*x + y*y) 
   exp(-5.0 * r) * sin(6.0 * pi * r + t) + 0.5

renderer.Surface <- ripple

// PART 4 - Simulate motion on a surface 

let surfacePoint (f : surface) x y = 
    Vector3(float32 x, float32 y,float32 (f x y))

let surfaceNormal f x y =
    let dx,dy = 0.01,0.01 
    let pA    = surfacePoint f x y 
    let pAdx = surfacePoint f (x+dx) y - pA 
    let pAdy = surfacePoint f x (y+dy) - pA 
    normalize (cross pAdx pAdy)

let gravity = Vector3(0.0f,0.0f,-9.81f)

// A ball is a tuple of position/velocity vectors
type Ball = Ball of Vector3 * Vector3 

// Display settings: how big is the torus we use to render it?
let radiusA = 0.010 
let radiusB = 0.005    
let timeDelta = 0.008

// This function computes a new Ball give the surface and a time
// interval for the simulation
let moveBall surface (Ball (position,velocity)) =

    // First compute the surface plane normal   
    let nHat     = surfaceNormal surface (float position.X) (float position.Y)  

    // Now project acceleration and velocity down the plane
    let acceleration = planeProject nHat gravity              // acceleration in plane 
    let velocity     = planeProject nHat velocity             // velocity     in plane 
    
    // Compute the new position
    let newPosition = position + Vector3.Scale(velocity,    float32 timeDelta)  // iterate 
    let newVelocity = velocity + Vector3.Scale(acceleration,float32 timeDelta)  // iterate 

    // Handle the bounce!
    let bounce (p,v) =                                        
        if   (p < 0.0 + radiusA) then (2.0 * (0.0 + radiusA) - p,-v) 
        elif (p > 1.0 - radiusA) then (2.0 * (1.0 - radiusA) - p,-v) 
        else                          (p,v)  
    let px,vx = bounce (float newPosition.X, float newVelocity.X) // bounce X edges 
    let py,vy = bounce (float newPosition.Y, float newVelocity.Y) // bounce Y edges 
    let newPosition = surfacePoint surface px py                  // keep to surface 
    let newVelocity = Vector3 (float32 vx, float32 vy,newVelocity.Z) 
    let newVelocity = planeProject nHat newVelocity               // velocity in plane     

    // We're done!
    Ball (newPosition,newVelocity)

//----------------------------------------------------------------------------
// PART 5 - Motion on a surface rendered
//----------------------------------------------------------------------------

let drawBall t (Ball (position,v)) =
    let norm  = surfaceNormal (renderer.Surface t) (float position.X) (float position.Y) 
    // position XY-projection, unit velocity XY-projection 
    let p0   = Vector3(position.X,position.Y,0.0f) 
    let pV   = Vector3(v.X, v.Y, 0.0f)                  
    let pVxZ = Vector3.Cross(pV, Z1)                         
    // velocity arrow on floor, normal arrow at point, torus for the ball
    renderer.DrawArrows(norm, position, p0, pV)
    renderer.DrawTorus(norm, position, radiusA, radiusB)


// The simulation runs as an agent. Requests to add balls and step
// the simulation are added to the queue.
type Message = 
    | PleaseTakeOneStep 
    | PleaseAddOneBall of Ball

let state = SimulationState<Ball>()

let simulationEngine = 
    Agent.Start(fun inbox -> 
        async { while true do 
                    // Wait for a message
                    let! msg = inbox.Receive()

                    match msg with 
                    | PleaseTakeOneStep -> 
                        state.Transform (moveBall (renderer.Surface (now()))) 

                    | PleaseAddOneBall ball -> 
                        state.AddObject ball 
                       })

// In this demo, we step the simulation once for each rendering of the display
renderer.DrawScene.Add(fun _ -> 
    simulationEngine.Post PleaseTakeOneStep)

renderer.DrawScene.Add(fun _ -> state.IterateObjects (drawBall (now())))


//----------------------------------------------------------------------------
// OK, we're ready to go. First, set up a bowl shaped area 

let bowl t x y = 
     let f phi u = ((1.0 + cos(2.0 * pi * u + phi )) / 2.0) 
     (f t x * f 0.0 y + 1.0) / 2.0

renderer.Surface    <- (fun t -> bowl 0.0)


// Second, add a ball 
let ballInTopLeftCorner = Ball (Vector3(0.1f,0.1f,0.1f),
                                Vector3(0.6f,0.5f,0.0f))

simulationEngine.Post (PleaseAddOneBall ballInTopLeftCorner)

// Add a ball train. 
//
// This starts an async agent that posts a request to add a 
// ball every 100 milliseconds

Async.Start 
    (async { for i in 0 .. 6 do 
                simulationEngine.Post (PleaseAddOneBall ballInTopLeftCorner)
                do! Async.Sleep(100)  })

// Now move the floor!

renderer.Surface <- (fun t x y -> bowl (2.0 * t) x y)

#if COMPILED
[<STAThread>]
do Application.Run(form)

do Application.Exit()
#endif

