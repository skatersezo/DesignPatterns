using System;
using System.Collections.Generic;
/// <summary>
/// The Open-Close Principle
/// ------------------------
/// Classes should be open for extension but closed for modification
/// </summary>
namespace OpenClosePrinciple
{
    public enum Color
    {
        Red, Green, Blue
    }

    public enum Size
    {
        Small, Medium, Large, XLarge
    }

    public class Product
    {
        public string Name;
        public Color Color;
        public Size Size;

        public Product(string name, Color color, Size size)
        {
            Name = name;
            Color = color;
            Size = size;
        }
    }

    /// <summary>
    /// Class ProductFilter
    /// This class claims for modifications every time we want to filter by new params.
    /// By using inheritance we will avoid modifications in this class.
    /// </summary>
    public class ProductFilter
    {
        public IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
        {
            foreach (var product in products)
            {
                if (product.Size == size)
                {
                    yield return product;
                }
            }
        }

        public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
        {
            foreach (var product in products)
            {
                if (product.Color == color)
                {
                    yield return product;
                }
            }
        }

        public IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> products, Size size, Color color)
        {
            foreach (var product in products)
            {
                if (product.Color == color && product.Size == size)
                {
                    yield return product;
                }
            }
        }
    }

    // Enterprise pattern used -> Specification pattern

    /// <summary>
    /// This interface implements the specification pattern which dictates whether or not
    /// a products satisfies a particular criteria
    /// </summary>
    public interface ISpecification<T> // By using T i allow this class to operate with any kind of type
    {
        bool IsSatisfied(T t);
    }

    public interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
    }

    public class ColorSpecification : ISpecification<Product>
    {
        private Color Color;

        public ColorSpecification(Color color)
        {
            this.Color = color;
        }
        public bool IsSatisfied(Product t)
        {
            return t.Color == Color;
        }
    }

    public class SizeSpecification : ISpecification<Product>
    {
        private Size Size;

        public SizeSpecification(Size size)
        {
            this.Size = size;
        }

        public bool IsSatisfied(Product t)
        {
            return t.Size == Size;
        }
    }

    public class AndSpecification<T> : ISpecification<T>
    {
        private ISpecification<T> first, second;

        public AndSpecification(ISpecification<T> first, ISpecification<T> second)
        {
            this.first = first;
            this.second = second;
        }

        public bool IsSatisfied(T t)
        {
            return first.IsSatisfied(t) && second.IsSatisfied(t);
        }
    }

    public class BetterFilter : IFilter<Product>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
        {
            foreach (var product in items)
            {
                if (spec.IsSatisfied(product))
                    yield return product;
            }
        }

        // You shouldn't need to come here and add more code, but instead create a new class with the ISpecification interface
    }

    class Program
    {
        static void Main(string[] args)
        {
            var apple = new Product("Apple", Color.Green, Size.Small);
            var tree = new Product("Tree", Color.Green, Size.Large);
            var house = new Product("House", Color.Blue, Size.Large);

            Product[] products = {apple, tree, house};

            var productFilter = new ProductFilter();
            Console.WriteLine("Green products (old):");
            foreach (var product in productFilter.FilterByColor(products, Color.Green))
            {
                Console.WriteLine($"- {product.Name} is green");
            }

            // ---------------------------------------------------------------------------

            var betterFilter = new BetterFilter();
            Console.WriteLine("Green products (new):");
            foreach (var product in betterFilter.Filter(products, new ColorSpecification(Color.Green)))
            {
                Console.WriteLine($"- {product.Name} is green");
            }

            // -----------------------------------------------------------------------------

            Console.WriteLine("Large blue items");
            foreach (var product in betterFilter.Filter(products, new AndSpecification<Product>(new ColorSpecification(Color.Blue), new SizeSpecification(Size.Large))))
            {
                Console.WriteLine($"- {product.Name} is big and blue.");
            }

            Console.ReadLine();
        }
    }
}
