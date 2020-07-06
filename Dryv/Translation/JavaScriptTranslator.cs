using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dryv.Extensions;
using Dryv.Reflection;

namespace Dryv.Translation
{
    public class JavaScriptTranslator : Translator
    {
        private static readonly MethodInfo DryvValidationResultImplicitConvert = typeof(DryvValidationResult).GetMethod("op_Implicit");

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

        public override void Translate(Expression expression, TranslationContext context, bool negated = false)
        {
            var needsBrackets = this.GetNeedsBrackets(expression);

            if (needsBrackets)
            {
                context.Writer.Write("(");
            }

            var context2 = context.Clone<CustomTranslationContext>();
            context2.Expression = expression;
            context2.Translator = this;
            context2.Negated = negated;

            if (!this.translatorProvider.GenericTranslators.Any(t => t.TryTranslate(context2)))
            {
                this.Visit((dynamic)expression, context2, negated);
            }

            if (needsBrackets)
            {
                context.Writer.Write(") ");
            }
        }

        public override string TranslateValue(object value)
        {
            switch (value)
            {
                case string txt:
                    return $"\"{txt}\"";

                case bool b:
                    return b ? "true" : "false";

                case null:
                    return "null";

                default:
                    return value.ToString();
            }
        }

        public override void Visit(BinaryExpression expression, TranslationContext context, bool negated = false)
        {
            var closingCode = new List<string>();
            var binaryExpression = Expression.MakeBinary(expression.NodeType,
                this.TranslateAsyncBinary(expression.Left, context, closingCode),
                this.TranslateAsyncBinary(expression.Right, context, closingCode),
                expression.IsLiftedToNull,
                expression.Method);

            if (!TryWriteInjectedExpression(binaryExpression.Left, context))
            {
                this.Translate(binaryExpression.Left, context);
            }

            if (!TryWriteTerminal(binaryExpression, context.Writer))
            {
                throw expression.Method != null
                    ? (Exception)new DryvMethodNotSupportedException(expression)
                    : new DryvExpressionNotSupportedException(expression);
            }

            if (!TryWriteInjectedExpression(binaryExpression.Right, context))
            {
                this.Translate(binaryExpression.Right, context);
            }

            foreach (var code in closingCode)
            {
                context.Writer.Write(code);
            }
        }

        public override void Visit(BlockExpression expression, TranslationContext context, bool negated = false)
        {
            if (TryWriteInjectedExpression(expression, context))
            {
                return;
            }

            foreach (var variable in expression.Variables)
            {
                context.Writer.WriteLine($"var {this.FormatIdentifier(variable.Name)};");
            }

            base.Visit(expression, context, negated);
        }

        public override void Visit(ConditionalExpression expression, TranslationContext context, bool negated = false)
        {
            context.IsAsync = false;

            if (!TryWriteInjectedExpression(expression.Test, context))
            {
                this.Translate(expression.Test, context);
            }

            if (context.IsAsync)
            {
                var paramName = context.GetVirtualParameter();

                context.Writer.Write(".then(function(");
                context.Writer.Write(paramName);
                context.Writer.Write("){ return ");

                if (expression.Test is UnaryExpression unaryExpression)
                {
                    if (unaryExpression.NodeType == ExpressionType.Not)
                    {
                        context.Writer.Write("!");
                        context.Writer.Write(paramName);
                    }
                    else
                    {
                        var parameter = Expression.Parameter(unaryExpression.Operand.Type, paramName);
                        var exp = Expression.MakeUnary(unaryExpression.NodeType, parameter, unaryExpression.Type);
                        this.Translate(exp, context);
                    }
                }
                else
                {
                    context.Writer.Write(paramName);
                }
            }

            context.Writer.IncrementIndent();
            context.Writer.Write(" ? ");
            this.Translate(expression.IfTrue, context);
            context.Writer.Write(" : ");
            this.Translate(expression.IfFalse, context);
            context.Writer.DecrementIndent();

            if (context.IsAsync)
            {
                context.Writer.Write(";})");
            }
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

        public override void Visit(IndexExpression expression, TranslationContext context, bool negated = false)
        {
            if (expression.Arguments.Count > 1)
            {
                throw new NotSupportedException("JavaScript does not support indexers with more than one argument.");
            }

            if (TryWriteInjectedExpression(expression, context))
            {
                return;
            }

            this.Translate(expression.Object, context);
            context.Writer.Write("[");
            this.Translate(expression.Arguments.First(), context);
            context.Writer.Write("]");
        }

        //public override void Visit(IDynamicExpression expression, TranslationContext context, bool negated = false)
        //{
        //    throw new NotSupportedException();
        //}
        public override void Visit(InvocationExpression expression, TranslationContext context, bool negated = false)
        {
            if (expression.Expression is MemberExpression)
            {
                throw new DryvExpressionNotSupportedException(expression);
            }

            if (TryWriteInjectedExpression(expression, context))
            {
                return;
            }

            this.Translate(expression.Expression, context);
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
            this.Translate(expression.Body, context);
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
                    this.Translate(argument, context);
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
            if (negated)
            {
                context.Writer.Write("!(");
            }

            this.WriteMember(expression, context);

            if (negated)
            {
                context.Writer.Write(")");
            }
        }

        public override void Visit(MemberInitExpression expression, TranslationContext context, bool negated = false)
        {
            this.Visit(expression.NewExpression, context);
        }

        public override void Visit(MethodCallExpression expression, TranslationContext context, bool negated = false)
        {
            if (expression.Method.IsStatic && expression.Arguments.Any() != true)
            {
                var value = expression.Method.Invoke(null, null);
                context.Writer.Write(this.TranslateValue(value));
                return;
            }

            if (TryWriteInjectedMethod(expression, context))
            {
                return;
            }

            var objectType = expression.Object?.Type ?? expression.Method.DeclaringType;

            var context2 = context.Clone<MethodTranslationContext>();
            context2.Translator = this;
            context2.Expression = expression;
            context2.Negated = negated;

            if (this.translatorProvider
                .MethodCallTranslators
                .Where(t => t.SupportsType(objectType))
                .Any(t => t.Translate(context2)))
            {
                return;
            }

            throw new DryvMethodNotSupportedException(expression);
        }

        public override void Visit(NewArrayExpression expression, TranslationContext context, bool negated = false)
        {
            context.Writer.Write("[");
            foreach (var child in expression.Expressions)
            {
                this.Translate(child, context);
            }
            context.Writer.Write("]");
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
            if (!TryWriteInjectedExpression(expression.SwitchValue, context))
            {
                this.Translate(expression.SwitchValue, context);
            }
            context.Writer.WriteLine("){");
            foreach (var expressionCase in expression.Cases)
            {
                foreach (var testCase in expressionCase.TestValues)
                {
                    context.Writer.Write("case ");
                    this.Translate(testCase, context);
                    context.Writer.WriteLine(":");
                }

                context.Writer.WriteLine("{");
                context.Writer.IncrementIndent();
                this.Translate(expressionCase.Body, context);
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
            throw new DryvExpressionNotSupportedException(expression);
        }

        public override void Visit(UnaryExpression expression, TranslationContext context, bool negated = false)
        {
            if (TryWriteInjectedExpression(expression, context))
            {
                return;
            }

            var negatedExpression = false;

            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                    negatedExpression = true;
                    break;

                case ExpressionType.Convert:
                    if (string.IsNullOrWhiteSpace(context.GroupName) || !Equals(expression.Method, DryvValidationResultImplicitConvert))
                    {
                        break;
                    }

                    // TODO: Code smell! This class is too low level for this code. Move code to an interface.
                    context.Writer.Write("{ type:\"error\", text:");
                    this.Translate(expression.Operand, context);
                    context.Writer.Write(", groupName: ");
                    context.Writer.Write(MethodCallTranslator.QuoteValue(context.GroupName));
                    context.Writer.Write("}");
                    return;
            }

            if (!negatedExpression)
            {
                TryWriteTerminal(expression, context.Writer);
            }

            this.Translate(expression.Operand, context, negatedExpression);
        }

        private static bool TryWriteInjectedExpression(Expression expression, TranslationContext context)
        {
            var visitor = new ExpressionNodeFinder<ParameterExpression>();
            visitor.Visit(expression);

            // Parameters that start with '$' are generated dummy parameters and should not be injected.
            if (visitor.FoundChildren.Any(c => c.Name.StartsWith("$") || c.Type == context.ModelType))
            {
                return false;
            }

            context.InjectRuntimeExpression(expression, visitor.FoundChildren);

            return true;
        }

        private static bool TryWriteInjectedMethod(MethodCallExpression expression, TranslationContext context)
        {
            if (!expression.Type.IsSystemType())
            {
                return false;
            }

            var parameter = expression.Object.GetOuterExpression<ParameterExpression>();
            if (parameter != null && !context.OptionsTypes.Contains(parameter.Type))
            {
                return false;
            }

            if (parameter == null && !expression.Method.IsStatic)
            {
                return false;
            }

            var parameterExpressions = (from a in expression.Arguments
                                        let p = a.GetOuterExpression<ParameterExpression>()
                                        where p != null
                                        select p).ToList();

            if (parameterExpressions.Any(p => p.Type == context.ModelType))
            {
                return false;
            }

            if (parameter == null && parameterExpressions.Any())
            {
                parameter = parameterExpressions.FirstOrDefault(p => context.OptionsTypes.Contains(p.Type));
            }

            if (parameter == null)
            {
                return false;
            }

            context.InjectRuntimeExpression(expression.Object ?? expression, parameter);
            return true;
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

        private bool GetNeedsBrackets(Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression _:
                case MethodCallExpression _:
                case MemberExpression _:
                case UnaryExpression _:
                case LambdaExpression _:
                    return false;
            }

            return this.translatorProvider.GenericTranslators.All(t => t.AllowSurroundingBrackets(expression) != false);
        }

        private Expression TranslateAsyncBinary(Expression expression, TranslationContext context, ICollection<string> closingCode)
        {
            var sb = new StringBuilder();
            var context2 = context.Clone<TranslationContext>(sb);
            context2.IsAsync = false;

            if (TryWriteInjectedExpression(expression, context2))
            {
                return expression;
            }

            this.Translate(expression, context2);

            if (!context2.IsAsync)
            {
                return expression;
            }

            context.Writer.Write(sb.ToString());

            var parameter = context2.GetVirtualParameter();

            context.Writer.Write(".then(function(");
            context.Writer.Write(parameter);
            context.Writer.Write("){ return ");

            closingCode.Add(";})");

            return Expression.Parameter(expression.Type, parameter);
        }

        private void WriteMember(MemberExpression expression, TranslationContext context)
        {
            if (TryWriteInjectedExpression(expression, context))
            {
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
                            break;
                        }
                    case FieldInfo field:
                        {
                            var value = field.GetValue(instance);
                            context.Writer.Write(this.TranslateValue(value));
                            break;
                        }
                }
            }
            else
            {
                if (context.PropertyExpression != null &&
                    expression.Expression.ToString().Contains(context.PropertyExpression.ToString()))
                {
                    var e = expression;

                    switch (e.Expression)
                    {
                        case MemberExpression mex:
                            this.Visit(mex, context);
                            break;

                        case ParameterExpression parameterExpression:
                            this.Visit(parameterExpression, context);
                            context.Writer.Write("$$MODELPATH$$");
                            break;

                        default:
                            this.Translate(expression.Expression, context);
                            break;
                    }
                }
                else
                {
                    this.Translate(expression.Expression, context);
                }

                context.Writer.Write(".");

                context.Writer.Write(this.FormatIdentifier(expression.Member.Name.ToCamelCase()));
            }
        }
    }
}