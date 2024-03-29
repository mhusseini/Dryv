﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Extensions;
using Dryv.Reflection;

namespace Dryv.RuleDetection
{
    public class ModelTreeBuilder
    {
        public ModelTreeNode Build(Type type)
        {
            return this.Build(type, string.Empty, string.Empty, new Dictionary<Type, ModelTreeNode>(), new Stack<MemberInfo>());
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return from property in type.GetProperties()
                   where property.DeclaringType.Namespace != typeof(object).Namespace
                   select property;
        }

        private ModelTreeNode Build(Type type, string uniquePath, string modelPath, IDictionary<Type, ModelTreeNode> processed, Stack<MemberInfo> hierarchy)
        {
            var isRecursive = processed.TryGetValue(type, out var originalNode);

            var node = new ModelTreeNode
            {
                UniquePath = uniquePath + ":" + type.GetNonGenericName(),
                ModelPath = modelPath,
                ModelType = type,
                IsRecursive = isRecursive,
                Hierarchy = hierarchy.Reverse().ToList()
            };

            if (!isRecursive)
            {
                processed.Add(type, node);

                node.Children = GetProperties(type).Select(p =>
                {
                    hierarchy.Push(p);

                    var result = this.BuildEdge(node, p, processed, hierarchy);

                    hierarchy.Pop();

                    return result;
                }).ToList();
            }
            else
            {
                node.Children = originalNode.CopyChildren(node, originalNode.ModelPath, originalNode.UniquePath, node.ModelPath, node.UniquePath);
            }

            return node;
        }

        private ModelTreeEdge BuildEdge(ModelTreeNode parent, PropertyInfo property, IDictionary<Type, ModelTreeNode> processed, Stack<MemberInfo> hierarchy)
        {
            var index = parent.UniquePath.LastIndexOf(":", StringComparison.OrdinalIgnoreCase);
            if (index < 0) index = 0;
            var uniquePath = parent.UniquePath.Substring(0, index) + ":" + property.DeclaringType.GetNonGenericName();

            return new ModelTreeEdge
            {
                Parent = parent,
                Property = property,
                Child = this.Build(property.PropertyType, uniquePath + "." + property.Name, parent.ModelPath + "." + property.Name, processed, hierarchy)
            };
        }
    }
}