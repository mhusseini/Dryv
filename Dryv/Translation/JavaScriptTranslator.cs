using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.DependencyInjection;
using Dryv.MethodCallTranslation;

namespace Dryv.Translation
{
    internal class JavaScriptTranslator : Translator
    {
        private static readonly MemberInfo SuccessMember = typeof(DryvResult).GetMember("Success").First();

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

        private readonly IMethodCallTranslator methodCallTranslator;
        private readonly ITranslatorProvider translatorProvider;

        public JavaScriptTranslator(IMethodCallTranslator methodCallTranslator,
            ITranslatorProvider translatorProvider)
        {
            this.methodCallTranslator = methodCallTranslator;
            this.translatorProvider = translatorProvider;
        }

        public bool UseLowercaseMembers { get; set; }

        public override void Visit(BinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.VisitWithBrackets(expression.Left, writer);

            if (!TryWriteTerminal(expression, writer))
            {
                throw expression.Method != null
                    ? (Exception)new MethodCallNotAllowedException(expression)
                    : new ExpressionTypeNotSupportedException(expression);
            }

            this.VisitWithBrackets(expression.Right, writer);
        }

        public override void Visit(BlockExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            foreach (var variable in expression.Variables)
            {
                writer.WriteLine($"var {this.FormatIdentifier(variable.Name)};");
            }

            base.Visit(expression, writer, negated);
        }

        public override void Visit(ConditionalExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.VisitWithBrackets(expression.Test, writer);
            writer.IncrementIndent();
            writer.Write(" ? ");
            this.VisitWithBrackets(expression.IfTrue, writer);
            writer.Write(" : ");
            this.VisitWithBrackets(expression.IfFalse, writer);
            writer.DecrementIndent();
        }

        public override void Visit(ConstantExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var text = MethodCallTranslator.QuoteValue(expression.Value);

            writer.Write(text);
        }

        public override void Visit(DefaultExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var value = this.GetDefaultValue(expression.Type);
            var text = MethodCallTranslator.QuoteValue(value);

            writer.Write(text);
        }

        public override void Visit(DynamicExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(GotoExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(IDynamicExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(IndexExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            if (expression.Arguments.Count > 1)
            {
                throw new NotSupportedException("JavaScript does not support indexers with more than one argument.");
            }

            this.Visit(expression.Object, writer);
            writer.Write("[");
            this.Visit(expression.Arguments.First(), writer);
            writer.Write("]");
        }

        public override void Visit(Expression expression, IndentingStringWriter writer, bool negated = false)
        {
            var parameters = new TranslationParameters
            {
                Expression = expression,
                Translator = this,
                Writer = writer,
                Negated = negated
            };

            if (!this.translatorProvider.GenericTranslators.Any(t => t.TryTranslate(parameters)))
            {
                this.Visit((dynamic)expression, writer, negated);
            }
        }

        public override void Visit(InvocationExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.VisitWithBrackets(expression.Expression, writer);
            writer.Write("(");
            MethodCallTranslator.WriteArguments(this, expression.Arguments, writer);
            writer.Write(")");
        }

        public override void Visit(LabelExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(LambdaExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("function(");
            writer.Write(string.Join(", ", expression.Parameters.Select(p => this.FormatIdentifier(p.Name))));
            writer.WriteLine(") {");
            writer.IncrementIndent();
            writer.Write("return ");
            this.Visit(expression.Body, writer);
            writer.WriteLine(";");
            writer.DecrementIndent();
            writer.Write("}");
        }

        public override void Visit(ListInitExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("= [");
            var sep = string.Empty;
            foreach (var initializer in expression.Initializers)
            {
                writer.Write(sep);
                foreach (var argument in initializer.Arguments)
                {
                    this.Visit(argument, writer);
                }
                sep = ", ";
            }
            writer.Write("]");
        }

        public override void Visit(LoopExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override void Visit(MemberExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            if (expression.Member == SuccessMember)
            {
                writer.Write("null");
            }

            if (expression.Expression != null)
            {
                this.Visit(expression.Expression, writer);
                writer.Write(".");
            }

            writer.Write(this.FormatIdentifier(expression.Member.Name));
        }

        public override void Visit(MemberInitExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.Visit(expression.NewExpression, writer);
        }

        public override void Visit(MethodCallExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var parameters = new MethodTranslationParameters
            {
                Translator = this,
                Expression = expression,
                Writer = writer,
                Negated = negated
            };

            this.methodCallTranslator.Translate(parameters);
        }

        public override void Visit(NewArrayExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("[]");
        }

        public override void Visit(NewExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("new ");
            writer.Write(expression.Constructor.DeclaringType.Name);
            writer.Write("(");
            MethodCallTranslator.WriteArguments(this, expression.Arguments, writer);
            writer.Write(")");
        }

        public override void Visit(ParameterExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write(expression.Name);
        }

        public override void Visit(RuntimeVariablesExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override void Visit(SwitchExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("switch(");
            this.Visit(expression.SwitchValue, writer);
            writer.WriteLine("){");
            foreach (var expressionCase in expression.Cases)
            {
                foreach (var testCase in expressionCase.TestValues)
                {
                    writer.Write("case ");
                    this.Visit(testCase, writer);
                    writer.WriteLine(":");
                }

                writer.WriteLine("{");
                writer.IncrementIndent();
                this.Visit(expressionCase.Body, writer);
                writer.WriteLine();
                writer.WriteLine("break;");
                writer.DecrementIndent();
                writer.WriteLine("}");
            }
            writer.Write("}");
        }

        public override void Visit(TryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override void Visit(TypeBinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override void Visit(UnaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var negatedExpression = expression.NodeType == ExpressionType.Not;
            if (!negatedExpression)
            {
                TryWriteTerminal(expression, writer);
            }

            this.Visit(expression.Operand, writer, negatedExpression);
        }

        public override void VisitWithBrackets(Expression expression, IndentingStringWriter writer)
        {
            var needsBrackets = GetNeedsBrackets(expression);

            if (needsBrackets)
            {
                writer.Write("(");
            }

            this.Visit(expression, writer);

            if (needsBrackets)
            {
                writer.Write(") ");
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