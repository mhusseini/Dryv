using System.Web.Mvc;
using Dryv.AspNetMvc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.AspNet.Mvc;

namespace Dryv.AspNetMvc.Unity.Tests
{
    [TestClass]
    public class AutoFacDependencyContainerTests : DependencyContainerTests<UnityContainer>
    {
        public override UnityContainer Begin() =>
            new UnityContainer();

        public override IDependencyContainer CreateDependencyContainer(UnityContainer container) =>
            new DependencyContainer(container);

        public override void SetResolver(UnityContainer container) =>
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
    }
}