using Dryv.DependencyInjection;
using Dryv.MethodCallTranslation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv.Translation
{
    internal class JavaScriptTranslator : Translator
    {
        private static readonly Dictionary<ExpressionType, string> Terminals = new Dictionary<ExpressionType, string>
        {
            [ExpressionType.Add] = "+",
            //[ExpressionType.AddChecked] = "",
            [ExpressionType.And] = "&&",
            [ExpressionType.AndAlso] = "&&",
            //[ExpressionType.ArrayLength] = "",
            //[ExpressionType.ArrayIndex] = "",
            //[ExpressionType.Call] = "",
            [ExpressionType.Coalesce] = "||",
            [ExpressionType.Conditional] = "",
            //[ExpressionType.Constant] = "",
            //[ExpressionType.Convert] = "",
            //[ExpressionType.ConvertChecked] = "",
            [ExpressionType.Divide] = "/",
            [ExpressionType.Equal] = "===",
            [ExpressionType.ExclusiveOr] = "||",
            [ExpressionType.GreaterThan] = ">",
            [ExpressionType.GreaterThanOrEqual] = ">=",
            //[ExpressionType.Invoke] = "",
            //[ExpressionType.Lambda] = "",
            //[ExpressionType.LeftShift] = "",
            [ExpressionType.LessThan] = "<",
            [ExpressionType.LessThanOrEqual] = "<=",
            //[ExpressionType.ListInit] = "",
            //[ExpressionType.MemberAccess] = "",
            //[ExpressionType.MemberInit] = "",
            [ExpressionType.Modulo] = "%",
            [ExpressionType.Multiply] = "*",
            //[ExpressionType.MultiplyChecked] = "",
            [ExpressionType.Negate] = "!",
            [ExpressionType.UnaryPlus] = "++",
            //[ExpressionType.NegateChecked] = "",
            //[ExpressionType.New] = "",
            //[ExpressionType.NewArrayInit] = "",
            //[ExpressionType.NewArrayBounds] = "",
            [ExpressionType.Not] = "!",
            [ExpressionType.NotEqual] = "!=",
            [ExpressionType.Or] = "||",
            [ExpressionType.OrElse] = "||",
            //[ExpressionType.Parameter] = "",
            //[ExpressionType.Power] = "",
            [ExpressionType.Quote] = "\"",
            //[ExpressionType.RightShift] = "",
            [ExpressionType.Subtract] = "-",
            //[ExpressionType.SubtractChecked] = "",
            //[ExpressionType.TypeAs] = "",
            [ExpressionType.TypeIs] = "is",
            [ExpressionType.Assign] = "=",
            //[ExpressionType.Block] = "",
            //[ExpressionType.DebugInfo] = "",
            [ExpressionType.Decrement] = "--",
            //[ExpressionType.Dynamic] = "",
            //[ExpressionType.Default] = "",
            //[ExpressionType.Extension] = "",
            //[ExpressionType.Goto] = "",
            [ExpressionType.Increment] = "++",
            //[ExpressionType.Index] = "",
            //[ExpressionType.Label] = "",
            //[ExpressionType.RuntimeVariables] = "",
            //[ExpressionType.Loop] = "",
            //[ExpressionType.Switch] = "",
            //[ExpressionType.Throw] = "throw",
            //[ExpressionType.Try] = "",
            //[ExpressionType.Unbox] = "",
            [ExpressionType.AddAssign] = "+=",
            [ExpressionType.AndAssign] = "&=",
            [ExpressionType.DivideAssign] = "/=",
            [ExpressionType.ExclusiveOrAssign] = "|=",
            //[ExpressionType.LeftShiftAssign] = "",
            //[ExpressionType.ModuloAssign] = "",
            [ExpressionType.MultiplyAssign] = "*=",
            [ExpressionType.OrAssign] = "|=",
            //[ExpressionType.PowerAssign] = "",
            //[ExpressionType.RightShiftAssign] = "",
            [ExpressionType.SubtractAssign] = "-=",
            //[ExpressionType.AddAssignChecked] = "",
            //[ExpressionType.MultiplyAssignChecked] = "",
            //[ExpressionType.SubtractAssignChecked] = "",
            //[ExpressionType.PreIncrementAssign] = "",
            //[ExpressionType.PreDecrementAssign] = "",
            //[ExpressionType.PostIncrementAssign] = "",
            //[ExpressionType.PostDecrementAssign] = "",
            //[ExpressionType.TypeEqual] = "",
            //[ExpressionType.OnesComplement] = "",
            [ExpressionType.IsTrue] = "!== true",
            [ExpressionType.IsFalse] = "!== false",
        };

        private readonly ITranslatorProvider translatorProvider;

        public JavaScriptTranslator(ITranslatorProvider translatorProvider)
        {
            this.translatorProvider = translatorProvider;
        }

        public bool UseLowercaseMembers { get; set; }

        public override object TranslateValue(object value)
        {
            switch (value)
            {
                case string txt:
                    return $"\"{txt}\"";

                case bool b:
                    return b ? "true" : "false";

                default:
                    return value.ToString();
            }
        }

        public override void Visit(BinaryExpression expression, TranslationContext context, bool negated = false)
        {
            this.VisitWithBrackets(expression.Left, context);

            if (!TryWriteTerminal(expression, context.Writer))
            {
                throw expression.Method != null
                    ? (Exception)new MethodCallNotAllowedException(expression)
                    : new ExpressionTypeNotSupportedException(expression);
            }

            this.VisitWithBrackets(expression.Right, context);
        }

        public override void Visit(BlockExpression expression, TranslationContext context, bool negated = false)
        {
            foreach (var variable in expression.Variables)
            {
                context.Writer.WriteLine($"var {this.FormatIdentifier(variable.Name)};");
            }

            base.Visit(expression, context, negated);
        }

        public override void Visit(ConditionalExpression expression, TranslationContext context, bool negated = false)
        {
            this.VisitWithBrackets(expression.Test, context);
            context.Writer.IncrementIndent();
            context.Writer.Write(" ? ");
            this.VisitWithBrackets(expression.IfTrue, context);
            context.Writer.Write(" : ");
            this.VisitWithBrackets(expression.IfFalse, context);
            context.Writer.DecrementIndent();
        }

        public override void Visit(ConstantExpression expression, TranslationContext context, bool negated = false)
        {
            var text = MethodCallTranslator.QuoteValue(expression.Value);

            context.Writer.Write(text);
        }

        public override void Visit(DefaultExpression expression, TranslationContext context, bool negated = false)
        {
            var value = this.GetDefaultValue(expression.Type);
            var text = MethodCallTranslator.QuoteValue(value);

            context.Writer.Write(text);
        }

        public override void Visit(DynamicExpression expression, TranslationContext context, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(GotoExpression expression, TranslationContext context, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(IDynamicExpression expression, TranslationContext context, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(IndexExpression expression, TranslationContext context, bool negated = false)
        {
            if (expression.Arguments.Count > 1)
            {
                throw new NotSupportedException("JavaScript does not support indexers with more than one argument.");
            }

            this.Visit(expression.Object, context);
            context.Writer.Write("[");
            this.Visit(expression.Arguments.First(), context);
            context.Writer.Write("]");
        }

        public override void Visit(Expression expression, TranslationContext context, bool negated = false)
        {
            var context2 = new GenericTranslationContext(context)
            {
                Expression = expression,
                Translator = this,
                Negated = negated
            };

            if (!this.translatorProvider.GenericTranslators.Any(t => t.TryTranslate(context2)))
            {
                this.Visit((dynamic)expression, context2, negated);
            }
        }

        public override void Visit(InvocationExpression expression, TranslationContext context, bool negated = false)
        {
            if (expression.Expression is MemberExpression)
            {
                throw new ExpressionNotSupportedException(expression);
            }

            this.VisitWithBrackets(expression.Expression, context);
            context.Writer.Write("(");
            MethodCallTranslator.WriteArguments(this, expression.Arguments, context);
            context.Writer.Write(")");
        }

        public override void Visit(LabelExpression expression, TranslationContext context, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(LambdaExpression expression, TranslationContext context, bool negated = false)
        {
            context.Writer.Write("function(");
            context.Writer.Write(string.Join(", ", expression.Parameters.Select(p => this.FormatIdentifier(p.Name))));
            context.Writer.WriteLine(") {");
            context.Writer.IncrementIndent();
            context.Writer.Write("return ");
            this.Visit(expression.Body, context);
            context.Writer.WriteLine(";");
            context.Writer.DecrementIndent();
            context.Writer.Write("}");
        }

        public override void Visit(ListInitExpression expression, TranslationContext context, bool negated = false)
        {
            context.Writer.Write("= [");
            var sep = string.Empty;
            foreach (var initializer in expression.Initializers)
            {
                context.Writer.Write(sep);
                foreach (var argument in initializer.Arguments)
                {
                    this.Visit(argument, context);
                }
                sep = ", ";
            }
            context.Writer.Write("]");
        }

        public override void Visit(LoopExpression expression, TranslationContext context, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override void Visit(MemberExpression expression, TranslationContext context, bool negated = false)
        {
            var parameter = GetParameter(expression);
            if (context.OptionsTypes.Contains(parameter?.Type))
            {
                var func = Expression.Lambda(expression, parameter);
                context.OptionDelegates.Add(func);
                if (negated)
                {
                    context.Writer.Write("!(");
                }
                context.Writer.Write($"$${func.GetHashCode()}$$");
                if (negated)
                {
                    context.Writer.Write(")");
                }
                return;
            }

            if (expression.Expression is ConstantExpression ||
                expression.Expression == null)
            {
                var instance = (expression.Expression as ConstantExpression)?.Value;
                switch (expression.Member)
                {
                    case PropertyInfo property:
                        {
                            var value = property.GetValue(instance);
                            context.Writer.Write(this.TranslateValue(value));
                            return;
                        }
                    case FieldInfo field:
                        {
                            var value = field.GetValue(instance);
                            context.Writer.Write(this.TranslateValue(value));
                            return;
                        }
                }
            }
            else
            {
                this.Visit(expression.Expression, context);
                context.Writer.Write(".");
            }

            context.Writer.Write(this.FormatIdentifier(expression.Member.Name));
        }

        public override void Visit(MemberInitExpression expression, TranslationContext context, bool negated = false)
        {
            this.Visit(expression.NewExpression, context);
        }

        public override void Visit(MethodCallExpression expression, TranslationContext context, bool negated = false)
        {
            if (expression.Method.IsStatic && expression.Arguments?.Any() != true)
            {
                var value = expression.Method.Invoke(null, null);
                context.Writer.Write(this.TranslateValue(value));
                return;
            }

            var context2 = new MethodTranslationContext(context)
            {
                Translator = this,
                Expression = expression,
                Negated = negated
            };

            var objectType = context2.Expression.Method.DeclaringType;

            if (this.translatorProvider
                .MethodCallTranslators
                .Where(t => t.SupportsType(objectType))
                .Any(t => t.Translate(context2)))
            {
                return;
            }

            throw new MethodCallNotAllowedException(expression);
        }

        public override void Visit(NewArrayExpression expression, TranslationContext context, bool negated = false)
        {
            context.Writer.Write("[]");
        }

        public override void Visit(NewExpression expression, TranslationContext context, bool negated = false)
        {
            context.Writer.Write("new ");
            context.Writer.Write(expression.Constructor.DeclaringType.Name);
            context.Writer.Write("(");
            MethodCallTranslator.WriteArguments(this, expression.Arguments, context);
            context.Writer.Write(")");
        }

        public override void Visit(ParameterExpression expression, TranslationContext context, bool negated = false)
        {
            context.Writer.Write(expression.Name);
        }

        public override void Visit(RuntimeVariablesExpression expression, TranslationContext context, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SwitchExpression expression, TranslationContext context, bool negated = false)
        {
            context.Writer.Write("switch(");
            this.Visit(expression.SwitchValue, context);
            context.Writer.WriteLine("){");
            foreach (var expressionCase in expression.Cases)
            {
                foreach (var testCase in expressionCase.TestValues)
                {
                    context.Writer.Write("case ");
                    this.Visit(testCase, context);
                    context.Writer.WriteLine(":");
                }

                context.Writer.WriteLine("{");
                context.Writer.IncrementIndent();
                this.Visit(expressionCase.Body, context);
                context.Writer.WriteLine();
                context.Writer.WriteLine("break;");
                context.Writer.DecrementIndent();
                context.Writer.WriteLine("}");
            }
            context.Writer.Write("}");
        }

        public override void Visit(TryExpression expression, TranslationContext context, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override void Visit(TypeBinaryExpression expression, TranslationContext context, bool negated = false)
        {
            throw new ExpressionNotSupportedException(expression);
        }

        public override void Visit(UnaryExpression expression, TranslationContext context, bool negated = false)
        {
            var negatedExpression = expression.NodeType == ExpressionType.Not;
            if (!negatedExpression)
            {
                TryWriteTerminal(expression, context.Writer);
            }

            this.Visit(expression.Operand, context, negatedExpression);
        }

        public override void VisitWithBrackets(Expression expression, TranslationContext context)
        {
            var needsBrackets = GetNeedsBrackets(expression);

            if (needsBrackets)
            {
                context.Writer.Write("(");
            }

            this.Visit(expression, context);

            if (needsBrackets)
            {
                context.Writer.Write(") ");
            }
        }

        private static bool GetNeedsBrackets(Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression _:
                case MethodCallExpression _:
                case MemberExpression _:
                case UnaryExpression _:
                    return false;
            }

            return true;
        }

        private static ParameterExpression GetParameter(Expression expression)
        {
            while (expression is MemberExpression memberExpression)
            {
                expression = memberExpression.Expression;
            }

            return expression as ParameterExpression;
        }

        private static bool TryWriteTerminal(Expression expression, TextWriter writer)
        {
            if (!Terminals.TryGetValue(expression.NodeType, out var terminal))
            {
                return false;
            }

            writer.Write(terminal);
            return true;
        }

        private string FormatIdentifier(string name)
        {
            return this.UseLowercaseMembers
                ? name.Length == 1
                    ? name.ToLower()
                    : name.Substring(0, 1).ToLower() + name.Substring(1)
                : name;
        }
    }
}