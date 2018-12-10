using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv.AspNetCore.TagHelpers
{
    [HtmlTargetElement(Attributes = "dryv-vee")]
    public class DryvVeeTagHelper : TagHelper
    {
        private delegate void MapAction(TagHelperContext context, TagHelperOutput output, TagHelperAttribute attribute);

        private static readonly Dictionary<string, MapAction> AttributeMapping = new Dictionary<string, MapAction>(StringComparer.OrdinalIgnoreCase)
        {
            ["data-val"] = Actions.Remove,
            ["data-val-required"] = Actions.Replace("required"),
            ["data-val-email"] = Actions.Replace("email"),
            ["data-val-creditcard"] = Actions.Replace("credit_card"),
            ["data-val-phone"] = Actions.Replace("phone"),
            ["data-val-length"] = Actions.Replace("length", "length-max"),
            ["data-val-minlength"] = Actions.Replace("min", "minlength-min"),
            ["data-val-maxlength"] = Actions.Replace("max", "maxlength-max"),
            ["data-val-range"] = Actions.Replace("between", "range-min", "range-max"),
            ["data-val-regex"] = Actions.Replace("regex", new[] { "regex-pattern" }, v => $"/{v}/"),
            ["data-val-remote"] = Actions.Remote("dryv-remote", "remote-url", "remote-additionalfields")
        };

        public override int Order { get; } = int.MaxValue;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var l = output.Attributes.ToList();

            foreach (var attribute in l)
            {
                if (AttributeMapping.TryGetValue(attribute.Name, out var action))
                {
                    action(context, output, attribute);
                }
            }

            output.Attributes.RemoveAll("dryv-vee");
        }

        private static class Actions
        {
            public static readonly MapAction Remove = (context, output, attr) => output.Attributes.Remove(attr);

            public static MapAction Replace(string name, params string[] args) => Replace(name, args, null);

            public static MapAction Replace(string name, string[] args, Func<object, string> formatter) => (context, output, attribute) =>
            {
                var allAttributes = output.Attributes;
                var (value, parameterAttributes) = CreateVeeExpression(name, args, null, formatter, allAttributes);

                allAttributes.Remove(attribute);
                allAttributes.RemoveAll(parameterAttributes);
                allAttributes.SetAttribute(new TagHelperAttribute("v-validate", value));
            };


            public static MapAction Remote(string name, params string[] args) => (context, output, attribute) =>
            {
                var allAttributes = output.Attributes;
                var mid = (from a in allAttributes
                           where a.Name.Equals("dryv-v", StringComparison.OrdinalIgnoreCase)
                           select a.Value?.ToString()).FirstOrDefault();
                if (mid == null)
                {
                    mid = $"v{Math.Abs(attribute.GetHashCode())}";
                    allAttributes.Add("dryv-v", mid);
                }

                var (value, parameterAttributes) = CreateVeeExpression(name, args, mid, null, allAttributes);

                allAttributes.Remove(attribute);
                allAttributes.RemoveAll(parameterAttributes);
                allAttributes.SetAttribute(new TagHelperAttribute("v-validate", value));
            };

            private static (string value, List<TagHelperAttribute> parameterAttributes) CreateVeeExpression(
                string name,
                IEnumerable<string> relParams,
                string customParams,
                Func<object, string> formatter,
                ICollection<TagHelperAttribute> allAttributes)
            {
                if (formatter == null)
                {
                    formatter = v => v?.ToString();
                }

                var d = allAttributes.ToDictionary(a => a.Name, a => a, StringComparer.OrdinalIgnoreCase);
                var parameterAttributes = relParams.Select(a => d.GetValueOrDefault($"data-val-{a}")).ToList();
                var parameterValues = parameterAttributes.Select(a => formatter(a?.Value)).ToList();
                if (!string.IsNullOrWhiteSpace(customParams))
                {
                    parameterValues.Insert(0, customParams);
                }

                var parameters = string.Join(",", parameterValues);

                if (!string.IsNullOrWhiteSpace(parameters))
                {
                    parameters = $":{parameters}";
                }

                string existing = null;

                if (d.TryGetValue("v-validate", out var existingAttribute))
                {
                    var text = existingAttribute.Value.ToString().Trim();
                    if (text.Length > 1)
                    {
                        existing = $"{text.Substring(1, text.Length - 2)}|";
                    }

                    allAttributes.Remove(existingAttribute);
                }

                var value = $"'{existing}{name}{parameters}'";
                return (value, parameterAttributes);
            }
        }
    }
}