using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.RuleDetection
{
    internal static class ModelTreeNodeExtensions
    {
        public static List<ModelTreeEdge> CopyChildren(this ModelTreeNode oldParent, ModelTreeNode newParent, string oldModelPath, string oldUniquePath, string newModelPath, string newUniquePath)
        {
            return oldParent.Children.Select(edge => new ModelTreeEdge
                {
                    Parent = oldParent,
                    Property = edge.Property,
                    Child = edge.Child.Copy(newParent, oldModelPath, oldUniquePath, newModelPath, newUniquePath)
                })
                .ToList();
        }

        private static ModelTreeNode Copy(this ModelTreeNode oldNode, ModelTreeNode newParent, string oldModelPath, string oldUniquePath, string newModelPath, string newUniquePath)
        {
            return new ModelTreeNode
            {
                Hierarchy = CopyHierarchy(oldNode, newParent),
                IsRecursive = oldNode.IsRecursive,
                ModelType = oldNode.ModelType,
                ModelPath = oldNode.ModelPath.Replace(oldModelPath, newModelPath),
                UniquePath = oldNode.UniquePath.Replace(oldUniquePath, newUniquePath),
                Children = oldNode.CopyChildren(newParent, oldModelPath, oldUniquePath, newModelPath, newUniquePath)
            };
        }

        private static List<MemberInfo> CopyHierarchy(ModelTreeNode oldNode, ModelTreeNode newParent)
        {
            var hierarchy = new List<MemberInfo>(newParent.Hierarchy);
            
            hierarchy.AddRange(oldNode.Hierarchy.Skip(newParent.Hierarchy.Count));

            return hierarchy;
        }
    }
}