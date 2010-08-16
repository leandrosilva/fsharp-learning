//---------------------------------------------------------------------------
// This is a demonstration script showing the famous 'command' pattern when
// coded in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


/// Represents a 3-dimensional vector.
type Vector3 = Vector3 of float * float * float


[<Measure>]
type degs

/// Represents a Mars Rover in a simulation.
type MarsRover() =
    
    let mutable direction = 0.0<degs>
    let mutable velocity = Vector3(0.0, 0.0, 0.0)
    
    member this.Direction = direction
    member this.Velocity  = velocity
    member this.CrashedIntoRock  = false
     
    member this.Rotate (x : float<degs>) = (*...*) ()
    member this.Accelerate (x : float)   = (*...*) ()

type RoverCommand = Command of (MarsRover -> unit)

type MarsRover with 

    member this.AddCommandToQueue (cmd : RoverCommand) = (*...*) ()


// Use first-order functions as commands
let AccelerateCommand = Command( fun rover -> rover.Accelerate( 0.6)   )
let BreakCommand      = Command( fun rover -> rover.Accelerate(-1.0)   )
let TurnLeftCommand   = Command( fun rover -> rover.Rotate(-5.0<degs>) )
let TurnRightCommand  = Command( fun rover -> rover.Rotate( 5.0<degs>) )


type Direction = 
   | ToTheLeft 
   | ToTheRight 
   | StraightOn

type Destination = 
   | CanyonEdge 
   | CanyonBottom
   | BackHomeToEarth
   
let directionToDestination (rover: MarsRover) (destination: Destination) = 
   match destination with 
   | CanyonEdge -> ToTheLeft
   | _ -> ToTheRight

// Test code

let rover = MarsRover()
let destination = CanyonEdge

while not rover.CrashedIntoRock do
   
    match directionToDestination rover destination with
    | ToTheLeft  -> rover.AddCommandToQueue TurnLeftCommand
    | ToTheRight -> rover.AddCommandToQueue TurnRightCommand
    | StraightOn -> ()

