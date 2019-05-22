using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MoreLinq.Extensions;

/// <summary>
/// Adapter Design Pattern
/// ----------------------
/// A construct which adapts an existing interface X to conform the
/// required interface Y.
///
/// Implementing an Adapter is easy: determine the API you have and the API you need
/// Create a component which aggregates (has a reference to, ...) the adaptee
/// Intermediate representations can pile up: use caching or other optimizations
/// </summary>
namespace Adapter
{
    public class Point
    {
        public int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        protected bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point) obj);
        }

        public override int GetHashCode() // we will need this one for caching
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }
    }

    // Suppose that in a different part of our app is built an actor for vector drawing
    public class Line
    {
        public Point Start, End;

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        protected bool Equals(Line other)
        {
            return Equals(Start, other.Start) && Equals(End, other.End);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Line) obj);
        }

        public override int GetHashCode() // we will need this one for caching
        {
            unchecked
            {
                return ((Start != null ? Start.GetHashCode() : 0) * 397) ^ (End != null ? End.GetHashCode() : 0);
            }
        }
    }
    // the previous idea is extended with the idea of graphic objects made out of vector collections
    public class VectorObject : Collection<Line> { }

    // This class is how someone has defined a Rectangle as a VectorObject
    public class VectorRectangle : VectorObject
    {
        public VectorRectangle(int x, int y, int width, int height)
        {
            Add(new Line(new Point(x, y), new Point(x + width, y)));
            Add(new Line(new Point(x + width, y), new Point(x + width, y + height)));
            Add(new Line(new Point(x, y), new Point(x, y + height)));
            Add(new Line(new Point(x, y + height), new Point(x + width, y + height)));
        }
    }

    /// <summary>
    /// Because we can only draw individual points but we've got the vectorObject functionality,
    /// we need an adaptor to do the job of transforming the vector objects into something that can
    /// be used by our app, because a line can be represented as a set of points
    /// </summary>
    public class LineToPointAdapter : IEnumerable<Point> // : Collection<Point> // with the cash this class is no longer a collection
    {
        private static int count; // number of invocations for the line to point

        // to solve the side effect of recalculating points, we will cash them based on their hashcode
        static Dictionary<int, List<Point>> cache = new Dictionary<int, List<Point>>();

        public LineToPointAdapter(Line line)
        {
            // first we check if the points already exists
            var hash = line.GetHashCode();
            if (cache.ContainsKey(hash)) return; // if they do, we skip the creation again

            var points = new List<Point>(); // we will use this list instead of adding the points to the vectorObject collection

            Console.WriteLine($"{++count}: Generating points for line [{line.Start.x}, {line.Start.y}]-[{line.End.x}, {line.End.y}]");
            int left = Math.Min(line.Start.x, line.End.x);
            int right = Math.Max(line.Start.x, line.End.x);
            int top = Math.Min(line.Start.y, line.End.y);
            int bottom = Math.Max(line.Start.y, line.End.y);
            int dx = right - left;
            int dy = line.End.y - line.Start.y;

            if (dx == 0)
            {
                for (int y = top; y <= bottom; y++)
                {
                    points.Add(new Point(left, y));
                }
            }
            else if (dy == 0)
            {
                for (int x = left; x <= right; x++)
                {
                    points.Add(new Point(x, top));
                }
            }

            cache.Add(hash, points);
        }

        // Now we need to expose all the points by becoming an IEnumerable

        public IEnumerator<Point> GetEnumerator()
        {   
            // SelectMany -> to flatten the list into a single list
            return cache.Values.SelectMany(x => x).GetEnumerator();
        }  

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class Program
    {
        // Let's consider we have a collection of vectorObjects
        private static readonly List<VectorObject> vectorObjects = new List<VectorObject>
        {
            new VectorRectangle(1, 1, 10, 10),
            new VectorRectangle(3, 3, 6, 6)
        };

        // This is the only piece of functionality we have
        public static void DrawPoint(Point p) // we can only draw individual pixels
        {
            // because the goal of this is to serve as example, the draw won't use coordinates
            Console.Write(".");
        }

        static void Main(string[] args)
        {
            // One side effect of the adapter is that you generate a lot of temp information
            // as we call the Draw method more than once we're regenerating some of the information
            // that has been already calculated
            Draw();
            Draw(); // With the cashing, the points won't be regenerated

            Console.ReadLine();
        }

        private static void Draw()
        {
            foreach (var vo in vectorObjects)
            {
                foreach (var line in vo)
                {
                    var adapter = new LineToPointAdapter(line);
                    adapter.ForEach(DrawPoint);
                }
            }
        }
    }
}
