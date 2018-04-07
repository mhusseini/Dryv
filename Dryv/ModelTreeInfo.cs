using System.Collections.Generic;

namespace Dryv
{
    internal class ModelTreeInfo
    {
        public Dictionary<object, string> PathsByModel { get; set; }
        public Dictionary<string, object> ModelsByPath { get; set; }
    }
}