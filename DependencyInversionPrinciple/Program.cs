using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

/// <summary>
/// Dependency Inversion Principle
/// ------------------------------
/// It presents the idea that high level parts of the system
/// should not depend on low level parts directly, instead
/// they should depend on some kind of abstractions
/// </summary>
namespace DependencyInversionPrinciple
{
    // This system performs querys in a genealogy database

    public enum Relationship
    {
        Parent,
        Child,
        Sibling
    }

    public class Person
    {
        public string Name;
        public DateTime DateOfBirth;
    }

    /// <summary>
    /// IRelationshipBrowser is the abstration that we use to access the low level module
    /// </summary>
    public interface IRelationshipBrowser
    {
        IEnumerable<Person> FindAllChildren(string name);
    }

    // low-level
    public class Relationships : IRelationshipBrowser 
    {
        // (Person from which the relationship stands, the relation itself, and the person which applies)
        private List<(Person, Relationship, Person)> relations = new List<(Person, Relationship, Person)>();

        public void AddParentAndChild(Person parent, Person child)
        {
            relations.Add((parent, Relationship.Parent, child));
            relations.Add((child, Relationship.Child, parent));
        }

        public IEnumerable<Person> FindAllChildren(string name)
        {
            return relations.Where(
                x => x.Item1.Name == name && x.Item2 == Relationship.Parent).Select(relations => relations.Item3);
        }

        // public List<(Person, Relationship, Person)> Relations => relations;
    }

    // Above the code to perform research on the genealogy

    public class Research
    {
//        public Research(Relationships relationship)
//        {
//            var relations = relationship.Relations; // Here we're accesing a very low part of the Relationships class
//            foreach (var r in relations.Where(
//                x => x.Item1.Name == "John" && x.Item2 == Relationship.Parent))
//            {
//                Console.WriteLine($"John has a child called {r.Item3.Name}");
//            }
//        }

        public Research(IRelationshipBrowser browser)
        {
            foreach (var p in browser.FindAllChildren("John"))
            {
                Console.WriteLine($"John has a child called {p.Name}");
            }
        }
        static void Main(string[] args)
        {
            var parent = new Person{Name = "John"};
            var child1 = new Person {Name = "Chris"};
            var child2 = new Person {Name = "Mary"};

            var relationships = new Relationships();
            relationships.AddParentAndChild(parent, child1);
            relationships.AddParentAndChild(parent, child2);

            new Research(relationships);

            Console.ReadLine();
        }
    }
}
