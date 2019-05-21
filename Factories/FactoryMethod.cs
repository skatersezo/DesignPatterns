using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Factories
{
    public class Point2
    {
        // the factory method
        /*public static Point2 NewCartesianPoint(double x, double y) // the key of the factory method is that the construction isn't tight to the class name
        {
            return new Point2(x, y);
        }*/ // moved to a seperated class

        private double x, y;

        // by making the constructor 'internal' it can no longer be accessed from outside of the namespace, so the users should use the factory
        internal Point2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
        }

        // This can be confusing, the first one initializes a transient while the second initializes a singleton
        public static Point2 Origin => new Point2(0, 0);
        public static Point2 Origin2 = new Point2(0, 0);

        public static class Factory
        {
            public static Point2 NewCartesianPoint(double x, double y) // the key of the factory method is that the construction isn't tight to the class name
            {
                return new Point2(x, y);
            }

            public static Point2 NewPolarPoint(double rho, double theta)
            {
                return new Point2(rho * Math.Cos(theta), rho * Math.Sin(theta));
            }
        }
    }

    // According to the Single Responsability Principle and the separation of concerns, the construction
    // of an object is a separated responsability of what the object actually does, and a separate class should be in charge of it


    class FactoryMethod
    {
        public static void ExampleFactoryMethod()
        {
            var point = Point2.Factory.NewPolarPoint(1.0, Math.PI / 2);
            Console.WriteLine(point);

            // Task.Factory.StartNew(params); // This is the factory method applied by .NET Framework
        }
    }
}
