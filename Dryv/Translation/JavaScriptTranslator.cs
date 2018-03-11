using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.MethodCallTranslation;

namespace Dryv.Translation
{
    internal class JavaScriptTranslator : Translator
    {
        private static readonly MethodCallTranslator MethodCallTranslator = new MethodCallTranslator();

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

        public bool UseLowercaseMembers { get; set; }

        public override object Visit(BinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.VisitWithBrackets(expression.Left, writer);

            if (!TryWriteTerminal(expression, writer))
            {
                throw expression.Method != null
                    ? (Exception)new MethodCallNotAllowedException(expression)
                    : new ExpressionTypeNotSupportedException(expression);
            }

            this.VisitWithBrackets(expression.Right, writer);
            return null;
        }

        public override object Visit(BlockExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            foreach (var variable in expression.Variables)
            {
                writer.WriteLine($"var {this.FormatIdentifier(variable.Name)};");
            }

            base.Visit(expression, writer, negated);
            return null;
        }

        public override object Visit(ConditionalExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.VisitWithBrackets(expression.Test, writer);
            writer.IncrementIndent();
            writer.Write(" ? ");
            this.VisitWithBrackets(expression.IfTrue, writer);
            writer.Write(" : ");
            this.VisitWithBrackets(expression.IfFalse, writer);
            writer.DecrementIndent();
            return null;
        }

        public override object Visit(ConstantExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var text = MethodCallTranslator.QuoteValue(expression.Value);

            writer.Write(text);
            return null;
        }

        public override object Visit(DefaultExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var value = this.GetDefaultValue(expression.Type);
            var text = MethodCallTranslator.QuoteValue(value);

            writer.Write(text);
            return null;
        }

        public override object Visit(DynamicExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override object Visit(GotoExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override object Visit(IDynamicExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override object Visit(IndexExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            if (expression.Arguments.Count > 1)
            {
                throw new NotSupportedException("JavaScript does not support indexers with more than one argument.");
            }

            this.Visit((dynamic)expression.Object, writer);
            writer.Write("[");
            this.Visit((dynamic)expression.Arguments.First(), writer);
            writer.Write("]");
            return null;
        }

        public override object Visit(InvocationExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.VisitWithBrackets(expression.Expression, writer);
            writer.Write("(");
            MethodCallTranslator.WriteArguments(this, expression.Arguments, writer);
            writer.Write(")");
            return null;
        }

        public override object Visit(LabelExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override object Visit(LambdaExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("function(");
            writer.Write(string.Join(", ", expression.Parameters.Select(p => this.FormatIdentifier(p.Name))));
            writer.WriteLine(") {");
            writer.IncrementIndent();
            writer.Write("return ");
            this.Visit((dynamic)expression.Body, writer);
            writer.WriteLine(";");
            writer.DecrementIndent();
            writer.Write("}");
            return null;
        }

        public override object Visit(ListInitExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("= [");
            var sep = string.Empty;
            foreach (var initializer in expression.Initializers)
            {
                writer.Write(sep);
                foreach (var argument in initializer.Arguments)
                {
                    this.Visit((dynamic)argument, writer);
                }
                sep = ", ";
            }
            writer.Write("]");
            return null;
        }

        public override object Visit(LoopExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override object Visit(MemberExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            if (expression.Member == SuccessMember)
            {
                writer.Write("null");
                return null;
            }

            if (expression.Expression != null)
            {
                var ignoredMember = this.Visit((dynamic)expression.Expression, writer);
                if (ignoredMember == expression.Member.Name)
                {
                    return null;
                }

                writer.Write(".");
            }

            writer.Write(this.FormatIdentifier(expression.Member.Name));
            return null;
        }

        public override object Visit(MemberInitExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.Visit(expression.NewExpression, writer);
            return null;
        }

        public override object Visit(MethodCallExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var options = new MethodTranslationOptions
            {
                Translator = this,
                Expression = expression,
                Writer = writer,
                Negated = negated
            };

            MethodCallTranslator.Translate(options);

            return options.Result;
        }

        public override object Visit(NewArrayExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("[]");
            return null;
        }

        public override object Visit(NewExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("new ");
            writer.Write(expression.Constructor.MemberType);
            writer.Write("(");
            MethodCallTranslator.WriteArguments(this, expression.Arguments, writer);
            writer.Write(")");
            return null;
        }

        public override object Visit(ParameterExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write(expression.Name);
            return null;
        }

        public override object Visit(RuntimeVariablesExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override object Visit(SwitchExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("switch(");
            this.Visit((dynamic)expression.SwitchValue, writer);
            writer.WriteLine("){");
            foreach (var expressionCase in expression.Cases)
            {
                foreach (var testCase in expressionCase.TestValues)
                {
                    writer.Write("case ");
                    this.Visit((dynamic)testCase, writer);
                    writer.WriteLine(":");
                }

                writer.WriteLine("{");
                writer.IncrementIndent();
                this.Visit((dynamic)expressionCase.Body, writer);
                writer.WriteLine();
                writer.WriteLine("break;");
                writer.DecrementIndent();
                writer.WriteLine("}");
            }
            writer.Write("}");
            return null;
        }

        public override object Visit(TryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotImplementedException();
        }

        public override object Visit(TypeBinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            throw new NotSupportedException();
        }

        public override object Visit(UnaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var negatedExpression = expression.NodeType == ExpressionType.Not;
            if (!negatedExpression)
            {
                TryWriteTerminal(expression, writer);
            }

            this.Visit((dynamic)expression.Operand, writer, negatedExpression);
            return null;
        }

        public object VisitWithBrackets(Expression expression, IndentingStringWriter writer)
        {
            var needsBrackets = GetNeedsBrackets(expression);

            if (needsBrackets)
            {
                writer.Write("(");
            }

            this.Visit((dynamic)expression, writer);

            if (needsBrackets)
            {
                writer.Write(") ");
            }
            return null;
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