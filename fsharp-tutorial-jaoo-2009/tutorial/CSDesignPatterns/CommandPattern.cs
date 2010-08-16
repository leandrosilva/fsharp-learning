using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSDesignPatterns
{
    #region "Setup"
    class Vector
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    class MarsRover
    {
        public float Direction { get; set; }
        public Vector Velocity { get; set; }

        public void Rotate(float rotation) { }
        public void Accelerate(float intensity) { }
    }
    #endregion

    #region "Command Pattern"
    
    abstract class Command
    {
        public abstract void Execute();
    }

    abstract class MarsRoverCommand : Command
    {
        protected MarsRover Rover { get; private set; }

        public MarsRoverCommand(MarsRover rover)
        {
            this.Rover = rover;
        }
    }

    class TurnLeftCommand : MarsRoverCommand
    {
        public TurnLeftCommand(MarsRover rover)
            : base(rover)
        {
        }

        public override void Execute()
        {
            Rover.Rotate(-5.0f);
        }
    }

    class TurnRightCommand : MarsRoverCommand
    {
        public TurnRightCommand(MarsRover rover)
            : base(rover)
        {
        }

        public override void Execute()
        {
            Rover.Rotate( 5.0f);
        }
    }

    class AccelerateCommand : MarsRoverCommand
    {
        public AccelerateCommand(MarsRover rover)
            : base(rover)
        {
        }

        public override void Execute()
        {
            Rover.Accelerate(0.6f);
        }
    }

    class BrakeCommand : MarsRoverCommand
    {
        public BrakeCommand(MarsRover rover)
            : base(rover)
        {
        }

        public override void Execute()
        {
            Rover.Accelerate(-1.0f);
        }
    }

    /*

     ... inside the NasaCommandCenter type ...
     
     if (TelemetryData.LastSpeedReading > MaxSpeed)
        CommunicationsWidget.SendCommand(new BrakeCommand())

    */
    #endregion
}
