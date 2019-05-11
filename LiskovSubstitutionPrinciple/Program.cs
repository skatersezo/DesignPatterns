using System;

/// <summary>
/// The Liskov Substitution Principle
/// ---------------------------------
/// You should be able to substitute a base type for a subtype
/// </summary>
namespace LiskovSubstitutionPrinciple
{
    public class Rectangle
    {
        // public int Width { get; set; }
        // public int Height { get; set; }

        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public Rectangle() {}

        public Rectangle(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}";
        }
    }

    public class Square : Rectangle
    {
        // To use "new" keyword violate the Liskov principle since a Square won't
        // behave as supposed when it is cast to his base object
        //  public new int Width
        //  {
        //     set { base.Width = base.Height = value;  }
        //  }
        //
        //  public new int Height
        //  {
        //     set { base.Width = base.Height = value; }
        //  }

        public override int Width
        {
            set { base.Width = base.Height = value;  }
        }

        public override int Height
        {
            set { base.Width = base.Height = value; }
        }
    }

    class Program
    {
        public static int Area(Rectangle r) => r.Width * r.Height;

        static void Main(string[] args)
        {
            Rectangle rec = new Rectangle(2, 3);
            Console.WriteLine($"{rec} has area {Area(rec)}");

            // Square sqa = new Square();
            Rectangle sqa = new Square(); // I should be able to change Square for Rectangle, but then it breaks
            sqa.Width = 4; // now when we access the width it knows (because of the virtual keyword) that we are referencing the square
            Console.WriteLine($"{sqa} has area {Area(sqa)}");

            Console.ReadLine();

        }
    }
}
