using System;

namespace Dryv.Reflection
{
    [Flags]
    internal enum BindingFlags
    {
        Default = 0,
        Instance = 4,
        Static = 8,
        Public = 16,
        NonPublic = 32,
        FlattenHierarchy = 64
    }
}