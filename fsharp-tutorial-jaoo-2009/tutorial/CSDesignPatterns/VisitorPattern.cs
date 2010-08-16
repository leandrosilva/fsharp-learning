using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSDesignPatterns
{
    #region "Setup"
    interface IVisitor
    {
        void VisitSphere(Sphere c);
        void VisitLine(Line l);
        void VisitScene(Graph children);
    }

    interface IVisitable
    {
       void Accept(IVisitor visitor);
    }


    class SceneGraphVisitor : IVisitor
    {

        public void VisitSphere(Sphere c)
        {
            Console.WriteLine("Sphere...");
        }

        public void VisitLine(Line l)
        {
            Console.WriteLine("Line...");
        }

        public void VisitScene(Graph children)
        {
            Console.WriteLine("Visiting Scene...");
        }
    }
    #endregion

    #region "Implementation"
    class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    class SceneGraphNode { }

    class Sphere : SceneGraphNode, IVisitable
    {
        public float Radius { get; set; }
        public Vector3 Location { get; set; }

        public void Accept(IVisitor visitor)
        {
            visitor.VisitSphere(this);
        }
    }

    class Line : SceneGraphNode, IVisitable
    {
        public Vector3 Point1 { get; set; }
        public Vector3 Point2 { get; set; }

        public void Accept(IVisitor visitor)
        {
            visitor.VisitLine(this);
        }
    }

    class Graph : SceneGraphNode, IVisitable
    {
        Graph[] ChildGraphs { get; set; }

        public void Accept(IVisitor visitor)
        {
            visitor.VisitScene(this);

            foreach (IVisitable childScene in this.ChildGraphs)
                childScene.Accept(visitor);
        }
    }
    #endregion
}
