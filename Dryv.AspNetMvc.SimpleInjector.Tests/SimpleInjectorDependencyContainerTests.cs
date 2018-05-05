using System.Web.Mvc;
using Dryv.AspNetMvc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace Dryv.AspNetMvc.SimpleInjector.Tests
{
    [TestClass]
    public class SimpleInjectorDependencyContainerTests : DependencyContainerTests<Container>
    {
        public override Container Begin() =>
            new Container();

        public override IDependencyContainer CreateDependencyContainer(Container container) =>
            new DependencyContainer(container);

        public override void SetResolver(Container container) =>
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
    }
}