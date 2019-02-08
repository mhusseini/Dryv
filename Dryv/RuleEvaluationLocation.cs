using System;

namespace Dryv
{
    [Flags]
    public enum RuleEvaluationLocation
    {
        Server = 1,
        Client = 2
    }
}