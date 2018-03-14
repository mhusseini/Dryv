using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.Demo.Nav
{
    public class NavCollector
    {
        private static Dictionary<string, NavStructure> allItems;

        public Dictionary<string, NavStructure> GetNavStructure(ViewContext viewContext)
        {
            if (allItems == null)
            {
                allItems = FindStructureElements();
            }

            SetElementSelection(viewContext);

            return allItems;
        }

        private static Dictionary<string, NavStructure> FindStructureElements()
        {
            return (from type in Assembly.GetExecutingAssembly().DefinedTypes
                    where typeof(Controller).IsAssignableFrom(type)
                    from method in type.GetMethods()
                    let attr = method.GetCustomAttribute<NavAttribute>()
                    where attr != null
                    select new NavStructure
                    {
                        Caption = attr.Caption,
                        Action = method.Name,
                        Controller = type.Name.Replace(nameof(Controller), string.Empty)
                    })
                .ToDictionary(s => s.Caption, StringComparer.OrdinalIgnoreCase);
        }

        private static void SetElementSelection(ActionContext viewContext)
        {
            var currentController = viewContext.RouteData.Values["controller"].ToString();
            var currentAction = viewContext.RouteData.Values["action"].ToString();

            foreach (var element in allItems.Values)
            {
                element.IsActive =
                    element.Controller.Equals(currentController, StringComparison.OrdinalIgnoreCase) &&
                    element.Action.Equals(currentAction, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}