using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

/// <summary>
/// Prototype design pattern
/// ------------------------
/// A partially or fully initialized object that you copy (clone) and
/// make use of.
///
/// Complicated objects(e.g. cars) aren't designed from scratch: they reiterate existing designs
/// An existing (partially or fully constructed design is a Prototype
/// We make a copy (clone) the prototype and customize it (requires 'deep copy' support)
/// We make the cloning convenient (e.g. via a Factory)
///
/// This approach saves costly resources and time, especially when the object creation is a heavy process.
/// </summary>
namespace Prototype
{
    // [Serializable]
    public class Person // : IPrototype<Person> // : ICloneable 
        
    {
        public string[] Names;
        public Address Address;

        public Person()
        {
            
        }

        public Person(string[] names, Address address)
        {
            Names = names;
            Address = address;
        }

        public override string ToString()
        {
            return $"{nameof(Names)}: {string.Join(" ", Names)}, {nameof(Address)}-> {Address}";
        }

        // to use ICloneable is not the best way to go because is badly specified and it returns an object instead of something strongly typed
        /*public object Clone() 
        {
            //return new Person(Names, Address); // shallow copy
            return new Person(Names, (Address) Address.Clone()); // this approach will fix problems, but is still not the ideal one
        }*/

        // Another approach to this problem is to use copy constructors, which is a term from C++
        public Person(Person other)
        {
            Names = other.Names;
            Address = new Address(other.Address);
        }

        /*public Person DeepCopy() // We don't need this one anymore, we made an universal mechanism
        {
            return new Person(Names, Address.DeepCopy());
        }*/
    }

    // [Serializable]
    public class Address // : IPrototype<Address> // : ICloneable
    {
        public string StreetName;
        public int HouseNumber;

        public Address()
        {
            
        }

        public Address(string streetName, int houseNumber)
        {
            StreetName = streetName;
            HouseNumber = houseNumber;
        }

        public override string ToString()
        {
            return $"{nameof(StreetName)}: {StreetName}, {nameof(HouseNumber)}: {HouseNumber}";
        }

        // to use ICloneable is not the best way to go because is badly specified and it returns an object instead of something strongly typed
        /*public object Clone()
        {
            return new Address(StreetName, HouseNumber);
        }*/

        // Another approach to this problem is to use copy constructors, which is a term from C++
        public Address(Address other)
        {
            StreetName = other.StreetName;
            HouseNumber = other.HouseNumber;
        }

        /*public Address DeepCopy() // We don't need this one anymore, we made an universal mechanism
        {
            return new Address(StreetName, HouseNumber);
        }*/
    }

    // All the business of having to implement the special copy logic in each type is very tedious and we want to stay away from it
    /*public interface IPrototype<T>
    {
        T DeepCopy();
    }*/

    // This is a way to automate the process by using a serializer it should serialize the entire tree
    // This is how the prototype pattern is applied in the real world
    public static class ExtensionMethods
    {
        public static T DeepCopy<T>(this T self)
        {
            var stream = new MemoryStream(); // it will be stored in memory
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, self); // this will require our classes to add Serializable attribute
            stream.Seek(0, SeekOrigin.Begin); // here we're getting the object out
            object copy = formatter.Deserialize(stream);
            stream.Close();
            return (T)copy;
        }

        public static T DeepCopyXml<T>(this T self) // another way to deepCopy an object
        {
            using (var ms = new MemoryStream()) // with 'using' we don't need to manually close the Stream
            {
                var s = new XmlSerializer(typeof(T)); // it needs the default constructor
                s.Serialize(ms, self);
                ms.Position = 0;
                return (T) s.Deserialize(ms);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var john = new Person(new [] {"John", "Smith"}, new Address("London Road", 123));

            // var jane = john; // wrong! we're copying a reference to john
            // jane.Names[0] = "Jane"; // this will modify the Name in both john and jane

            // var jane = (Person) john.Clone(); // This won't solve the problem since the implementation of clone does a shallow copy
            // jane.Address.HouseNumber = 321; // this will change the property in both objects

            // var jane = new Person(john);
            // jane.Address.HouseNumber = 321; // everything is working correctly

            // var jane = john.DeepCopy(); Uncomment the Serializable attribute
            var jane = john.DeepCopyXml();
            jane.Names[0] = "Jane";
            jane.Address.HouseNumber = 321;

            Console.WriteLine(john);
            Console.WriteLine(jane);

            Console.ReadLine();
        }
    }
}
