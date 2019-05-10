using System;

/// <summary>
/// Interface Segregation Principle
/// -------------------------------
/// When creating an interface, the user shouldn't implement
/// methods that won't need
/// </summary>
namespace InterfaceSegregationPrinciple
{
    public class Document
    {
        // properties
    }

    /// <summary>
    /// This interface does too much, for some classes
    /// will be ok, but some other won't need some functions
    /// </summary>
    public interface IMachine
    {
        void Print(Document document);
        void Scan(Document document);
        void Fax(Document document);
    }

    public class MultifunctionPrinter : IMachine
    {
        public void Fax(Document document)
        {
            // code
        }

        public void Print(Document document)
        {
            // code
        }

        public void Scan(Document document)
        {
            // code
        }
    }

    public class OldFashionPrinter : IMachine
    {
        public void Fax(Document document)
        {
            throw new NotImplementedException(); // this function isn't necessary
        }

        public void Print(Document document)
        {
            // code
        }

        public void Scan(Document document)
        {
            throw new NotImplementedException(); // this function isn't necessary
        }
    }

    public interface IPrinter
    {
        void Print(Document document);
    }

    public interface IScanner
    {
        void Scan(Document document);
    }

    public class Photocopier : IPrinter, IScanner
    {
        public void Print(Document document)
        {
            // code
        }

        public void Scan(Document document)
        {
            // code
        }
    }

    public interface IMultifunctionDevice : IScanner, IPrinter // ,...
    {
        // Interfaces can implement another interfaces if necessary
    }

    public class MultifunctionMachine : IMultifunctionDevice
    {
        // By creating this properties we can delegate in them for the interface implementation
        private IPrinter printer;
        private IScanner scanner;

        public MultifunctionMachine(IPrinter printer, IScanner scanner)
        {
            this.printer = printer;
            this.scanner = scanner;
        }

        public void Print(Document document)
        {
            printer.Print(document);
        }

        public void Scan(Document document)
        {
            scanner.Scan(document);
        } // decorator pattern
    }

    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
