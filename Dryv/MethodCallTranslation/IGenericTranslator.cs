namespace Dryv.MethodCallTranslation
{
    public interface IGenericTranslator
    {
        bool TryTranslate(TranslationParameters parameters);
    }
}