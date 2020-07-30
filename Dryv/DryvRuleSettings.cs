using System.Collections.Generic;

namespace Dryv
{
    public class DryvRuleSettings : Dictionary<string, object>
    {
        public DryvRuleSettings()
        {
        }

        public DryvRuleSettings(string groupName) : this(groupName, default(string))
        {
        }

        public DryvRuleSettings(string groupName, string ruleName)
        {
            this.Name = ruleName;
            this.GroupName = groupName;
        }

        public DryvRuleSettings(IDictionary<string, object> annotations) : base(annotations)
        {
        }

        public DryvRuleSettings(string groupName, IDictionary<string, object> annotations) : base(annotations)
        {
            this.GroupName = groupName;
        }

        public DryvRuleSettings(string groupName, string ruleName, IDictionary<string, object> annotations) : base(annotations)
        {
            this.Name = ruleName;
            this.GroupName = groupName;
        }

        public string GroupName { get; set; }
        public string Name { get; set; }

        public static implicit operator DryvRuleSettings(string groupName) => new DryvRuleSettings(groupName);
    }
}