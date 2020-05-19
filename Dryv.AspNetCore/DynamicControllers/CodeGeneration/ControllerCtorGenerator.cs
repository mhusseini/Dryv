using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    internal static class ControllerCtorGenerator
    {
        public static void GenerateConstructor(TypeBuilder typeBuilder, Type baseType, IReadOnlyCollection<FieldBuilder> fields)
        {
            var ctrArgs = fields.Select(f => f.FieldType).ToArray();
            var baseConstructorInfo = baseType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[0], null);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctrArgs);

            AddNameParameters(fields.Select(f => f.Name), constructor);

            var ilGenerator = constructor.GetILGenerator();

            AddBaseCtorCall(baseConstructorInfo, ilGenerator);

            AddFieldInitializer(fields, ilGenerator);

            ilGenerator.Emit(OpCodes.Ret); // Return
        }

        private static void AddBaseCtorCall(ConstructorInfo baseConstructorInfo, ILGenerator ilGenerator)
        {
            if (baseConstructorInfo == null)
            {
                return;
            }

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, baseConstructorInfo);
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Nop);
        }

        private static void AddFieldInitializer(IEnumerable<FieldBuilder> fields, ILGenerator ilGenerator)
        {
            var i = 1;

            foreach (var field in fields)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg, i++);
                ilGenerator.Emit(OpCodes.Stfld, field);
            }
        }

        private static void AddNameParameters(IEnumerable<string> parameterNames, ConstructorBuilder constructor)
        {
            var i = 1;

            foreach (var parameterName in parameterNames)
            {
                constructor.DefineParameter(i++, ParameterAttributes.None, parameterName);
            }
        }
    }
}