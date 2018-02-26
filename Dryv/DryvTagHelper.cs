using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv
{
    [HtmlTargetElement(Attributes = AttributeName)]
    public class DryvTagHelper : TagHelper
    {
        public const string AttributeName = "asp-for";

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var model = context.AllAttributes.Where(a => a.Name == AttributeName).Select(a => a.Value).OfType<ModelExpression>().FirstOrDefault();
            if (model == null)
            {
                return Task.CompletedTask;
            }

            var objectType = model.Metadata.ContainerType;
            var allRulesForType = (from p in objectType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                   where typeof(Rules).IsAssignableFrom(p.FieldType)
                                   select p.GetValue(null) as Rules).Union(
                from p in objectType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where typeof(Rules).IsAssignableFrom(p.PropertyType)
                select p.GetValue(null) as Rules);

            var propertyName = model.Name;
            var rulesForProperty = from rules in allRulesForType
                                   where rules.PropertyRules.ContainsKey(propertyName)
                                   from expression in rules.PropertyRules[propertyName]
                                   select expression;

            var translator = new JavaScriptTranslator();
            var javaScriptRulesForProperty = from rule in rulesForProperty
                                             select translator.Translate(rule);

            var tagBuilder = new TagBuilder("script");
            tagBuilder.InnerHtml.AppendHtml(string.Join(Environment.NewLine, javaScriptRulesForProperty));

            output.PostElement.AppendHtml(tagBuilder);
            return Task.CompletedTask;
        }
    }
}