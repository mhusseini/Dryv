using System.Collections.Generic;

namespace Dryv
{
    public class ClientModelElementValidation
    {
        public IDictionary<string, string> ElementAttribute { get; set; }

        public string ValidationCode { get; set; }
        public string Name { get; set; }
    }
}