using System;
using System.IO;
using System.Text;

namespace Dryv.SampleVue
{
    public static class DemoExtensions
    {
        public static string GetString(this Action<TextWriter> writerFunc)
        {
            var sb = new StringBuilder();
            using var w = new StringWriter(sb);

            writerFunc(w);

            return sb.ToString();
        }
    }
}