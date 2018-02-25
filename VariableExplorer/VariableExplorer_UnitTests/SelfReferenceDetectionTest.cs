using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLocals.Model.VSPropertyModel;

namespace VariableExplorer_UnitTests
{
    [TestFixture]
    class SelfReferenceDetectionTest
    {
        [Test]
        public void Returns_true_when_property_name_is_repeated()
        {
            Assert.IsTrue(SelfReferenceDetection.DoesItLookLikeSelfReference("property.Date.Date.Date.Date"));
            Assert.IsTrue(SelfReferenceDetection.DoesItLookLikeSelfReference("property.Date.Date.Date.Date.Date"));            
        }

        [Test]
        public void Rerurns_false_when_propery_name_is_not_repeated()
        {            
            Assert.IsFalse(SelfReferenceDetection.DoesItLookLikeSelfReference("class.Prop.Date.property.Date.Date"));
            Assert.IsFalse(SelfReferenceDetection.DoesItLookLikeSelfReference("class.Prop.Date.property"));
            Assert.IsFalse(SelfReferenceDetection.DoesItLookLikeSelfReference("class.Prop"));
            Assert.IsFalse(SelfReferenceDetection.DoesItLookLikeSelfReference("class.class"));
            Assert.IsFalse(SelfReferenceDetection.DoesItLookLikeSelfReference("Prop.Date.date.Date"));
        }
    }
}
