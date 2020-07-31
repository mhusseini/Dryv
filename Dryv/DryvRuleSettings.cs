using System.Collections.Generic;

namespace Dryv
{
    public class DryvRuleSettings : Dictionary<string, object>
    {
        public DryvRuleSettings()
        {
        }

        public DryvRuleSettings(string group) : this(group, default(string))
        {
        }

        public DryvRuleSettings(string group, string ruleName)
        {
            this.Name = ruleName;
            this.Group = group;
        }

        public DryvRuleSettings(IDictionary<string, object> annotations) : base(annotations)
        {
        }

        public DryvRuleSettings(string group, IDictionary<string, object> annotations) : base(annotations)
        {
            this.Group = group;
        }

        public DryvRuleSettings(string group, string ruleName, IDictionary<string, object> annotations) : base(annotations)
        {
            this.Name = ruleName;
            this.Group = group;
        }

        public string Group { get; set; }
        public string Name { get; set; }

        public static implicit operator DryvRuleSettings(string group) => new DryvRuleSettings(group);
    }
}