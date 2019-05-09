using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

/// <summary>
/// Single Responsability Principle
/// -------------------------------
/// Every class should have just a single reason to change
/// </summary>
namespace DesignPatterns
{
    /// <summary>
    /// Journal class
    /// The concern of this class is to keep a list of entries
    /// </summary>
    public class Journal
    {
        private readonly List<string> entries = new List<string>();
        private static int count = 0;

        public int AddEntry(string text)
        {
            entries.Add($"{++count}: {text}");
            return count; // memento pattern
        }

        public void RemoveEntry(int index)
        {
            entries.RemoveAt(index);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, entries);
        }

        // The following code adds too much responsability to the Journal class

        /*public void Save(string fileName)
        {
            File.WriteAllText(fileName, ToString());
        }

        public static Journal Load(string fileName)
        {
            return null;
        }

        public void Load(Uri uri)
        {

        }*/
    }

    /// <summary>
    /// Class Persistence
    /// This class introduces a separation of concerns
    /// This class is now concern just to persist the data in memory
    /// </summary>
    public class Persistence
    {
        public void SaveToFile(Journal journal, string fileName, bool overwrite = false)
        {
            if (overwrite || !File.Exists(fileName))
                File.WriteAllText(fileName, journal.ToString());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var journal = new Journal();
            journal.AddEntry("I ate jamon");
            journal.AddEntry("I strumbled upon a stone");
            Console.WriteLine(journal);
            
            var p = new Persistence();
            var fileName = @"D:\dev\DesignPatterns\temp\journal.txt";
            p.SaveToFile(journal, fileName, true);
            Process.Start(fileName);
        }
    }
}
