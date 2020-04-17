﻿using System;
using System.Reflection;
using Dryv.Translation;
using Dryv.Validation;

namespace Dryv.Configuration
{
    public class DryvOptions
    {
        public TranslationErrorBehavior TranslationErrorBehavior { get; set; }

        public Type ClientValidatorType { get; private set; }

        public void UseClientValidator<T>() where T : IDryvClientValidationProvider
        {
            this.ClientValidatorType = typeof(T);
        }

        public Type ClientBodyGeneratorType { get; private set; }

        public void UseClientBodyGenerator<T>() where T : IDryvScriptBlockGenerator
        {
            this.ClientBodyGeneratorType = typeof(T);
        }

        public bool BreakOnFirstValidationError { get; set; } = true;
    }
}