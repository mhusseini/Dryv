using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    internal class DryvPocoGenerator
    {
        private const MethodAttributes PropertyMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

        public TypeBuilder GenerateType(ModuleBuilder moduleBuilder, string nameSpace, string name, IDictionary<string, Type> properties)
        {
            var typeBuilder = moduleBuilder.DefineType($"{nameSpace}.{name}", TypeAttributes.Class | TypeAttributes.Public);

            foreach (var (propertyName, propertyType) in properties)
            {
                AddProperty(typeBuilder, propertyName, propertyType);
            }

            typeBuilder.CreateType();

            return typeBuilder;
        }

        private static PropertyBuilder AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var field = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Generate getter method

            var getter = typeBuilder.DefineMethod("get_" + propertyName, PropertyMethodAttributes, propertyType, Type.EmptyTypes);

            var il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // Push "this" on the stack
            il.Emit(OpCodes.Ldfld, field); // Load the field "_Name"
            il.Emit(OpCodes.Ret); // Return

            propertyBuilder.SetGetMethod(getter);

            // Generate setter method

            var setter = typeBuilder.DefineMethod("set_" + propertyName, PropertyMethodAttributes, null, new[] { propertyType });

            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // Push "this" on the stack
            il.Emit(OpCodes.Ldarg_1); // Push "value" on the stack
            il.Emit(OpCodes.Stfld, field); // Set the field "_Name" to "value"
            il.Emit(OpCodes.Ret); // Return

            propertyBuilder.SetSetMethod(setter);

            return propertyBuilder;
        }
    }
}