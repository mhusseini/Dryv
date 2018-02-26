using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    internal class JavaScriptTranslator : Translator
    {
        private static readonly MemberInfo ErrorMember = typeof(DryvResult).GetMember("Error").First();

        private static readonly MemberInfo SuccessMember = typeof(DryvResult).GetMember("Success").First();

        private static readonly Dictionary<ExpressionType, string> Terminals = new Dictionary<ExpressionType, string>
        {
            //[ExpressionType.Add] = "",
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
            //[ExpressionType.Throw] = "",
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

        public override void Visit(BinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.Visit((dynamic)expression.Left, writer);
            writer.Write(" ");

            if (!TryWriteTerminal(expression, writer) && expression.Method != null)
            {
                writer.Write($"[{this.FormatIdentifier(expression.Method.Name)}]");
            }

            writer.Write(" ");
            this.Visit((dynamic)expression.Right, writer);
        }

        public override void Visit(BlockExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            foreach (var variable in expression.Variables)
            {
                writer.WriteLine($"var {this.FormatIdentifier(variable.Name)};");
            }

            base.Visit(expression, writer);
        }

        public override void Visit(ConditionalExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("(");
            this.Visit((dynamic)expression.Test, writer);
            writer.WriteLine(")");
            writer.IncrementIndent();
            writer.Write(" ? (");
            this.Visit((dynamic)expression.IfTrue, writer);
            writer.WriteLine(")");
            writer.Write(" : (");
            this.Visit((dynamic)expression.IfFalse, writer);
            writer.Write(")");
            writer.DecrementIndent();
        }

        public override void Visit(ConstantExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var text = QuoteValue(expression.Value);

            writer.Write(text);
        }

        public override void Visit(DefaultExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            var value = this.GetDefaultValue(expression.Type);
            var text = QuoteValue(value);

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

            this.Visit((dynamic)expression.Object, writer);
            writer.Write("[");
            this.Visit((dynamic)expression.Arguments.First(), writer);
            writer.Write("]");
        }

        public override void Visit(InvocationExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("(");
            this.Visit((dynamic)expression.Expression, writer);
            writer.Write(")(");
            this.WriteArguments(expression.Arguments, writer);
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
            this.Visit((dynamic)expression.Body, writer);
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
                    this.Visit((dynamic)argument, writer);
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
                return;
            }

            if (expression.Expression != null)
            {
                this.Visit((dynamic)expression.Expression, writer);
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
            switch (expression.Method.Name)
            {
                case nameof(object.Equals):
                    this.Visit((dynamic)expression.Object, writer);
                    writer.Write(negated ? "!==" : " === ");
                    this.WriteArguments(expression.Arguments, writer);
                    break;

                case nameof(string.Contains):
                    this.Visit((dynamic)expression.Object, writer);
                    writer.Write(".indexOf(");
                    this.WriteArguments(expression.Arguments, writer);
                    writer.Write(negated ? ") < 0" : ") >= 0");
                    break;

                case nameof(string.StartsWith):
                    this.Visit((dynamic)expression.Object, writer);
                    writer.Write(".indexOf(");
                    this.WriteArguments(expression.Arguments, writer);
                    writer.Write(negated ? ") !== 0" : ") === 0");
                    break;

                case nameof(string.ToLower):
                    if (!negated)
                    {
                        writer.Write("!(");
                    }
                    this.Visit((dynamic)expression.Object, writer);
                    writer.Write(".toLowerCase()");
                    if (!negated)
                    {
                        writer.Write(")");
                    }
                    break;

                case nameof(string.ToUpper):
                    if (!negated)
                    {
                        writer.Write("!(");
                    }
                    this.Visit((dynamic)expression.Object, writer);
                    writer.Write(".toUpperCase()");
                    if (!negated)
                    {
                        writer.Write(")");
                    }
                    break;

                case nameof(DryvResult.Error):
                    if (expression.Method != ErrorMember)
                    {
                        throw new MethodCallNotAllowed(expression);
                    }

                    writer.Write(expression.Arguments.First());
                    break;
                case nameof(string.IsNullOrEmpty):
                    writer.Write("!(");
                    this.Visit((dynamic)expression.Arguments.First(), writer);
                    writer.Write(")");
                    break;

                case nameof(string.IsNullOrWhiteSpace):
                    writer.Write(@"!/\S/.test((");
                    this.Visit((dynamic)expression.Arguments.First(), writer);
                    writer.Write(@") || """")");
                    break;
                default:
                    throw new MethodCallNotAllowed(expression);
                    //this.Visit((dynamic)expression.Object, writer);
                    //writer.Write(".");
                    //writer.Write(this.FormatIdentifier(expression.Method.Name));
                    //writer.Write("(");
                    //this.WriteArguments(expression.Arguments, writer);
                    //writer.Write(")");
                    //break;
            }
        }

        public override void Visit(NewArrayExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("[]");
        }

        public override void Visit(NewExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            writer.Write("new ");
            writer.Write(expression.Constructor.MemberType);
            writer.Write("(");
            this.WriteArguments(expression.Arguments, writer);
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

            this.Visit((dynamic)expression.Operand, writer, negatedExpression);
        }

        private static string QuoteValue(object value)
        {
            return value == null
                ? "null"
                : (value.GetType().IsPrimitive
                    ? value.ToString()
                    : $@"""{value}""");
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
            return name.Length == 1
                ? name.ToLower()
                : name.Substring(0, 1).ToLower() + name.Substring(1);
        }

        private void WriteArguments(IEnumerable<Expression> arguments, IndentingStringWriter writer)
        {
            var sep = string.Empty;
            foreach (var argument in arguments)
            {
                writer.Write(sep);
                this.Visit((dynamic)argument, writer);
                sep = ", ";
            }
        }
    }
}