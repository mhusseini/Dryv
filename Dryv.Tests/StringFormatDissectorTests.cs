using Dryv.MethodCallTranslation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class StringFormatDissectorTests
    {
        [TestMethod]
        public void ParsePattern()
        {
            var translator = new StringFormatDissector();
            var parts = translator.Parse("test {0} is {1} but {{2}} isn't.");

            Assert.AreEqual(5, parts.Count);
            Assert.AreEqual(typeof(string), parts[0].GetType());
            Assert.AreEqual(typeof(int), parts[1].GetType());
            Assert.AreEqual(typeof(string), parts[2].GetType());
            Assert.AreEqual(typeof(int), parts[3].GetType());
            Assert.AreEqual(typeof(string), parts[4].GetType());
        }

        [TestMethod]
        public void RecombinePattern()
        {
            var translator = new StringFormatDissector();
            var parts = translator.Recombine("test {0} is {1} but {{2}} isn't.", new object[] { "pattern", "cool" });
            var text = string.Concat(parts);

            Assert.AreEqual("test pattern is cool but {{2}} isn't.", text);
        }
    }
}
