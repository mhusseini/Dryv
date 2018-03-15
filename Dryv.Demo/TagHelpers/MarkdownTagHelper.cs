using Markdig;
using Markdig.Extensions.Bootstrap;
using Markdig.Extensions.Emoji;
using Markdig.Renderers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dryv.Demo.TagHelpers
{
    [HtmlTargetElement("p", Attributes = "markdown")]
    [HtmlTargetElement("div", Attributes = "markdown")]
    [HtmlTargetElement("markdown")]
    [OutputElementHint("p")]
    public class MarkdownTagHelper : TagHelper, IHostingEnvironmentAwareViewComponent
    {
        private static readonly ConcurrentDictionary<string, string> Html = new ConcurrentDictionary<string, string>();

        private static readonly Regex RegexLineSpace = new Regex(@"^\s+", RegexOptions.Compiled);

        public MarkdownTagHelper(IHostingEnvironment hostingEnvironment)
        {
            this.HostingEnvironment = hostingEnvironment;
        }

        public ModelExpression Content { get; set; }
        public string File { get; set; }

        public IHostingEnvironment HostingEnvironment { get; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output.TagName == "markdown")
            {
                output.TagName = null;
            }

            output.Attributes.RemoveAll("markdown");

            var markdown = await this.GetContent(output);
            var lines = markdown.Split('\n');
            var space = lines.Take(2).Select(l => RegexLineSpace.Match(l)).Select(m => m.Length).FirstOrDefault(l => l > 1);
            markdown = string.Join("\n", lines.Select(l => l.Substring(Math.Min(space, RegexLineSpace.Match(l).Length))));
            var html = Html.GetOrAdd(markdown, CompileMarkdown);

            output.TagName = context.TagName;
            output.Content.SetHtmlContent(html);
        }

        private static string CompileMarkdown(string markdown)
        {
            var builder = new MarkdownPipelineBuilder().UseAdvancedExtensions();
            builder.Extensions.Add(new BootstrapExtension());
            builder.Extensions.Add(new EmojiExtension());
            var pipeline = builder.Build();
            var document = Markdown.Parse(markdown, pipeline);

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var htmlRenderer = new HtmlRenderer(writer);
                pipeline.Setup(htmlRenderer);
                htmlRenderer.Render(document);
            }

            return sb.ToString();
        }

        private async Task<string> GetContent(TagHelperOutput output) =>
            this.Content?.Model?.ToString()
            ?? this.ReadFile(this.File)
            ?? (await output.GetChildContentAsync()).GetContent();
    }
}