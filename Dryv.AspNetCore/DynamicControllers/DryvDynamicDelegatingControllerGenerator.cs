using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers
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
                var nameSpace = "Dryv.Dynamic";
                var typeNameBase = $"DryvDynamic{assemblyIndex}";
                var baseType = typeof(Controller);
                var injectedObject = methodExpression.Object;
                var innerType = injectedObject?.Type;
                var currentAssembly = Assembly.GetExecutingAssembly();
                var assemblyName = new AssemblyName($"DryvDynamicAssembly{assemblyIndex}")
                {
                    CodeBase = currentAssembly.CodeBase,
                };
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);

                var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
                var typeBuilder = moduleBuilder.DefineType($"{nameSpace}.{typeNameBase}Controller", TypeAttributes.Class | TypeAttributes.Public, baseType);

                foreach (var (attributeType, arguments) in this.options.Value.DefaultAttributes)
                {
                    SetAttribute(typeBuilder, attributeType, arguments);
                }

                var innerFieldBuilder = innerType == null ? null : typeBuilder.DefineField("inner", innerType, FieldAttributes.Private);

                GenerateConstructor(typeBuilder, baseType, innerFieldBuilder);

                if (this.options.Value.HttpMethod == DryvDynamicControllerMethods.Post)
                {
                    var properties = methodInfo.GetParameters().ToDictionary(p => p.Name, p => p.ParameterType);
                    var pocoGenerator = new DryvPocoGenerator();
                    var dtoType = pocoGenerator.GenerateType(moduleBuilder, nameSpace, $"{typeNameBase}Dto", properties);

                    this.GenerateWrapperMethodPost(methodInfo, typeBuilder, innerFieldBuilder, dtoType);
                }
                else
                {
                    this.GenerateWrapperMethodGet(methodInfo, typeBuilder, innerFieldBuilder);
                }

                typeBuilder.CreateType();

                //var generator = new Lokad.ILPack.AssemblyGenerator();
                //generator.GenerateAssembly(assemblyBuilder, $"{assemblyName}.dll");
                //Process.Start(new ProcessStartInfo
                //{
                //    UseShellExecute = true,
                //    WindowStyle = ProcessWindowStyle.Normal,
                //    FileName = "dotnet",
                //    Arguments = $"ildasm {assemblyName}.dll -o {assemblyName}.ildasm --force",
                //}).WaitForExit();

                return assemblyBuilder;
            });
        }

        private static void GenerateConstructor(TypeBuilder typeBuilder, Type baseType, FieldInfo innerFieldBuilder)
        {
            var ctrArgs = innerFieldBuilder == null ? null : new[] { innerFieldBuilder.FieldType };
            var baseConstructorInfo = baseType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[0], null);
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
            var routeAttributeBuilder = CreateAttributeBuilder<T>(args);
            builder.SetCustomAttribute(routeAttributeBuilder);
        }

        private static CustomAttributeBuilder CreateAttributeBuilder<T>(params object[] args) where T : Attribute
        {
            return new CustomAttributeBuilder(typeof(T).GetConstructor(args.Select(a => a.GetType()).ToArray()), args);
        }

        private void AddRoutingAttribute(MemberInfo methodInfo, MemberInfo typeBuilder, IEnumerable<ParameterInfo> parameters, MethodBuilder methodBuilder)
        {
            if (this.options.Value.MapTemplate == null)
            {
                return;
            }

            var template = this.options.Value.MapTemplate(typeBuilder.Name, methodInfo.Name, parameters.ToDictionary(p => p.Name, p => p.ParameterType));
            SetAttribute<RouteAttribute>(methodBuilder, template);
        }

        private void GenerateWrapperMethodPost(MethodInfo methodInfo, TypeBuilder typeBuilder, FieldInfo innerFieldBuilder, Type dtoType)
        {
            var parameters = methodInfo.GetParameters();
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public, methodInfo.ReturnType, new Type[] { dtoType });
            var parameterBuilder = methodBuilder.DefineParameter(1, ParameterAttributes.None, "model");
            parameterBuilder.SetCustomAttribute(CreateAttributeBuilder<FromBodyAttribute>());

            SetAttribute<HttpPostAttribute>(methodBuilder);
            this.AddRoutingAttribute(methodInfo, typeBuilder, parameters, methodBuilder);

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, innerFieldBuilder);

            var dtoGetters = dtoType.GetProperties().ToDictionary(p => p.Name, p => p.GetMethod, StringComparer.OrdinalIgnoreCase);

            foreach (var parameter in methodInfo.GetParameters())
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, dtoGetters[parameter.Name]);
            }

            il.Emit(OpCodes.Callvirt, methodInfo);
            il.Emit(OpCodes.Ret); // Return
        }

        private void GenerateWrapperMethodGet(MethodInfo methodInfo, TypeBuilder typeBuilder, FieldInfo innerFieldBuilder)
        {
            var parameters = methodInfo.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public, methodInfo.ReturnType, parameterTypes);

            SetAttribute<HttpGetAttribute>(methodBuilder);
            this.AddRoutingAttribute(methodInfo, typeBuilder, parameters, methodBuilder);

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