using System;

/// <summary>
/// Factories come to exist because constructors aren't that good
/// in the first bit of this class some problems will be exposed
///
/// A factory method is a static method that creates objects
/// A factory can take care of object creation
/// A factory can be external or reside inside the object as an inner class
/// Hierarchies of factories can be used to create related objects
/// </summary>
namespace Factories
{
    class Program
    {
        public class Point
        {
            private double x, y;  // coordinates


            // What if you want to start the point with polar coordinates?
            // I'm not able to have two constructors with the same params 
            /*public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }*/

            // Since this class wants to be able to use Polar or Cartesian coordinates, we must make the params neutral (a, b)
            public Point(double a, double b, CoordinateSystem system = CoordinateSystem.Cartesian)
            {
                switch (system)
                {
                    case CoordinateSystem.Cartesian:
                        x = a; // in this scenario a and b can cause confusion
                        y = b;
                        break;
                    case CoordinateSystem.Polar:
                        x = a * Math.Cos(b);
                        y = b * Math.Sin(b);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(system), system, null);
                }
            }

        }

        public enum CoordinateSystem
        {
            Cartesian, Polar
        }

        private static void Main(string[] args)
        {
            // Factory method example
            FactoryMethod.ExampleFactoryMethod();
            Console.ReadLine();
            // Abstract factory example
            AbstractFactory.ExampleOfAbstractFactory();
            Console.ReadLine();
        }
    }
}
