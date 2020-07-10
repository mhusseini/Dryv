using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    internal class ControllerMethodGenerator
    {
        private static readonly Type ControllerBaseType = typeof(DryvDynamicController);

        private static readonly MethodInfo JsonAsyncMethod = ControllerBaseType.GetMethod(nameof(DryvDynamicController.JsonAsync));

        private static readonly MethodInfo JsonMethod = ControllerBaseType.GetMethod(nameof(DryvDynamicController.JsonSync));

        public static void GenerateWrapperMethodGet(MethodInfo methodInfo, Type modelType, MethodCallExpression methodExpression, LambdaExpression lambda, TypeBuilder typeBuilder, FieldInfo delegateField, IDictionary<ParameterExpression, FieldBuilder> innerFields, DryvControllerGenerationContext context, DryvDynamicControllerOptions options)
        {
            var isAsync = IsTaskType(lambda);
            var memberFinder = new ControllerGenerator.MemberFinder(modelType);
            memberFinder.Visit(methodExpression);
            var properties = memberFinder.FoundMemberExpressions.Select(e => e.Member).Distinct().OfType<PropertyInfo>().ToList();

            var method = lambda.Type.GetMethod("Invoke");
            var methodBuilder = CreateMethodBuilder(typeBuilder, methodInfo.Name, GetReturnType(lambda), properties.Select(p => p.PropertyType).ToArray(), context, options);
            var index = 1;
            var parameterBuilders = properties.ToDictionary(
                p => p,
                p => methodBuilder.DefineParameter(index++, ParameterAttributes.None, p.Name));

            ControllerAttributeGenerator.SetAttribute<HttpGetAttribute>(methodBuilder);
            AddRoutingAttribute(methodBuilder, context, options);

            var il = methodBuilder.GetILGenerator();
            var modelVariable = il.DeclareLocal(modelType);

            var ctor = modelType.GetConstructors().First(c => !c.GetParameters().Any());
            il.Emit(OpCodes.Newobj, ctor);

            index = 1;

            foreach (var builder in parameterBuilders)
            {
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldarg, index++);
                il.Emit(OpCodes.Callvirt, builder.Key.SetMethod);
            }

            il.Emit(OpCodes.Stloc_S, modelVariable);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldsfld, delegateField);

            foreach (var parameter in lambda.Parameters)
            {
                if (innerFields.TryGetValue(parameter, out var field))
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, field);
                }
                else if (parameter.Type == modelType)
                {
                    il.Emit(OpCodes.Ldloc_S, modelVariable);
                }
            }

            il.Emit(OpCodes.Callvirt, method);
            il.Emit(OpCodes.Callvirt, isAsync ? JsonAsyncMethod : JsonMethod);

            il.Emit(OpCodes.Ret);
        }

        public static void GenerateWrapperMethodPost(MethodInfo methodInfo, Type modelType, LambdaExpression lambda, TypeBuilder typeBuilder, FieldInfo delegateField, IDictionary<ParameterExpression, FieldBuilder> innerFields, DryvControllerGenerationContext context, DryvDynamicControllerOptions options)
        {
            var isAsync = IsTaskType(lambda);
            var method = lambda.Type.GetMethod("Invoke");
            var methodBuilder = CreateMethodBuilder(typeBuilder, methodInfo.Name, GetReturnType(lambda), new[] { modelType }, context, options);
            var parameterBuilder = methodBuilder.DefineParameter(1, ParameterAttributes.None, "model");
            parameterBuilder.SetCustomAttribute(ControllerAttributeGenerator.CreateAttributeBuilder<FromBodyAttribute>());

            ControllerAttributeGenerator.SetAttribute<HttpPostAttribute>(methodBuilder);
            AddRoutingAttribute(methodBuilder, context, options);

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldsfld, delegateField);

            foreach (var parameter in lambda.Parameters)
            {
                if (innerFields.TryGetValue(parameter, out var field))
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, field);
                }
                else if (parameter.Type == modelType)
                {
                    il.Emit(OpCodes.Ldarg_1);
                }
            }

            il.Emit(OpCodes.Callvirt, method);
            il.Emit(OpCodes.Callvirt, isAsync ? JsonAsyncMethod : JsonMethod);

            il.Emit(OpCodes.Ret);
        }

        private static void AddRoutingAttribute(MethodBuilder methodBuilder, DryvControllerGenerationContext context, DryvDynamicControllerOptions options)
        {
            if (options.MapRouteTemplate != null)
            {
                ControllerAttributeGenerator.SetAttribute<RouteAttribute>(methodBuilder, options.MapRouteTemplate(context));
            }
        }

        private static MethodBuilder CreateMethodBuilder(TypeBuilder typeBuilder, string methodName, Type returnType, Type[] parameterTypes, DryvControllerGenerationContext context, DryvDynamicControllerOptions options)
        {
            var methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public, returnType, parameterTypes);

            ControllerAttributeGenerator.AddCustomAttributes(context, methodBuilder.SetCustomAttribute, options.MapActionFilters);

            return methodBuilder;
        }

        private static Type GetReturnType(LambdaExpression lambda)
        {
            return IsTaskType(lambda)
                ? typeof(Task<IActionResult>)
                : typeof(IActionResult);
        }

        private static bool IsTaskType(LambdaExpression lambda)
        {
            return typeof(Task).IsAssignableFrom(lambda.Body.Type);
        }
    }
}