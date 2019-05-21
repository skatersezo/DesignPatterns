using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;

/// <summary>
/// Singleton Design Pattern
/// ------------------------
/// It is a component which is instantiated only once.
/// This pattern is a very controversial one since the use of it can
/// lead easily to errors.
/// Motivation
/// - For some components it only makes sense to have one in the system (database repository, object factory)
/// - When the constructor call is expensive, by using a singleton we only do it once, and we provide
///  everyone with the same instance.
/// - Want to prevent anyone creating additional copies
/// - Need to take care of lazy instantiation and thread safety
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

        private SingletonDatabase() // to avoid somebody to create a new instance, we must hide the constructor
        {
            Console.WriteLine("Initializing database");

            capitals = File.ReadAllLines("capitals.txt")
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

        // With lazy instantiation we avoid the use of resources unnecesary
        private static Lazy<SingletonDatabase> instance = new Lazy<SingletonDatabase>(() => new SingletonDatabase());

        public static SingletonDatabase Instance => instance.Value; // we return the singleton db
    }

    class Program
    {
        static void Main(string[] args)
        {
            var db = SingletonDatabase.Instance;
            var city = "Tokyo";
            Console.WriteLine($"{city} has population {db.GetPopulation(city)}");

            Console.ReadLine();
        }
    }
}
