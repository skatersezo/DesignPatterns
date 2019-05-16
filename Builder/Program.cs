using System;
using System.Collections.Generic;
using System.Text;
using static Builder.Builder;

namespace Builder
{
    class Program
    {

        static void Main(string[] args)
        {
            // In .NET Framework the StringBuilder implements the Builder pattern already
            var hello = "hello";
            var sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append(hello);
            sb.Append("</p>");
            Console.WriteLine(sb);
            sb.Clear();
            // This way to build HTML elements is too complex
            var words = new[] {"hello", "world"};
            sb.Append("<ul>");
            foreach (var word in words)
            {
                sb.AppendFormat("<li>{0}</p>", word);
            }
            sb.Append("</ul>");
            Console.WriteLine(sb);

            // --------------------------------------------------------------------------------
            // Now the code is structured in a more Object Oriented way
            // having an HtmlBuilder which his own purpose is to build html elements
            var builder = new HtmlBuilder("ul");
            builder.AddChild("li", "hello").AddChild("li", "world"); ;
            Console.WriteLine(builder.ToString());

            Console.ReadLine();

            // -------------------------------------------------------------------------------

            var pb = new PersonBuilder();
            // The reason why we can jump from building the address to the employment info
            // is because both, the address and the employment builder inherit from PersonBuilder
            // so both of them expose every other builder
            // A side effect is that you can do -> person.Lives.Lives.Lives...
            Person person = pb.Lives.At("123 London Road").WithPostcode("SW12AC")
                .Works.At("Acme").AsA("Coyote").Earning(23000);
            // We've used two sub-builders to present an interface that is nice and convenient, but the object
            // we built isn't a Person, it appears like PersonBuilder, to avoid that happen we use
            // public static implicit operator Person(PersonBuilder pb) and return pb.person
            Console.WriteLine(person.ToString());

            Console.ReadLine();
        }
    }

    public class HtmlElement
    {
        public string Name, Text;
        public List<HtmlElement> Elements = new List<HtmlElement>();
        private const int indentSize = 2;

        public HtmlElement() { }

        public HtmlElement(string name, string text)
        {
            Name = name;
            Text = text;
        }

        private string ToStringImpl(int indent)
        {
            var sb = new StringBuilder();
            var i = new string(' ', indentSize * indent);
            sb.AppendLine($"{i}<{Name}>");

            if (!string.IsNullOrEmpty(Text))
            {
                sb.Append(new string(' ', indentSize * (indent + 1)));
                sb.AppendLine(Text);
            }

            foreach (var htmlElement in Elements)
            {
                sb.Append(htmlElement.ToStringImpl(indent + 1));
            }

            sb.AppendLine($"{i}</{Name}>");
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToStringImpl(0);
        }
    }

    public class HtmlBuilder
    {
        private readonly string RootName;
        HtmlElement root = new HtmlElement();

        public HtmlBuilder(string rootName)
        {
            this.RootName = rootName;
            root.Name = rootName;
        }

        public HtmlBuilder AddChild(string childName, string childText)
        {
            var e = new HtmlElement(childName, childText);
            root.Elements.Add(e);
            return this; // by this technique we can chain the AddChild method
        }

        public override string ToString()
        {
            return root.ToString();
        }

        public void Clear()
        {
            root = new HtmlElement {Name = RootName};
        }
    }
}
