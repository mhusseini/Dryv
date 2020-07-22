using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.Options;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    public class ControllerGenerator
    {
        private const string NameSpace = "Dryv.Dynamic";
        private static int assemblyCount;
        private readonly IOptions<DryvDynamicControllerOptions> options;

        public ControllerGenerator(IOptions<DryvDynamicControllerOptions> options)
        {
            this.options = options;
        }

        public Assembly CreateControllerAssembly(Expression expression, Type modelType, string action)
        {
            return this.CreateAssembly(expression, modelType, action);
        }

        public static List<ParameterExpression> FindParameters(Expression methodCallExpression)
        {
            //var finder = new ExpressionNodeFinder<ParameterExpression>();
            return new ParameterFinder().Find(methodCallExpression);
        }

        private Assembly CreateAssembly(Expression expression, Type modelType, string action)
        {
            var assemblyIndex = ++assemblyCount;
            var typeNameBase = $"DryvDynamic{assemblyIndex}";
            var baseType = typeof(DryvDynamicController);
            var currentAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName($"DryvDynamicAssembly{assemblyIndex}")
            {
                CodeBase = currentAssembly.CodeBase,
            };

            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            var typeBuilder = moduleBuilder.DefineType($"{NameSpace}.{typeNameBase}Controller", TypeAttributes.Class | TypeAttributes.Public, baseType);

            var context = new DryvControllerGenerationContext(typeBuilder, action);
            ControllerAttributeGenerator.AddCustomAttributes(context, typeBuilder.SetCustomAttribute, this.options.Value.MapControllerFilters);
            ControllerAttributeGenerator.AddCustomAttributes(context, typeBuilder.SetCustomAttribute, () => new DryvDisableAttribute());

            var parameters = FindParameters(expression);
            var modelParameter = parameters.Find(p => p.Type == modelType);
            var lambda = expression is LambdaExpression l
                ? Expression.Lambda(l.Body, parameters)
                : Expression.Lambda(expression, parameters);

            var delegateField = typeBuilder.DefineField("_delegate", lambda.Type, FieldAttributes.Private | FieldAttributes.Static);
            parameters.Remove(modelParameter);

            var innerFields = parameters.ToDictionary(
                p => p,
                p => typeBuilder.DefineField(p.Name, p.Type, FieldAttributes.Private));

            ControllerCtorGenerator.GenerateConstructor(typeBuilder, baseType, innerFields.Values.ToList());

            if (this.options.Value.HttpMethod == DryvDynamicControllerMethods.Post)
            {
                ControllerMethodGenerator.GenerateWrapperMethodPost(modelType, action, lambda, typeBuilder, delegateField, innerFields, context, this.options.Value);
            }
            else
            {
                ControllerMethodGenerator.GenerateWrapperMethodGet(modelType, action, expression, lambda, typeBuilder, delegateField, innerFields, context, this.options.Value);
            }

            var type = typeBuilder.CreateType();
            var field = type.GetField("_delegate", BindingFlags.Static | BindingFlags.NonPublic);

            Debug.Assert(field != null);

            field.SetValue(null, lambda.Compile());

            this.options.Value.GeneratedAssemblyOutput?.Invoke(assemblyBuilder);

            return assemblyBuilder;
        }

        public class MemberFinder : ExpressionVisitor
        {
            private readonly Type type;

            public MemberFinder(Type type)
            {
                this.type = type;
            }

            public List<MemberExpression> FoundMemberExpressions { get; } = new List<MemberExpression>();

            protected override Expression VisitMember(MemberExpression node)
            {
                if (this.type.IsAssignableFrom(node.Member.DeclaringType))
                {
                    this.FoundMemberExpressions.Add(node);
                }

                return base.VisitMember(node);
            }
        }
    }
}