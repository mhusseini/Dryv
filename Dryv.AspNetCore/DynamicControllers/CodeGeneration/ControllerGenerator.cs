using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Dryv.Translation;
using Microsoft.AspNetCore.Mvc;
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

        public Assembly CreateControllerAssembly(MethodCallExpression methodExpression, Type modelType)
        {
            var methodInfo = methodExpression.Method;
            return this.CreateAssembly(methodExpression, modelType, methodInfo);
        }

        private static List<ParameterExpression> FindParameters(Expression methodCallExpression)
        {
            var finder = new ParameterFinder();
            finder.Visit(methodCallExpression);

            return finder.FoundChildren.Distinct().ToList();
        }

        private Assembly CreateAssembly(MethodCallExpression methodExpression, Type modelType, MethodInfo methodInfo)
        {
            var assemblyIndex = ++assemblyCount;
            var typeNameBase = $"DryvDynamic{assemblyIndex}";
            var baseType = typeof(Controller);
            var currentAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName($"DryvDynamicAssembly{assemblyIndex}")
            {
                CodeBase = currentAssembly.CodeBase,
            };

            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            var typeBuilder = moduleBuilder.DefineType($"{NameSpace}.{typeNameBase}Controller", TypeAttributes.Class | TypeAttributes.Public, baseType);

            var context = new DryvControllerGenerationContext(typeBuilder, methodInfo);
            ControllerAttributeGenerator.AddCustomAttributes(context, typeBuilder.SetCustomAttribute, this.options.Value.MapControllerFilters);
            ControllerAttributeGenerator.AddCustomAttributes(context, typeBuilder.SetCustomAttribute, () => new DryvDisableAttribute());

            var parameters = FindParameters(methodExpression);
            var modelParameter = parameters.Find(p => p.Type == modelType);
            var lambda = Expression.Lambda(methodExpression, parameters);

            var delegateField = typeBuilder.DefineField("_delegate", lambda.Type, FieldAttributes.Private | FieldAttributes.Static);
            parameters.Remove(modelParameter);

            var innerFields = parameters.ToDictionary(
                p => p,
                p => typeBuilder.DefineField(p.Name, p.Type, FieldAttributes.Private));

            ControllerCtorGenerator.GenerateConstructor(typeBuilder, baseType, innerFields.Values.ToList());

            if (this.options.Value.HttpMethod == DryvDynamicControllerMethods.Post)
            {
                ControllerMethodGenerator.GenerateWrapperMethodPost(methodInfo, modelType, lambda, typeBuilder, delegateField, innerFields, context, this.options.Value);
            }
            else
            {
                ControllerMethodGenerator.GenerateWrapperMethodGet(methodInfo, modelType, methodExpression, lambda, typeBuilder, delegateField, innerFields, context, this.options.Value);
            }

            var type = typeBuilder.CreateType();
            var field = type.GetField("_delegate", BindingFlags.Static | BindingFlags.NonPublic);

            Debug.Assert(field != null);

            field.SetValue(null, lambda.Compile());

            var generator = new Lokad.ILPack.AssemblyGenerator();
            generator.GenerateAssembly(assemblyBuilder, $"{typeNameBase}.dll");
            //Process.Start(new ProcessStartInfo
            //{
            //    UseShellExecute = true,
            //    WindowStyle = ProcessWindowStyle.Normal,
            //    FileName = "dotnet",
            //    Arguments = $"ildasm {typeNameBase}.dll -o EMIT.ildasm --force",
            //}).WaitForExit();

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