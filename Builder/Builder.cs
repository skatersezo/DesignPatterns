using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Builder
{
    class Builder
    {
        public class Person
        {
            // address
            public string StreetAdress, Postcode, City;
            // employment
            public string CompanyName, Position;
            public int AnnualIncome;

            public override string ToString()
            {
                return $"{nameof(StreetAdress)}: {StreetAdress}, {nameof(Postcode)}: {Postcode}, {nameof(City)}: {City}" +
                       $", {nameof(CompanyName)}: {CompanyName}, {nameof(Position)}: {Position}" +
                       $", {nameof(AnnualIncome)}: {AnnualIncome}";
            }
        }

        public class PersonBuilder // facade for other builders
        {
            protected Person person = new Person(); // reference to the object to build!

            public PersonJobBuilder Works => new PersonJobBuilder(person);
            public PersonAddressBuilder Lives => new PersonAddressBuilder(person);

            public static implicit operator Person(PersonBuilder pb)
            {
                return pb.person;
            }
        }

        public class PersonJobBuilder : PersonBuilder
        {
            public PersonJobBuilder(Person person)
            {
                this.person = person;
            }

            public PersonJobBuilder At(string companyName)
            {
                person.CompanyName = companyName;
                return this;
            }

            public PersonJobBuilder AsA(string position)
            {
                person.Position = position;
                return this;
            }

            public PersonJobBuilder Earning(int amount)
            {
                person.AnnualIncome = amount;
                return this;
            }
        }

        public class PersonAddressBuilder : PersonBuilder
        {
            public PersonAddressBuilder(Person person)
            {
                this.person = person;
            }

            public PersonAddressBuilder At(string streetAddress)
            {
                person.StreetAdress = streetAddress;
                return this;
            }

            public PersonAddressBuilder WithPostcode(string postcode)
            {
                person.Postcode = postcode;
                return this;
            }
        }
    }
}
