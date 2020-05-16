using System;

namespace Dryv.Translation
{
    public interface IDryvClientCodeTransformer
    {
        string Transform(string code);
    }
}