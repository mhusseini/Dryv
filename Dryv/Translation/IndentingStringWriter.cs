using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dryv.Translation
{
    public class IndentingStringWriter : StringWriter
    {
        private int indent;

        private string indentText = string.Empty;

        private bool needsIndent = true;

        public IndentingStringWriter()
        {
        }

        public IndentingStringWriter(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        public IndentingStringWriter(StringBuilder sb) : base(sb)
        {
        }

        public IndentingStringWriter(StringBuilder sb, IFormatProvider formatProvider) : base(sb, formatProvider)
        {
        }

        public void DecrementIndent()
        {
            this.indentText = string.Empty.PadRight(--this.indent * 4, ' ');
        }

        public void IncrementIndent()
        {
            this.needsIndent = true;
            this.indentText = string.Empty.PadRight(++this.indent * 4, ' ');
        }

        public override void Write(bool value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(char value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(char[] buffer)
        {
            this.WriteIndent();
            base.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            this.WriteIndent();
            base.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(double value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(int value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(long value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(object value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(float value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(string value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            this.WriteIndent();
            base.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            this.WriteIndent();
            base.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            this.WriteIndent();
            base.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            this.WriteIndent();
            base.Write(format, arg);
        }

        public override void Write(uint value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override void Write(ulong value)
        {
            this.WriteIndent();
            base.Write(value);
        }

        public override Task WriteAsync(char value)
        {
            this.WriteIndent();
            return base.WriteAsync(value);
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            this.WriteIndent();
            return base.WriteAsync(buffer, index, count);
        }

        public override Task WriteAsync(string value)
        {
            this.WriteIndent();
            return base.WriteAsync(value);
        }

        public override void WriteLine()
        {
            base.WriteLine();
            this.needsIndent = true;
        }

        public override void WriteLine(bool value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(char value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(char[] buffer)
        {
            base.WriteLine(buffer);
            this.needsIndent = true;
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            base.WriteLine(buffer, index, count);
            this.needsIndent = true;
        }

        public override void WriteLine(decimal value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(double value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(int value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(long value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(object value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(float value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(string format, object arg0)
        {
            base.WriteLine(format, arg0);
            this.needsIndent = true;
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            base.WriteLine(format, arg0, arg1);
            this.needsIndent = true;
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            base.WriteLine(format, arg0, arg1, arg2);
            this.needsIndent = true;
        }

        public override void WriteLine(string format, params object[] arg)
        {
            base.WriteLine(format, arg);
            this.needsIndent = true;
        }

        public override void WriteLine(uint value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override void WriteLine(ulong value)
        {
            base.WriteLine(value);
            this.needsIndent = true;
        }

        public override async Task WriteLineAsync()
        {
            await base.WriteLineAsync();
            this.needsIndent = true;
        }

        public override async Task WriteLineAsync(char value)
        {
            await base.WriteLineAsync(value);
            this.needsIndent = true;
        }

        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            await base.WriteLineAsync(buffer, index, count);
            this.needsIndent = true;
        }

        public override async Task WriteLineAsync(string value)
        {
            await base.WriteLineAsync(value);
            this.needsIndent = true;
        }

        private void WriteIndent(bool newNeedsIndent = false)
        {
            if (this.needsIndent)
            {
                base.Write(this.indentText);
            }

            this.needsIndent = newNeedsIndent;
        }
    }
}