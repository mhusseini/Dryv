using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    internal class ControllerGenerator
    {
        private const string NameSpace = "Dryv.Dynamic";
        private static readonly ConcurrentDictionary<string, Assembly> Cache = new ConcurrentDictionary<string, Assembly>();
        private static int assemblyCount;
        private readonly IOptions<DryvDynamicControllerOptions> options;

        public ControllerGenerator(IOptions<DryvDynamicControllerOptions> options)
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
                var typeBuilder = moduleBuilder.DefineType($"{NameSpace}.{typeNameBase}Controller", TypeAttributes.Class | TypeAttributes.Public, baseType);

                var context = new DryvControllerGenerationContext(typeBuilder, methodInfo);
                AddCustomAttributes(context, this.options.Value.MapControllerFilters, typeBuilder.SetCustomAttribute);

                var innerFieldBuilder = innerType == null ? null : typeBuilder.DefineField("inner", innerType, FieldAttributes.Private);

                GenerateConstructor(typeBuilder, baseType, innerFieldBuilder);

                if (this.options.Value.HttpMethod == DryvDynamicControllerMethods.Post)
                {
                    var properties = methodInfo.GetParameters().ToDictionary(p => p.Name, p => p.ParameterType);
                    var pocoGenerator = new DryvPocoGenerator();
                    var dtoType = pocoGenerator.GenerateType(moduleBuilder, NameSpace, $"{typeNameBase}Dto", properties);

                    this.GenerateWrapperMethodPost(methodInfo, typeBuilder, innerFieldBuilder, dtoType, context);
                }
                else
                {
                    this.GenerateWrapperMethodGet(methodInfo, typeBuilder, innerFieldBuilder, context);
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

        private static CustomAttributeBuilder CreateAttributeBuilder<T>(params object[] args) where T : Attribute
        {
            return new CustomAttributeBuilder(typeof(T).GetConstructor(args.Select(a => a.GetType()).ToArray()), args);
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

        private static void SetAttribute<T>(MethodBuilder methodBuilder, params object[] args) where T : Attribute
        {
            var attributeBuilder = CreateAttributeBuilder<T>(args);
            methodBuilder.SetCustomAttribute(attributeBuilder);
        }

        private static void AddCustomAttributes(DryvControllerGenerationContext context, Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> get, Action<CustomAttributeBuilder> set)
        {
            var expressions = get?.Invoke(context);

            if (expressions == null)
            {
                return;
            }

            foreach (var expression in from expression in expressions
                                       where expression != null
                                       select expression)
            {
                var (ctor, args, properties, fields, values) = ControllerFilterHelper.GetAttributeBuilderArgs(expression);
                var attributeBuilder = values == null
                    ? new CustomAttributeBuilder(ctor, args)
                    : fields == null
                    ? new CustomAttributeBuilder(ctor, args, properties, values)
                    : new CustomAttributeBuilder(ctor, args, fields, values);

                set(attributeBuilder);
            }
        }

        private void AddRoutingAttribute(MethodBuilder methodBuilder, DryvControllerGenerationContext context)
        {
            if (this.options.Value.MapRouteTemplate == null)
            {
                return;
            }

            var template = this.options.Value.MapRouteTemplate(context);

            SetAttribute<RouteAttribute>(methodBuilder, template);
        }

        private MethodBuilder CreateMethodBuilder(TypeBuilder typeBuilder, MethodInfo methodInfo, Type[] parameterTypes, DryvControllerGenerationContext context)
        {
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public, methodInfo.ReturnType, parameterTypes);

            AddCustomAttributes(context, this.options.Value.MapActionFilters, methodBuilder.SetCustomAttribute);

            return methodBuilder;
        }

        private void GenerateWrapperMethodGet(MethodInfo methodInfo, TypeBuilder typeBuilder, FieldInfo innerFieldBuilder, DryvControllerGenerationContext context)
        {
            var parameters = methodInfo.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var methodBuilder = this.CreateMethodBuilder(typeBuilder, methodInfo, parameterTypes, context);

            SetAttribute<HttpGetAttribute>(methodBuilder);
            this.AddRoutingAttribute(methodBuilder, context);

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

        private void GenerateWrapperMethodPost(MethodInfo methodInfo, TypeBuilder typeBuilder, FieldInfo innerFieldBuilder, Type dtoType, DryvControllerGenerationContext context)
        {
            var methodBuilder = this.CreateMethodBuilder(typeBuilder, methodInfo, new[] { dtoType }, context);
            var parameterBuilder = methodBuilder.DefineParameter(1, ParameterAttributes.None, "model");
            parameterBuilder.SetCustomAttribute(CreateAttributeBuilder<FromBodyAttribute>());

            SetAttribute<HttpPostAttribute>(methodBuilder);
            this.AddRoutingAttribute(methodBuilder, context);

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
    }
}