using System.Web.Mvc;
using Castle.Windsor;
using Dryv.AspNetMvc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.AspNetMvc.CastleWindsor.Tests
{
    [TestClass]
    public class CastleWindsorDependencyContainerTests : DependencyContainerTests<IWindsorContainer>
    {
        public override IWindsorContainer Begin() =>
            new WindsorContainer();

        public override IDependencyContainer CreateDependencyContainer(IWindsorContainer container) =>
            new DependencyContainer(container);

        public override void SetResolver(IWindsorContainer container) =>
            DependencyResolver.SetResolver(new WindsorDependencyResolver(container));
    }
}