using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dryv.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dryv.AspNetCore.TagHelpers
{
    [HtmlTargetElement("dryv-client-rules", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class DryvClientRulesTagHelper : TagHelper
    {
        private readonly DryvClientWriter clientWriter;

        public DryvClientRulesTagHelper(DryvClientWriter clientWriter)
        {
            this.clientWriter = clientWriter;
        }

        [HtmlAttributeName("for")]
        public object Model { get; set; }

        [HtmlAttributeName("name")]
        public string ValidationSetName { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public IReadOnlyDictionary<string, object> Parameters { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var modelTypes = this.GetModelTypes();
            if (!modelTypes.Any())
            {
                throw new InvalidOperationException("The 'for' attribute must have a non-empty value or the view must have a model type.");
            }

            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;

            var content = await this.clientWriter.WriteDryvValidation(modelTypes, this.ViewContext.HttpContext.RequestServices.GetService, this.Parameters);
            
            output.Content.AppendHtml(content);
        }

        private Dictionary<string, Type> GetModelTypes()
        {
            var items = new Dictionary<string, object>();
            var itemTypes = new Dictionary<string, Type>();

            switch (this.Model)
            {
                case null:
                    {
                        var type = this.ViewContext.ViewData.ModelMetadata.ModelType;
                        var name = this.GetSingleValidationSetName(type);
                        itemTypes.Add(name, type);
                        break;
                    }
                case Type type:
                    {
                        var name = this.GetSingleValidationSetName(type);
                        itemTypes.Add(name, type);
                        break;
                    }

                case Tuple<string, Type> type:
                    itemTypes.Add(type.Item1, type.Item2);
                    break;

                case Tuple<string, object> model:
                    items.Add(model.Item1, model.Item2);
                    break;

                case IEnumerable<KeyValuePair<string, Type>> types:
                    itemTypes.AddRange(types);
                    break;

                case IEnumerable<(string, Type)> types:
                    itemTypes.AddRange(types.Select(i => new KeyValuePair<string, Type>(i.Item1, i.Item2)));
                    break;

                case IEnumerable<(string, object)> models:
                    items.AddRange(models.Select(i => new KeyValuePair<string, object>(i.Item1, i.Item2)));
                    break;

                case IEnumerable<KeyValuePair<string, object>> models:
                    items.AddRange(models);
                    break;

                case IEnumerable<object> models:
                    items.AddRange(models.Select(m => new KeyValuePair<string, object>(m.GetType().FullName, m)));
                    break;

                case { } model:
                    {
                        var name = this.ValidationSetName ?? model.GetType().FullName;
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            throw new InvalidOperationException($"The type specified in the 'for' attribute does not have a name and the 'name' attribute is not specified, either.");
                        }

                        items.Add(name, model);
                        break;
                    }
            }

            itemTypes.AddRange(items.Select(i => new KeyValuePair<string, Type>(i.Key, i.Value.GetType())));

            return itemTypes;
        }

        private string GetSingleValidationSetName(Type type)
        {
            var name = this.ValidationSetName ?? type.FullName;

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException($"The type specified in the 'for' attribute does not have a name and the 'name' attribute is not specified, either.");
            }

            return name;
        }
    }
}