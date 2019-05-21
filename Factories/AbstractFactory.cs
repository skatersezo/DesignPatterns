using System;
using System.Collections.Generic;

/// <summary>
/// In an abstract factory you're not returning the types you're creating.
/// You're returning abstract classes or interfaces.
/// Some literature says that abstract factories are supposing to return families
/// of objects, but that's not necessarily true.
/// In this example we're going to create two different objects with two different
/// factories.
/// </summary>
namespace Factories
{
    public interface IHotDrink // We want to give the user the ability to consume a drink without holding the actual type
    {
        void Consume();
    }

    // the following classes are internal, no factory will return this types, instead they will return an IHotDrink
    internal class Tea : IHotDrink 
    {
        public void Consume()
        {
            Console.WriteLine("This tea is nice, but where's the milk?");
        }
    }

    internal class Coffee : IHotDrink
    {
        public void Consume()
        {
            Console.WriteLine("This coffee is really good!");
        }
    }

    // Now we want two different factories for Tea and Coffee
    // for whatever reason (complex implementation, different process, different kind of teas and coffees etc.)
    // Instead of creating one HotDrinkFactory class, we will create an interface
    public interface IHotDrinkFactory
    {
        IHotDrink Prepare(int amount);
    }

    // the next following factories are internal too, we won't give it to anyone

    internal class TeaFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"Put in a tea bag, boil water, pour {amount} ml, add lemon. Enjoy!");
            return new Tea(); // normally, the object will be customized in any way
        }
    }

    internal class CoffeeFactory : IHotDrinkFactory
    {
        public IHotDrink Prepare(int amount)
        {
            Console.WriteLine($"Grind the beans, pour {amount} ml of water, add cream an no sugar!!!");
            return new Coffee();
        }
    }

    // Now we have the ability to define a HotDrinkMachine for the user to get the drink he/she wants

    public class HotDrinkMachine
    {
        public enum AvailableDrink // We need to specify the drinks that are available
        {
            Coffee, Tea // It can be a list instead of an enum
        } // *In the first approach we introduced this Enum, but since it breaks the Open-Closed principle, we need a different approach 

        // We will have a bunch of factories, and this is where the abstractness of the factory comes into play
        // because if you have an interface or an abstract class you can make a list or a dictionary of those things

        // We have a dictionary that maps from the Enum the available drinks to return the corresponding factory
        private Dictionary<AvailableDrink, IHotDrinkFactory> factories = new Dictionary<AvailableDrink, IHotDrinkFactory>();

        public HotDrinkMachine()
        {
            foreach (AvailableDrink drink in Enum.GetValues(typeof(AvailableDrink)))
            {
                var factory = (IHotDrinkFactory) Activator.CreateInstance( // We need to use CreateInstance() because we're going to instantiate by name
                    Type.GetType("Factories." + Enum.GetName(typeof(AvailableDrink), drink) + "Factory")
                    );
                factories.Add(drink, factory);
            }
        }

        public IHotDrink MakeDrink(AvailableDrink drink, int amount)
        {
            return factories[drink].Prepare(amount);
        }
    }

    /// <summary>
    /// This implementation don't violate the open-closed principle and presents a interactive HotDrinkMachine
    /// by using reflection.
    /// We will find every type in the current assembly which implements an IHotDrinkFactory.
    /// Typically you'll do this with a dependency injection container rather than reflection, but in this example
    /// we will use reflection to show how the process works
    /// </summary>
    public class BetterHotDrinkMachine
    {
        private List<Tuple<string, IHotDrinkFactory>> factories = new List<Tuple<string, IHotDrinkFactory>>();

        public BetterHotDrinkMachine()
        {
            foreach (var t in typeof(HotDrinkMachine).Assembly.GetTypes())
            {
                // here we check if a particular type implements a particular interface, and also not to get the interface itself
                if (typeof(IHotDrinkFactory).IsAssignableFrom(t) && !t.IsInterface)
                {
                    // this approach will inspect all the factories in the assembly and add them to the list
                    factories.Add(Tuple.Create(
                        t.Name.Replace("Factory", string.Empty), 
                        (IHotDrinkFactory)Activator.CreateInstance(t)));
                }
            }
        }

        public IHotDrink MakeDrink()
        {
            Console.WriteLine("Available drinks:");
            for (int index = 0; index < factories.Count; index++)
            {
                var tuple = factories[index];
                Console.WriteLine($"{index}: {tuple.Item1}");
            }

            while (true)
            {
                string userStringInput;
                if ((userStringInput = Console.ReadLine()) != null 
                    && int.TryParse(userStringInput, out int selectedDrink) 
                    && selectedDrink >= 0
                    && selectedDrink < factories.Count)
                {
                    Console.Write("Specify amount: ");
                    userStringInput = Console.ReadLine();
                    if (userStringInput != null
                        && int.TryParse(userStringInput, out int amount)
                        && amount > 0)
                    {
                        return factories[selectedDrink].Item2.Prepare(amount);
                    }
                }

                Console.WriteLine("Incorrect, try again");
            }
        }

    }

    class AbstractFactory
    {
        public static void ExampleOfAbstractFactory()
        {
            // var machine = new HotDrinkMachine();
            // var drink = machine.MakeDrink(HotDrinkMachine.AvailableDrink.Coffee, 100);
            // drink.Consume();

            var machine = new BetterHotDrinkMachine();
            var drink = machine.MakeDrink();
            drink.Consume();
        }

    }
}
