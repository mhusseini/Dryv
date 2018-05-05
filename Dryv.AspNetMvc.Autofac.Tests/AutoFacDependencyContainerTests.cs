using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Dryv.AspNetMvc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dryv.AspNetMvc.Autofac.Tests
{
    [TestClass]
    public class AutofacDependencyContainerTests : DependencyContainerTests<ContainerBuilder>
    {
        public override ContainerBuilder Begin() =>
            new ContainerBuilder();

        public override IDependencyContainer CreateDependencyContainer(ContainerBuilder builder) =>
            new DependencyContainer(builder);

        public override void SetResolver(ContainerBuilder builder) =>
            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
    }
}