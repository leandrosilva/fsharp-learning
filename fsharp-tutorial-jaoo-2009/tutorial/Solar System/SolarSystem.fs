//----------------------------------------------------------------------------
// A Simple Solar SYstem Simulator, using Units of Measure
//
// Copyright (c) Microsoft Corporation 2005-2008.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 
//----------------------------------------------------------------------------



open System
open System.Windows.Forms
open System.Drawing
open Microsoft.FSharp.Control.CommonExtensions

//-----------------------------------------------
// Graphics System

// Define a special type of form that doesn't flicker
type SmoothForm() as x = 
    inherit Form()
    do x.DoubleBuffered <- true
        
let form = new SmoothForm(Text="F# Solar System Simulator",  Visible=true, TopMost=true, Width=500, Height=500)

type IPaintObject = 
    abstract Paint : Graphics -> unit
    
// Keep a list of objects to draw
let paintObjects = new ResizeArray<IPaintObject>()

form.Paint.Add (fun args -> 

        let g = args.Graphics
        // Clear the form
        g.Clear(color=Color.Blue)

        // Draw the paint objects
        for paintObject in paintObjects do
           paintObject.Paint(g)

        // Invalidate the form again in 10 milliseconds to get continuous update
        async { do! Async.Sleep 10
                form.Invalidate() } |> Async.Start
   )

// Set things going with an initial Invaldiate
form.Invalidate()

//-----------------------------------------------

let G = 6.67e-11
let m_per_AU = 149597870691.0
let AU_per_m = 1.0/m_per_AU
let Pixels_per_AU = 200.0
let m_per_km = 1000.0
let AU_per_km = m_per_km * AU_per_m 

// Make 5 seconds into one year
let sec_per_year = 60.0 * 60.0 * 24.0 * 365.0

// One second of real time is 1/40th of a year of model time
let realTimeToModelTime (x:float) = float x * sec_per_year / 80.0

let pixels (x:float) = int32 x


type Planet(ipx:float,ipy:float,
            ivx:float,ivy:float,
            brush:Brush,mass:float,
            width,height) =

    // For this sample e store the simulation state directly in the object
    let mutable px = ipx
    let mutable py = ipy
    let mutable vx = ivx
    let mutable vy = ivy
    
    member p.Mass = mass
    member p.X with get() = px and set(v) = (px <- v)
    member p.Y with get() = py and set(v) = (py <- v)
    member p.VX with get() = vx and set(v) = (vx <- v)
    member p.VY with get() = vy and set(v) = (vy <- v)
    
    interface IPaintObject with
              member obj.Paint(g) =
             
                 let rect = Rectangle(x=pixels (px * Pixels_per_AU)-width/2,
                                      y=pixels (py * Pixels_per_AU)-height/2,
                                      width=width,height=height)               
                 g.FillEllipse(brush,rect)
                                                   

type Simulator() = 
    // Get the start time for the animation
    let startTime = System.DateTime.Now
    let lastTimeOption = ref None

    let ComputeGravitationalAcceleration (obj:Planet) (obj2:Planet) = 
        let dx = (obj2.X-obj.X)*m_per_AU
        let dy = (obj2.Y-obj.Y)*m_per_AU
        let d2 = (dx*dx) + (dy*dy)
        let d = sqrt d2
        let g = obj.Mass * obj2.Mass * G /d2
        let ax = (dx / d) * g / obj.Mass
        let ay = (dy / d) * g / obj.Mass
        ax,ay

    /// Find all the gravitational objects in the system except the given object
    let FindObjects(obj) = 
        [ for paintObject in paintObjects do 
              match paintObject with
              | :? Planet as p2 when p2 <> obj -> 
                  yield p2
              | _ -> 
                  yield! [] ]

    member sim.Step(time:TimeSpan) = 
        match !lastTimeOption with
        | None -> ()
        | Some(lastTime) -> 
           for paintObject in paintObjects do
               match paintObject with
               | :? Planet as obj -> 
                   let timeStep = (time - lastTime).TotalSeconds |>  realTimeToModelTime
                   obj.X <- obj.X + timeStep * obj.VX
                   obj.Y <- obj.Y + timeStep * obj.VY

                   // Find all the gravitational objects in the system
                   let objects = FindObjects(obj)

                   // For each object, apply its gravitational field to this object
                   for obj2 in objects do 
                       let (ax,ay) = ComputeGravitationalAcceleration obj obj2
                       obj.VX <- obj.VX + timeStep * ax * AU_per_m 
                       obj.VY <- obj.VY + timeStep * ay * AU_per_m 
               | _ ->  ()

        lastTimeOption := Some time 

    member sim.Start() = 
        async { while true do 
                   let time = System.DateTime.Now - startTime
                   // Sleep a little to give better GUI updates
                   do! Async.Sleep 1 
                   sim.Step(time) } 
        |> Async.Start

let s = Simulator().Start() 

let massOfEarth   = 5.9742e24
let massOfMoon    = 7.3477e22
let massOfMercury = 3.3022e23
let massOfVenus   = 4.8685e24
let massOfSun     = 1.98892e30

let mercuryDistanceFromSun  = 57910000.0 * AU_per_km
let venusDistanceFromSun    = 0.723332
let distanceFromMoonToEarth =384403.0 * AU_per_km

let orbitalSpeedOfMoon   = 1.023 * AU_per_km
let orbitalSpeedOfMecury = 47.87 * AU_per_km
let orbitalSpeedOfVenus  = 35.02 * AU_per_km
let orbitalSpeedOfEarth  = 29.8  * AU_per_km 

let sun   = new Planet(ipx=1.1,                        
                       ipy=1.1,
                       ivx=0.0,
                       ivy=0.0,
                       brush=Brushes.Yellow,
                       mass=massOfSun,
                       width=20,
                       height=20)

let mercury = new Planet(ipx=sun.X+mercuryDistanceFromSun,
                       ipy=sun.Y,
                       ivx=0.0,
                       ivy=orbitalSpeedOfMecury,
                       brush=Brushes.Goldenrod,
                       mass=massOfMercury,
                       width=10,
                       height=10)

let venus = new Planet(ipx=sun.X+venusDistanceFromSun,
                       ipy=sun.Y,
                       ivx=0.0,
                       ivy=orbitalSpeedOfVenus,
                       brush=Brushes.BlanchedAlmond,
                       mass=massOfVenus,
                       width=10,
                       height=10)

let earth = new Planet(ipx=sun.X+1.0,
                       ipy=sun.Y,
                       ivx=0.0,
                       ivy=orbitalSpeedOfEarth,
                       brush=Brushes.Green,
                       mass=massOfEarth,
                       width=10,
                       height=10)

let moon  = new Planet(ipx=earth.X+distanceFromMoonToEarth,
                       ipy=earth.Y,
                       ivx=earth.VX,
                       ivy=earth.VY+orbitalSpeedOfMoon,
                       brush=Brushes.White,
                       mass=massOfMoon,
                       width=2,
                       height=2)

paintObjects.Add(sun)
paintObjects.Add(mercury)
paintObjects.Add(venus)
paintObjects.Add(earth)
paintObjects.Add(moon)

form.Show()

#if COMPILED
[<STAThread>]
do Application.Run(form)
#endif
