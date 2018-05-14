using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Windsor;

namespace Dryv.AspNetMvc
{
    internal sealed class WindsorDependencyResolver : IDependencyResolver
    {
        private readonly IWindsorContainer container;

        public WindsorDependencyResolver(IWindsorContainer container) => this.container = container ?? throw new ArgumentNullException(nameof(container));

        public object GetService(Type t) => this.container.Kernel.HasComponent(t) ? this.container.Resolve(t) : null;

        public IEnumerable<object> GetServices(Type t) => this.container.ResolveAll(t).Cast<object>().ToArray();
    }
}