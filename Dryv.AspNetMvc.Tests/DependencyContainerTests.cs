using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.AspNetMvc.Tests
{
    public abstract class DependencyContainerTests<TState>
    {
        public abstract TState Begin();

        public abstract IDependencyContainer CreateDependencyContainer(TState state);

        [TestMethod]
        public void RegiserMultipleInstances()
        {
            var state = this.Begin();
            var container = this.CreateDependencyContainer(state);

            container.AddInstance(typeof(TestService), new TestService("text1"));
            container.AddInstance(typeof(TestService), new TestService("text2"));
            container.AddInstance(typeof(TestService), new TestService("text3"));

            this.SetResolver(state);

            HttpContext.Current = CreateHttpContext();
            var texts = DependencyResolver.Current.GetServices<TestService>().Select(x => x.Name).ToArray();

            Assert.AreEqual("text1", texts[0]);
            Assert.AreEqual("text2", texts[1]);
            Assert.AreEqual("text3", texts[2]);
        }

        public abstract void SetResolver(TState state);

        private static HttpContext CreateHttpContext()
        {
            var sb = new StringBuilder();
            var w = new StringWriter(sb);
            var httpContext = new HttpContext(
                new HttpRequest("file.txt", "http://somethind.org", null),
                new HttpResponse(w));
            return httpContext;
        }
    }

    public class TestService
    {
        public TestService(string name) => this.Name = name;

        public string Name { get; }

        public override int GetHashCode() => this.Name.GetHashCode();
    }
}