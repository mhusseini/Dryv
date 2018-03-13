using System;

namespace Dryv.MethodCallTranslation
{
    public interface IMethodCallTranslator
    {
        bool SupportsType(Type type);

        bool Translate(MethodTranslationParameters options);
    }
}