using System;

/// <summary>
/// Bridge Pattern
/// --------------
/// It is a structural design pattern.
/// A mechanism that decouples an interface (hierarchy) from
/// an implementation (hierarchy).
/// </summary>
namespace Bridge
{
    // Let's pretend we're rendering objects, and we can do it by a vector or a raster

    public interface IRenderer
    {
        void RenderCircle(float radius);
    }

    public class VectorRenderer : IRenderer
    {
        public void RenderCircle(float radius)
        {
            Console.WriteLine($"Drawing a circle of radius {radius}");
        }
    }

    public class RasterRenderer : IRenderer
    {
        public void RenderCircle(float radius)
        {
            Console.WriteLine($"Drawing pixels for a circle with radius {radius}");
        }
    }

    // Circle is only one example of a shape

    // Shape abstract class represents the top of the hierarchy for the objects
    public abstract class Shape // HERE is where the bridging happens
    {
        // instead of specify that the shape is able to draw itself as either a vector or a raster
        // you don't put this limitation in play, so we don't let shape decide in which way it can be drawn

        protected IRenderer renderer; // IRenderer will be the bridge between the shape and whoever is rendering it

        protected Shape(IRenderer renderer)
        {
            this.renderer = renderer;
        }

        public abstract void Draw();
        public abstract void Resize(float factor);
    }

    public class Circle : Shape
    {
        private float radius;

        public Circle(IRenderer renderer, float radius) : base(renderer)
        {
            this.radius = radius;
        }

        public override void Draw()
        {
            // here we're using the bridge to draw the shape
            renderer.RenderCircle(radius);
        }

        public override void Resize(float factor)
        {
            radius *= factor;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // IRenderer renderer = new RasterRenderer();
            var renderer = new VectorRenderer();
            var circle = new Circle(renderer, 5); // here we see the connection between the shape and the renderer

            circle.Draw();
            circle.Resize(2);
            circle.Draw();

            Console.ReadLine();
        }
    }
}
