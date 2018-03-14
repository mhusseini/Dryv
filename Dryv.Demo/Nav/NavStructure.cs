using System.Collections.Generic;

namespace Dryv.Demo.Nav
{
    public class NavStructure
    {
        public string Action { get; set; }
        public string Caption { get; set; }
        public string Controller { get; set; }
        public bool IsActive { get; internal set; }
    }
}