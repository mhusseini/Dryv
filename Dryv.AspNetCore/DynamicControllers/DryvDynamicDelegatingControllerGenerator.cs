using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dryv.DynamicControllers
{
    internal class DryvDynamicDelegatingControllerGenerator
    {
        private static readonly ConcurrentDictionary<string, Assembly> Cache = new ConcurrentDictionary<string, Assembly>();
        private static int assemblyCount;
        private readonly IOptions<DryvDynamicControllerOptions> options;

        public DryvDynamicDelegatingControllerGenerator(IOptions<DryvDynamicControllerOptions> options)
        {
            this.options = options;
        }

        public Assembly CreateControllerAssembly(MethodCallExpression methodExpression)
        {
            var methodInfo = methodExpression.Method;

            var key = GetCacheKey(methodInfo);
            return Cache.GetOrAdd(key, _ =>
            {
                var assemblyIndex = ++assemblyCount;
                var baseType = typeof(Controller);
                var injectedObject = methodExpression.Object;
                var innerType = injectedObject?.Type;
                var assemblyName = new AssemblyName($"DryvDynamicAssembly{assemblyIndex}");
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
                var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
                var typeBuilder = moduleBuilder.DefineType($"Dryv.Dynamic.DryvDynamic{assemblyIndex}Controller", TypeAttributes.Class | TypeAttributes.Public, baseType);

                foreach (var (attributeType, arguments) in this.options.Value.DefaultAttributes)
                {
                    SetAttribute(typeBuilder, attributeType, arguments);
                }

                var innerFieldBuilder = innerType == null ? null : typeBuilder.DefineField("inner", innerType, FieldAttributes.Private);

                GenerateConstructor(typeBuilder, baseType, innerFieldBuilder);
                this.GenerateWrapperMethod(methodInfo, typeBuilder, innerFieldBuilder);

                typeBuilder.CreateType();

                return assemblyBuilder;
            });
        }

        private static void GenerateConstructor(TypeBuilder typeBuilder, Type baseType, FieldInfo innerFieldBuilder)
        {
            var ctrArgs = innerFieldBuilder == null ? null : new[] { innerFieldBuilder.FieldType };
            var baseConstructorInfo = baseType.GetConstructor(BindingFlags.Public | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[0], null);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctrArgs);
            var ilGenerator = constructor.GetILGenerator();

            if (baseConstructorInfo != null)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0); // push "this" onto stack.
                ilGenerator.Emit(OpCodes.Call, baseConstructorInfo); // call base constructor
                ilGenerator.Emit(OpCodes.Nop);
                ilGenerator.Emit(OpCodes.Nop);
            }

            if (innerFieldBuilder != null)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0); // push "this" onto stack.
                ilGenerator.Emit(OpCodes.Ldarg_1); // push 1. argument onto stack.
                ilGenerator.Emit(OpCodes.Stfld, innerFieldBuilder); // push "this" onto stack.
            }

            ilGenerator.Emit(OpCodes.Ret); // Return
        }

        private static string GetCacheKey(MethodInfo methodInfo)
        {
            Debug.Assert(methodInfo.DeclaringType != null);
            return string.Join('|', methodInfo.DeclaringType.AssemblyQualifiedName, methodInfo.Name, methodInfo.ReturnParameter.ParameterType.AssemblyQualifiedName, string.Join('|', methodInfo.GetParameters().Select(p => p.ParameterType.AssemblyQualifiedName)));
        }

        private static void SetAttribute(TypeBuilder builder, Type attributeType, params object[] args)
        {
            var routeAttributeBuilder = new CustomAttributeBuilder(attributeType.GetConstructor(args.Select(a => a.GetType()).ToArray()), args);
            builder.SetCustomAttribute(routeAttributeBuilder);
        }

        private static void SetAttribute<T>(MethodBuilder builder, params object[] args) where T : Attribute
        {
            var routeAttributeBuilder = new CustomAttributeBuilder(typeof(T).GetConstructor(args.Select(a => a.GetType()).ToArray()), args);
            builder.SetCustomAttribute(routeAttributeBuilder);
        }

        private void GenerateWrapperMethod(MethodInfo methodInfo, TypeBuilder typeBuilder, FieldInfo innerFieldBuilder)
        {
            var parameters = methodInfo.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public, methodInfo.ReturnType, parameterTypes);

            if (this.options.Value.MapTemplate != null)
            {
                var template = this.options.Value.MapTemplate(typeBuilder.Name, methodInfo.Name, parameters.ToDictionary(p => p.Name, p => p.ParameterType));
                SetAttribute<HttpGetAttribute>(methodBuilder, template);
            }

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, innerFieldBuilder);

            var i = 0;

            foreach (var parameter in methodInfo.GetParameters())
            {
                ++i;
                methodBuilder.DefineParameter(i, ParameterAttributes.None, parameter.Name);
                il.Emit(OpCodes.Ldarg, i);
            }

            il.Emit(OpCodes.Callvirt, methodInfo);
            il.Emit(OpCodes.Ret); // Return
        }
    }
}