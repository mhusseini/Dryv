using System;

namespace Dryv.Translation
{
    public interface IDryvClientCodeModifier
    {
        string Transform(string code);
    }
}