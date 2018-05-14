using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Markdig;
using Markdig.Extensions.Bootstrap;
using Markdig.Extensions.Emoji;
using Markdig.Renderers;

namespace DryvDemo.HtmlHelpers
{
    public static class MarkdownHtmlHelper
    {
        private static readonly ConcurrentDictionary<string, string> Html = new ConcurrentDictionary<string, string>();

        private static readonly Regex RegexLineSpace = new Regex(@"^\s+", RegexOptions.Compiled);

        public static IHtmlString Markdown(this HtmlHelper helper, string filename)
        {
            var markdown = GetContent(helper, filename);
            var lines = markdown.Split('\n');
            var space = lines.Take(2).Select(l => RegexLineSpace.Match(l)).Select(m => m.Length).FirstOrDefault(l => l > 1);
            markdown = string.Join("\n", lines.Select(l => l.Substring(Math.Min(space, RegexLineSpace.Match(l).Length))));
            var html = Html.GetOrAdd(markdown, CompileMarkdown);

            return new HtmlString(html);
        }

        private static string CompileMarkdown(string markdown)
        {
            var builder = new MarkdownPipelineBuilder().UseAdvancedExtensions();
            builder.Extensions.Add(new BootstrapExtension());
            builder.Extensions.Add(new EmojiExtension());
            var pipeline = builder.Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var htmlRenderer = new HtmlRenderer(writer);
                pipeline.Setup(htmlRenderer);
                htmlRenderer.Render(document);
            }

            return sb.ToString();
        }

        private static string GetContent(HtmlHelper helper, string filename)
        {
            var virtualViewPath = (helper.ViewContext.View as BuildManagerCompiledView)?.ViewPath ?? string.Empty;
            var viewPath = helper.ViewContext.Controller.ControllerContext.HttpContext.Server.MapPath(virtualViewPath);
            var viewDir = Path.GetDirectoryName(viewPath) ?? string.Empty;
            var markdownPath = Path.Combine(viewDir, filename);

            return File.ReadAllText(markdownPath);
        }
    }
}