using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using NUnit.Framework;

namespace Singleton
{
    [TestFixture]
    class SingletonTests
    {
        [Test]
        public void IsSingletonTest()
        {
            var db = SingletonDatabase.Instance;
            var db2 = SingletonDatabase.Instance;
            Assert.That(db, Is.SameAs(db2));
            Assert.That(SingletonDatabase.Count, Is.EqualTo(1));
        }

        [Test]
        public void SingletonTotalPopulationTest()
        {
            // Here we're testing on a 'live db'
            // One of the consequences is that we need to look at the DB for the values we want to test
            var rf = new SingletonRecordFinder();
            // If somebody removes from the db one of the entries, the test will break
            var names = new[] {"Seoul", "Mexico City", "Osaka"};
            int tp = rf.GetTotalPopulation(names);
            Assert.That(tp, Is.EqualTo(17500000 + 17400000 + 16425000));
            // to be independent from a live db is very expensive
            // what you want to do is to fake the object instead the real db
            // In this particular scenario, to fake it isn't possible because RecordFinder is using a hardcoded reference
        }

        [Test]
        public void ConfigurablePopulationTest()
        {
            // now in the ConfigurableRecordFinder we provide the db we want to use, for the test, will be the dummy one
            var rf = new ConfigurableRecordFinder(new DummyDatabase());
            // In this test we don't need to find records from the production db, we faked ones already
            var names = new[] {"alpha", "gamma"};
            int tp = rf.GetTotalPopulation(names);
            Assert.That(tp, Is.EqualTo(4));
        }

        [Test]
        public void DIPopulationTest()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<OrdinaryDatabase>() // instead of making the component a singleton
                .As<IDatabase>() // we register it as a IDatabase
                .SingleInstance(); // but we tell the DI container to provide it as Singleton
            cb.RegisterType<ConfigurableRecordFinder>(); // one per request (each call instantiate a new instance)
            using (var c = cb.Build()) // we build the container
            {
                // here we ger a ConfigurableRecordFinder whose constructor parameter is initialized with a singleton
                // instance of IDatabase which we have configured for our purposes with the ordinary database
                // but it can be easily done with the dummy db
                var rf = c.Resolve<ConfigurableRecordFinder>();
                 
            }
        }
    }
}
