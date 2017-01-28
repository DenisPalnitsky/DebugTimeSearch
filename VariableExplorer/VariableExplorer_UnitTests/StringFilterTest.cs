using MyCompany.VariableExplorer.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    public class StringFilterTest
    {
        [Test]
        public void IsMatch_returns_true()
        {
            //Exact match 
            StringFilter filter = new StringFilter("Abc");
            Assert.IsTrue(filter.IsMatching("Abc"));

            //Case insensitive
            filter = new StringFilter("Abc");
            Assert.IsTrue(filter.IsMatching("ABc"));

            // Wildcard *
            filter = new StringFilter("A*");
            Assert.IsTrue(filter.IsMatching("ABc"));

            // Wildcard ?
            filter = new StringFilter("A?c");
            Assert.IsTrue(filter.IsMatching("ABc"));

            filter = new StringFilter(String.Empty);
            Assert.IsTrue(filter.IsMatching("ABc"));
        }

        [Test]
        public void Regression()
        {
             StringFilter filter = new StringFilter("item");
             Assert.IsTrue(filter.IsMatching("Item 0"));

             filter = new StringFilter("item 0");
             Assert.IsTrue(filter.IsMatching("Item 0"));
        }

    }
}
