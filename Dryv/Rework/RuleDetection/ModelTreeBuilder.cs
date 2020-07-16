using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Rework.RuleDetection
{
    internal class ModelTreeBuilder
    {
        public ModelTreeNode Build(Type type)
        {
            return this.Build(type, string.Empty, string.Empty, new HashSet<Type>(), new Stack<MemberInfo>());
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return from property in type.GetProperties()
                   where property.DeclaringType.Namespace != typeof(object).Namespace
                   select property;
        }

        private ModelTreeNode Build(Type type, string uniquePath, string modelPath, ICollection<Type> processed, Stack<MemberInfo> hierarchy)
        {
            var isRecursive = processed.Contains(type);

            processed.Add(type);

            var node = new ModelTreeNode
            {
                UniquePath = uniquePath + ":" + type.FullName,
                ModelPath = modelPath,
                ModelType = type,
                IsRecursive = isRecursive,
                Hierarchy = hierarchy.Reverse().ToList()
            };

            if (!isRecursive)
            {
                node.Children = GetProperties(type).Select(p =>
                {
                    hierarchy.Push(p);

                    var result = this.BuildEdge(node, p, processed, hierarchy);
                    hierarchy.Pop();

                    return result;
                }).ToList();
            }

            return node;
        }

        private ModelTreeEdge BuildEdge(ModelTreeNode parent, PropertyInfo property, ICollection<Type> processed, Stack<MemberInfo> hierarchy)
        {
            return new ModelTreeEdge
            {
                Parent = parent,
                Property = property,
                Child = this.Build(property.PropertyType, parent.UniquePath + "." + property.Name, parent.ModelPath + "." + property.Name, processed, hierarchy)
            };
        }
    }
}