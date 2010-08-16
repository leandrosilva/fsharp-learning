//---------------------------------------------------------------------------
// This is a demonstration script showing the basics of record types in F#.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 


// Simple records
type Point = { X : float; Y : float }

type MarsRover = 
    { 
        Location : Point
        Direction : float
        Velocity : float
    }

// Type inference infers rover
let isGoingBackwards rover = (rover.Velocity < 0.0)


