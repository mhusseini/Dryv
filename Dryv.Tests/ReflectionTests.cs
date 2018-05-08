using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dryv.Reflection;

namespace Dryv.Tests
{
    [TestClass]
    public class ReflectionTests
    {
        [TestMethod]
        public void TestGetFlattenedProperties()
        {
            var proprties = typeof(TestClassA).GetProperties(BindingFlags.FlattenHierarchy);

            Assert.IsTrue(proprties.Any(p => p.DeclaringType == typeof(TestClassB)), "Inherited properties not returned.");
            Assert.IsTrue(proprties.Any(p => p.DeclaringType == typeof(TestClassA)), "Instance properties not returned.");
        }

        [TestMethod]
        public void TestGetInstanceProperties()
        {
            var proprties = typeof(TestClassA).GetProperties(BindingFlags.Instance);

            Assert.IsTrue(proprties.All(p => p.Name != nameof(TestClassA.StaticProperty)), "Static properties returned.");
        }

        [TestMethod]
        public void TestGetNonFlattenedProperties()
        {
            var proprties = typeof(TestClassA).GetProperties(BindingFlags.Public);

            Assert.IsTrue(proprties.All(p => p.DeclaringType != typeof(TestClassB)), "Inherited properties returned.");
            Assert.IsTrue(proprties.Any(p => p.DeclaringType == typeof(TestClassA)), "Instance properties not returned.");
        }

        [TestMethod]
        public void TestGetNonPublicProperties()
        {
            var proprties = typeof(TestClassA).GetProperties(BindingFlags.NonPublic);

            Assert.IsTrue(proprties.All(p => p.Name == "NonPublicProperty"), "Public properties returned.");
        }

        [TestMethod]
        public void TestGetPublicProperties()
        {
            var proprties = typeof(TestClassA).GetProperties(BindingFlags.Public);

            Assert.IsTrue(proprties.All(p => p.Name == nameof(TestClassA.PublicProperty)), "Non-public properties returned.");
        }

        [TestMethod]
        public void TestGetStaticProperties()
        {
            var proprties = typeof(TestClassA).GetProperties(BindingFlags.Static);

            Assert.IsTrue(proprties.All(p => p.Name == nameof(TestClassA.StaticProperty)), "Non-static properties returned.");
        }

        private class TestClassA : TestClassB
        {
            public static bool StaticProperty { get; set; }
            public bool PublicProperty { get; set; }
            private bool NonPublicProperty { get; set; }
        }

        private class TestClassB
        {
            public bool BaseClassProperty { get; set; }
        }
    }
}