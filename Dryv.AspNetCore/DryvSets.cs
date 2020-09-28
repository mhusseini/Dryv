using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.AspNetCore
{
    public static class DryvSets
    {
        private static readonly List<(Type Type, string Name)> Sets;

        static DryvSets() => Sets = (
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            orderby t.FullName
            let attr = t.GetCustomAttribute<DryvSetAttribute>()
            where attr != null
            select (t, attr.Name)
        ).ToList();

        public static List<(Type Type, string Name)> GetDryvSets() => Sets;
    }
}