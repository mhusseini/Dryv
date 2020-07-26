namespace Dryv.Translation.Translators
{
    public class ToStringTranslator : MethodCallTranslator
    {
        public ToStringTranslator()
        {
            this.Supports<object>();

            this.AddMethodTranslator(nameof(object.ToString), ToString);
        }

        private static void ToString(MethodTranslationContext context)
        {
            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".toString()");
        }
    }
}