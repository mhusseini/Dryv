using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

namespace Dryv.AspNetCore.DynamicControllers.Translation
{
    internal class MemberExpressionComparer : IEqualityComparer<MemberExpression>
    {
        public static readonly MemberExpressionComparer Default = new MemberExpressionComparer();

        public bool Equals(MemberExpression x, MemberExpression y)
        {
            return x != null && y != null &&
                   x.Member.DeclaringType != null && y.Member.DeclaringType != null &&
                   x.Member.DeclaringType.FullName == y.Member.DeclaringType.FullName &&
                   x.Member.Name == y.Member.Name;
        }

        public int GetHashCode(MemberExpression obj)
        {
            return obj.Member.DeclaringType == null ? 0 : (obj.Member.DeclaringType.FullName + obj.Member.Name).GetHashCode();
        }
    }
}