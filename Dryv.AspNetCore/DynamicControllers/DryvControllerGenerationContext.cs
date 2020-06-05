using System.Linq;
using System.Reflection;

namespace Dryv.AspNetCore.DynamicControllers
{
    public class DryvControllerGenerationContext
    {
        private static readonly int ControllerLength = "controller".Length;

        internal DryvControllerGenerationContext(MemberInfo type, MemberInfo method)
        {
            var typeName = type.Name.Split('.').Last();
            var controller = typeName.Substring(0, typeName.Length - ControllerLength);

            this.ControllerFullName = type.Name;
            this.Controller = controller;
            this.Action = method.Name;
            this.RuleMethod = method;
        }

        public DryvControllerGenerationContext()
        {
        }

        /// <summary>
        /// The name of the generated action.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The short name of the generated controller.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// The full name of the generated controller.
        /// </summary>
        public string ControllerFullName { get; set; }

        /// <summary>
        /// The method that is used for the async method call in the validation rule.
        /// </summary>
        public MemberInfo RuleMethod { get; set; }
    }
}