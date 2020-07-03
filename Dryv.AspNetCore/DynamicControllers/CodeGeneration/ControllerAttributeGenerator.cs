using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    internal class ControllerAttributeGenerator
    {
        public static void AddCustomAttributes(DryvControllerGenerationContext context, Action<CustomAttributeBuilder> set, Func<DryvControllerGenerationContext, IEnumerable<Expression<Func<Attribute>>>> get)
        {
            var expressions = get?.Invoke(context);
            AddCustomAttributes(context, set, expressions);
        }

        public static void AddCustomAttributes(DryvControllerGenerationContext context, Action<CustomAttributeBuilder> set, params Expression<Func<Attribute>>[] expressions)
        {
            AddCustomAttributes(context, set, (IEnumerable<Expression<Func<Attribute>>>)expressions);
        }

        public static void AddCustomAttributes(DryvControllerGenerationContext context, Action<CustomAttributeBuilder> set, IEnumerable<Expression<Func<Attribute>>> expressions)
        {
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

        public static CustomAttributeBuilder CreateAttributeBuilder<T>(params object[] args) where T : Attribute
        {
            return new CustomAttributeBuilder(typeof(T).GetConstructor(args.Select(a => a.GetType()).ToArray()), args);
        }

        public static void SetAttribute<T>(MethodBuilder methodBuilder, params object[] args) where T : Attribute
        {
            var attributeBuilder = CreateAttributeBuilder<T>(args);
            methodBuilder.SetCustomAttribute(attributeBuilder);
        }
    }
}