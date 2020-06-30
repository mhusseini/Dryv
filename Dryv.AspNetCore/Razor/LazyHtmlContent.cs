using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace Dryv.AspNetCore.Razor
{
    public class LazyHtmlContent : IHtmlContent
    {
        private readonly Action<TextWriter> content;

        public LazyHtmlContent(Action<TextWriter> content) => this.content = content;

        public override string ToString()
        {
            var sb = new StringBuilder();
            using var w = new StringWriter(sb);

            this.WriteTo(w, null);

            return sb.ToString();
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            this.content.Invoke(writer);
        }
    }
}