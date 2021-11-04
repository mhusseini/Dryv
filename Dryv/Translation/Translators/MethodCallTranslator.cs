﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Dryv.Extensions;
using Dryv.Reflection;

namespace Dryv.Translation.Translators
{
    public abstract class MethodCallTranslator : IDryvMethodCallTranslator
    {
        private readonly List<RegexAndTranslator> methodTranslatorsByRegex = new List<RegexAndTranslator>();

        private readonly List<Type> supportedTypes = new List<Type>();

        public int? OrderIndex { get; set; }

        public static void WriteArguments(ITranslator translator, IEnumerable<Expression> arguments, TranslationContext context)
        {
            var sep = string.Empty;

            foreach (var argument in arguments)
            {
                context.Writer.Write(sep);
                translator.Translate(argument, context);
                sep = ", ";
            }
        }

        public virtual bool SupportsType(Type type)
        {
            return this.supportedTypes.Find(t => t.IsAssignableFrom(type)) != null;
        }

        public virtual bool Translate(MethodTranslationContext options)
        {
            var translator = this.methodTranslatorsByRegex.Where(i => i.Method.IsMatch(options.Expression.Method.Name)).Select(i => i.Translator).FirstOrDefault();

            if (translator == null)
            {
                return false;
            }

            if (!options.WhatIfMode)
            {
                translator(options);
            }

            return true;
        }

        protected static bool ArgumentIs<T>(MethodTranslationContext options, int index, T value)
        {
            return Equals(options.Expression.Arguments
                .Skip(index)
                .Take(1)
                .OfType<ConstantExpression>()
                .Select(i => i.Value)
                .FirstOrDefault(), value);
        }

        protected static T FindValue<T>(MemberInfo member)
            where T : class
        {
            return member switch
            {
                FieldInfo fieldInfo when fieldInfo.IsStatic => (fieldInfo.GetValue(null) as T),
                PropertyInfo propertyInfo when propertyInfo.GetMethod.IsStatic => (propertyInfo.GetValue(null) as T),
                _ => default
            };
        }

        protected static T FindValue<T>(params Expression[] expressions)
        {
            return FindValue<T>((IList<Expression>)expressions);
        }

        protected static T FindValue<T>(IList<Expression> expressions)
        {
            var constantExpressions = expressions.OfType<ConstantExpression>().ToList();

            return constantExpressions.Select(e => e.Value)
                .Union(from exp in constantExpressions
                       let v = exp.Value
                       from f in v?.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static |
                                                        BindingFlags.Public | BindingFlags.NonPublic |
                                                        BindingFlags.FlattenHierarchy)
                       select f.GetValue(v))
                .Union(from exp in constantExpressions
                       let v = exp.Value
                       from f in v?.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static |
                                                            BindingFlags.Public | BindingFlags.NonPublic |
                                                            BindingFlags.FlattenHierarchy)
                       select f.GetValue(v))
                .Union(from exp in expressions.OfType<MemberExpression>()
                       let obj = (exp.Expression as ConstantExpression)?.Value
                       where obj != null
                       let field = exp.Member as FieldInfo
                       let property = exp.Member as PropertyInfo
                       select field?.GetValue(obj) ?? property?.GetValue(obj))
                .OfType<T>()
                .FirstOrDefault();
        }

        protected void AddMethodTranslator(string methodName, Action<MethodTranslationContext> translator)
        {
            this.AddMethodTranslator(new Regex($"^{methodName}$"), translator);
        }

        protected void AddMethodTranslator(Regex regex, Action<MethodTranslationContext> translator)
        {
            this.methodTranslatorsByRegex.Add(new RegexAndTranslator(regex, translator));
        }

        protected void Supports(Type type)
        {
            this.supportedTypes.Add(type);
        }

        protected void Supports<T>()
        {
            this.Supports(typeof(T));
        }

        private struct RegexAndTranslator
        {
            public RegexAndTranslator(Regex method, Action<MethodTranslationContext> translator)
            {
                this.Method = method;
                this.Translator = translator;
            }

            public Regex Method { get; }
            public Action<MethodTranslationContext> Translator { get; }
        }
    }
}