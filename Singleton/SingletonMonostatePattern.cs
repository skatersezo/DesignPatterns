using System;
using System.Collections.Generic;
using System.Text;

namespace Singleton
{
    /// <summary>
    /// The monostate pattern uses static fields in a pretty bizarre way
    /// An static class can't be instantiated and need to be refered by it's
    /// name, making impossible to use dependency injection
    /// </summary>
    class SingletonMonostatePattern
    {
        public static void Example()
        {
            var ceo = new CEO();
            ceo.Name = "Adam Smith";
            ceo.Age = 57;

            var ceo2 = new CEO();
            Console.WriteLine($"First instance of CEO class: {ceo}");
            Console.WriteLine($"Second instance of CEO class:{ceo2}");
        }
    }

    /// <summary>
    /// In our company there can be only 1 CEO at a given time
    /// so we want to prevent people from create more than one CEO
    /// We will have the state of the object being static but exposing it's
    /// properties in a non static way
    /// </summary>
    public class CEO
    {
        // these are the actual properties of the CEO
        private static string name;
        private static int age;
        // the following properties just exposes the CEO props in a non-static way
        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Age
        {
            get => age;
            set => age = value;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Age)}: {Age}";
        }
    }
}
