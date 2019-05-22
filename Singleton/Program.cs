using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;

/// <summary>
/// <title>Singleton Design Pattern</title>
/// <content>It is a component which is instantiated only once.
/// This pattern is a very controversial one since the use of it can
/// lead easily to errors.</content>
/// 
/// <motivation>
///     - For some components it only makes sense to have one in the system (database repository, object factory)
///     - When the constructor call is expensive, by using a singleton we only do it once, and we provide
///       everyone with the same instance.
///     - Want to prevent anyone creating additional copies
///     - Need to take care of lazy instantiation and thread safety
/// </motivation>
/// <summary>
///     Making a 'safe' singleton is easy: construct a static Lazy<T> and return its Value
///     Singletons are difficult to test
///     Instead of directly using a Singleton, consider depending on an abstraction(e.g. an interface)
///     Consider defining singleton lifetime in a DI container 
/// </summary>
/// </summary>
namespace Singleton
{
    public interface IDatabase
    {
        int GetPopulation(string name);
    }

    public class SingletonDatabase : IDatabase
    {
        private Dictionary<string, int> capitals;
        private static int instanceCount;
        public static int Count => instanceCount;


        private SingletonDatabase() // to avoid somebody to create a new instance, we must hide the constructor
        {
            instanceCount++; // how many times was the constructor called
            Console.WriteLine("Initializing database");

            capitals = File.ReadAllLines(
                    Path.Combine(new FileInfo(typeof(IDatabase).Assembly.Location).DirectoryName, "capitals.txt"))
                .Batch(2)
                .ToDictionary(
                    list => list.ElementAt(0).Trim(),
                    list => int.Parse(list.ElementAt(1))
                );
        }

        public int GetPopulation(string name)
        {
            return capitals[name];
        }

        // private static SingletonDatabase instance = new SingletonDatabase(); // we start the db

        // With lazy instantiation we avoid the use of resources unnecesary, we call it when need it
        private static Lazy<SingletonDatabase> instance = new Lazy<SingletonDatabase>(() => new SingletonDatabase());

        public static SingletonDatabase Instance => instance.Value; // we return the singleton db
    }

    /// <summary>
    /// This class will help us to test not the Db directly but a component who uses it
    /// it will shown the singleton problematic
    /// </summary>
    public class SingletonRecordFinder
    {
        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach (var name in names)
                result += SingletonDatabase.Instance.GetPopulation(name);
            return result;
        }
    }

    /// <summary>
    /// This class will serve as example on how to use a decoupled singleton
    /// </summary>
    public class ConfigurableRecordFinder
    {
        private IDatabase database;

        public ConfigurableRecordFinder(IDatabase database)
        {
            this.database = database;
        }

        public int GetTotalPopulation(IEnumerable<string> names)
        {
            int result = 0;
            foreach (var name in names)
                result += database.GetPopulation(name);
            return result;
        }
    }

    /// <summary>
    /// This class simulates a db to help us not to work with a production db
    /// </summary>
    public class DummyDatabase : IDatabase
    {
        public int GetPopulation(string name)
        {
            return new Dictionary<string, int>
            {
                ["alpha"] = 1,
                ["beta"] = 2,
                ["gamma"] = 3
            }[name];
        }
    }

    // This db is not a Singleton, but by using Dependency Injection it will be treated by our app as a Singleton
    public class OrdinaryDatabase : IDatabase
    {
        private Dictionary<string, int> capitals;


        private OrdinaryDatabase()
        {
            Console.WriteLine("Initializing database");

            capitals = File.ReadAllLines(
                    Path.Combine(new FileInfo(typeof(IDatabase).Assembly.Location).DirectoryName, "capitals.txt"))
                .Batch(2)
                .ToDictionary(
                    list => list.ElementAt(0).Trim(),
                    list => int.Parse(list.ElementAt(1))
                );
        }

        public int GetPopulation(string name)
        {
            return capitals[name];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var db = SingletonDatabase.Instance;
            var city = "Tokyo";
            Console.WriteLine($"{city} has population {db.GetPopulation(city)}");
            Console.WriteLine("---------------------------------------------------------");
            SingletonMonostatePattern.Example();
            Console.ReadLine();
        }
    }
}
