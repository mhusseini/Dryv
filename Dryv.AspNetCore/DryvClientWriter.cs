using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dryv.AspNetCore.Razor;
using Dryv.Validation;
using Microsoft.AspNetCore.Html;

namespace Dryv.AspNetCore
{
    public class DryvClientWriter
    {
        private readonly DryvTranslator translator;
        private readonly IDryvClientValidationFunctionWriter functionWriter;
        private readonly IDryvClientValidationSetWriter setWriter;

        public DryvClientWriter(DryvTranslator translator, IDryvClientValidationFunctionWriter functionWriter, IDryvClientValidationSetWriter setWriter)
        {
            this.translator = translator ?? throw new ArgumentNullException(nameof(translator));
            this.functionWriter = functionWriter ?? throw new ArgumentNullException(nameof(functionWriter));
            this.setWriter = setWriter ?? throw new ArgumentNullException(nameof(setWriter));
        }

        public IHtmlContent WriteDryvValidation<TModel>(string validationSetName, Func<Type, object> serviceProvider)
        {
            var translation = this.translator.TranslateValidationRules(typeof(TModel), serviceProvider);
            var parameters = translation.Parameters;
            var validators = translation.ValidationFunctions.ToDictionary(i => i.Key, i => this.functionWriter.GetValidationFunction(i.Value));
            var disablers = translation.DisablingFunctions.ToDictionary(i => i.Key, i => this.functionWriter.GetValidationFunction(i.Value));

            return new LazyHtmlContent(writer =>
            {
                this.setWriter.WriteBegin(writer);
                this.setWriter.WriteValidationSet(writer, validationSetName, validators, disablers, parameters);
                this.setWriter.WriteEnd(writer);
            });
        }

        public IHtmlContent WriteDryvValidation(IEnumerable<KeyValuePair<string, Type>> validationSets, Func<Type, object> serviceProvider)
        {
            var resultSet = new Dictionary<string, (Dictionary<string, Action<TextWriter>> validators, Dictionary<string, Action<TextWriter>> disablers, Dictionary<string, object> parameters)>();

            foreach (var (setName, type) in validationSets)
            {
                var translation = this.translator.TranslateValidationRules(type, serviceProvider);
                var parameters = translation.Parameters;
                var validators = translation.ValidationFunctions.ToDictionary(i => i.Key, i => this.functionWriter.GetValidationFunction(i.Value));
                var disablers = translation.DisablingFunctions.ToDictionary(i => i.Key, i => this.functionWriter.GetValidationFunction(i.Value));

                resultSet.Add(setName, (validators, disablers, parameters));
            }

            return new LazyHtmlContent(writer =>
            {
                this.setWriter.WriteBegin(writer);

                foreach (var (setName, (validators, disablers, parameters)) in resultSet)
                {
                    this.setWriter.WriteValidationSet(writer, setName, validators, disablers, parameters);
                }

                this.setWriter.WriteEnd(writer);
            });
        }
    }
}