using System.Web.Mvc;
using Dryv.AspNetMvc.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Web.Mvc;

namespace Dryv.AspNetMvc.Ninject.Tests
{
    [TestClass]
    public class AutoFacDependencyContainerTests : DependencyContainerTests<IKernel>
    {
        public override IKernel Begin() =>
            new StandardKernel();

        public override IDependencyContainer CreateDependencyContainer(IKernel kernel) =>
            new DependencyContainer(kernel);

        public override void SetResolver(IKernel kernel) =>
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
    }
}