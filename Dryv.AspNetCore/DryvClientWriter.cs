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
        private readonly DryvClientValidationLoader loader;
        private readonly IDryvClientValidationSetWriter rulesWriter;

        public DryvClientWriter(DryvClientValidationLoader loader, IDryvClientValidationSetWriter rulesWriter)
        {
            this.loader = loader ?? throw new ArgumentNullException(nameof(loader));
            this.rulesWriter = rulesWriter ?? throw new ArgumentNullException(nameof(rulesWriter));
        }

        public IHtmlContent WriteDryvValidation<TModel>(Func<Type, object> serviceProvider, string validationSetName)
        {
            var validators = this.loader.GetDryvClientValidationFunctions<TModel>();
            var disablers = this.loader.GetDryvClientDisablingFunctions<TModel>();

            return new LazyHtmlContent(writer =>
            {
                this.rulesWriter.WriteBegin(writer, serviceProvider);
                this.rulesWriter.WriteValidationSet(writer, validationSetName, validators, disablers, serviceProvider);
                this.rulesWriter.WriteEnd(writer, serviceProvider);
            });
        }

        public IHtmlContent WriteDryvValidation(Func<Type, object> serviceProvider, params (string, Type)[] validationSets)
        {
            var arguments = from x in validationSets
                            select (
                                name: x.Item1,
                                validators: this.loader.GetDryvClientValidationFunctions(x.Item2),
                                disablers: this.loader.GetDryvClientDisablingFunctions(x.Item2)
                            );

            return this.CreateHtmlContent(arguments, serviceProvider);
        }

        public IHtmlContent WriteDryvValidation(IEnumerable<KeyValuePair<string, Type>> validationSets, Func<Type, object> serviceProvider)
        {
            var arguments = from x in validationSets
                            select (
                                name: x.Key,
                                validators: this.loader.GetDryvClientValidationFunctions(x.Value),
                                disablers: this.loader.GetDryvClientDisablingFunctions(x.Value)
                            );

            return this.CreateHtmlContent(arguments, serviceProvider);
        }

        private IHtmlContent CreateHtmlContent(IEnumerable<(string name, IDictionary<string, Action<Func<Type, object>, TextWriter>> validators, IDictionary<string, Action<Func<Type, object>, TextWriter>> disablers)> arguments, Func<Type, object> serviceProvider)
        {
            return new LazyHtmlContent(writer =>
            {
                this.rulesWriter.WriteBegin(writer, serviceProvider);

                foreach (var (name, validators, disablers) in arguments)
                {
                    this.rulesWriter.WriteValidationSet(writer, name, validators, disablers, serviceProvider);
                }

                this.rulesWriter.WriteEnd(writer, serviceProvider);
            });
        }
    }
}