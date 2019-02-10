using System.Diagnostics;
using Dryv.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.Tests
{
    [TestClass]
    public class ModelTreeTests
    {
        [TestMethod]
        public void ModelPath()
        {
            var childc = new Model
            {
                Name = "C"
            };

            var model = new Model
            {
                Name = "A",
                Child = new Model
                {
                    Name = "B",
                    Child2 = childc
                }
            };

            var tree = childc.GetTreeInfo(model);

            Assert.AreEqual(tree.PathsByModel[childc], string.Empty);
            Assert.AreEqual(tree.ModelsByPath[string.Empty], childc);
        }

        [TestMethod]
        public void FindModelInTheMiddle()
        {
            var childc = new Model
            {
                Name = "C"
            };

            var model = new Model
            {
                Name = "A",
                Child = new Model
                {
                    Name = "B",
                    Child2 = childc
                }
            };

            var tree = childc.GetTreeInfo(model);
            var intermediate = tree.FindModel("child2") as Model;

            Assert.IsNotNull(intermediate);
            Assert.AreEqual("B", intermediate.Name);
        }

        [DebuggerDisplay("{" + nameof(Name) + "}")]
        public class Model
        {
            public string Name { get; set; }
            public Model Child { get; set; }
            public Model Child2 { get; set; }
        }
    }
}