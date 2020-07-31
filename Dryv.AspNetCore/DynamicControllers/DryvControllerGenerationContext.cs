using System;
using System.Linq;
using Dryv.Rules;

namespace Dryv.AspNetCore.DynamicControllers
{
    public class DryvControllerGenerationContext
    {
        private static readonly int ControllerLength = "controller".Length;

        internal DryvControllerGenerationContext(Type type, string action, DryvCompiledRule rule)
        {
            var typeName = type.Name.Split('.').Last();
            var controller = typeName.Substring(0, typeName.Length - ControllerLength);

            this.ControllerFullName = type.Name;
            this.Controller = controller;
            this.Action = action;
            this.Rule = rule;
        }

        public DryvControllerGenerationContext()
        {
        }

        /// <summary>
        /// The name of the generated action.
        /// </summary>
        public string Action { get; set; }

        public DryvCompiledRule Rule { get; set; }

        /// <summary>
        /// The short name of the generated controller.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// The full name of the generated controller.
        /// </summary>
        public string ControllerFullName { get; set; }
    }
}