using Dryv.Demo.Nav;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.Demo
{
    public class NavCollector
    {
        private static List<NavStructure> allItems;
        private static List<NavStructure> itemsHiaerarchy;

        public List<NavStructure> GetNavStructure(ViewContext viewContext)
        {
            if (itemsHiaerarchy == null)
            {
                BuildUp();
            }

            ResetElements();
            SetActiveHierarchy(viewContext);

            return itemsHiaerarchy;
        }

        private static void BuildUp()
        {
            var structureElements = FindStructureElements();

            BuildUpHierarchy(structureElements);
            allItems = structureElements.Values.ToList();

            itemsHiaerarchy = allItems
                .Where(s => s.Parent == null)
                .ToList();
        }

        private static void BuildUpHierarchy(Dictionary<string, NavStructure> structureElements)
        {
            foreach (var element in from element in structureElements.Values
                                    where !string.IsNullOrWhiteSpace(element.ParentName)
                                    select element)
            {
                structureElements[element.ParentName].Children.Add(element);
            }
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
                        Name = attr.Name,
                        Caption = attr.Caption,
                        Action = method,
                        Controller = type,
                        ParentName = attr.Parent
                    })
                                 .ToDictionary(s => s.Name);
        }

        private static NavStructure GetCurrentNavElement(ViewContext viewContext)
        {
            var currentController = viewContext.RouteData.Values["controller"].ToString() + "Controller";
            var currentAction = viewContext.RouteData.Values["action"].ToString();
            var currentElement = GetCurrentNavElement(currentController, currentAction, allItems);
            return currentElement;
        }

        private static NavStructure GetCurrentNavElement(string currentController, string currentAction, IEnumerable<NavStructure> structureElements)
        {
            return structureElements
                            .First(s => s.Controller.Name.Equals(currentController, StringComparison.OrdinalIgnoreCase)
                        && s.Action.Name.Equals(currentAction, StringComparison.OrdinalIgnoreCase));
        }

        private static void ResetElements()
        {
            foreach (var element in allItems)
            {
                element.IsActive = false;
            }
        }

        private static void SetActiveHierarchy(ViewContext viewContext)
        {
            var currentElement = GetCurrentNavElement(viewContext);
            currentElement = SetActiveHierarchy(currentElement);
        }

        private static NavStructure SetActiveHierarchy(NavStructure currentElement)
        {
            while (currentElement != null)
            {
                currentElement.IsActive = true;
                currentElement = currentElement.Parent;
            }

            return currentElement;
        }
    }
}