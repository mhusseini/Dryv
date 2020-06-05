using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Translation;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    internal static class ControllerFilterHelper
    {
        public static (ConstructorInfo ctor,
            object[] args,
            PropertyInfo[] properties,
            FieldInfo[] fields,
            object[] values) GetAttributeBuilderArgs(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            switch (expression)
            {
                case LambdaExpression lambdaExpression:
                    return GetAttributeBuilderArgs(lambdaExpression.Body);

                case MemberInitExpression memberInitExpression:
                    var ctor = memberInitExpression.NewExpression.Constructor;
                    var args = memberInitExpression.NewExpression.Arguments.Select(a => a.GetValue()).ToArray();
                    var (properties, fields, values) = GetInitializers(memberInitExpression, ctor);
                    return (ctor, args, properties, fields, values);

                case NewExpression newExpression:
                    ctor = newExpression.Constructor;
                    args = newExpression.Arguments.Select(a => a.GetValue()).ToArray();
                    return (ctor, args, null, null, null);

                default:
                    throw new DryvConfigurationException("Only 'new' expressions with optional initializers are allowed.");
            }
        }

        private static (PropertyInfo[] properties, FieldInfo[] fields, object[] values) GetInitializers(MemberInitExpression memberInitExpression, ConstructorInfo ctor)
        {
            var initializers = (from binding in memberInitExpression.Bindings
                                where binding is MemberAssignment
                                let member = binding.Member
                                let value = ((MemberAssignment)binding).Expression.GetValue()
                                select (member, value)).ToList();
            var memberTypes = initializers.Select(i => i.member.MemberType).Distinct().ToList();

            if (memberTypes.Count > 1)
            {
                throw new DryvConfigurationException($"Cannot initialize filter {ctor.DeclaringType?.FullName} with both property and field initialization. Please only set properties or only set fields in the initializer expression.");
            }

            var memberType = memberTypes.Single();

            switch (memberType)
            {
                case MemberTypes.Field:
                    var (fields, values1) = GetTypedInitializers<FieldInfo>(initializers);
                    return (null, fields, values1);

                case MemberTypes.Property:
                    var (properties, values2) = GetTypedInitializers<PropertyInfo>(initializers);
                    return (properties, null, values2);

                default:
                    throw new DryvConfigurationException($"Cannot initialize filter {ctor.DeclaringType?.FullName} with {memberType}. Please only set properties or only set fields in the initializer expression.");
            }
        }

        private static (T[] fields, object[] values) GetTypedInitializers<T>(IReadOnlyCollection<(MemberInfo member, object value)> initializers)
            where T : MemberInfo
        {
            var fields = initializers.Select(i => i.member).Cast<T>().ToArray();
            var values = initializers.Select(i => i.value).ToArray();

            return (fields, values);
        }
    }
}