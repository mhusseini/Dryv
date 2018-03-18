namespace Dryv.MethodCallTranslation
{
    public interface IGenericTranslator
    {
        bool TryTranslate(GenericTranslationContext context);
    }
}