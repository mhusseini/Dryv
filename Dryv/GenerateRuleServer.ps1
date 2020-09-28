$fn = "DryvRules``.server.g.cs"

function WriteMethod($propCount, $optionCount, $methodPrefix)
{
	$gen = "";
	if($optionCount -gt 0){
		$sep = "";
		$gen = "<";
		$gen2 = "";
		for($i = 1; $i -le $optionCount; $i++)
		{
			$gen += "$($sep)TOptions$i";
			$gen2 += "TOptions$i, ";
			$sep = ", ";
		}
		$gen += ">";
	}

	$parameters = "";
	$properties = "";
	for($j = 1; $j -le $propCount; $j++)
	{
		$parameters += "			Expression<Func<TModel, object>> property$j,`r`n";
		$properties += "property$j, ";
	}

	if($optionCount -gt 0){
		$ruleSwitch = ",`r`n         			Func<$($gen2)bool> ruleSwitch = null";
		$ruleSwitchArgument = "ruleSwitch";
	}
	else {
		$ruleSwitchArgument = "null";
	}

	$gen3 = "";
	for($i = 1; $i -le $optionCount; $i++)
	{
		$gen3 += ", typeof(TOptions$i)";
	}

    @"
    public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Func<TModel, $($gen2)DryvValidationResult> rule$ruleSwitch, DryvRuleSettings settings = null)
    {
        this.Add$($methodPrefix)(rule,
            new[] { $properties},
            $ruleSwitchArgument,
            settings
            $gen3);
        return this;
    }

    public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Func<TModel, $($gen2)Task<DryvValidationResult>> rule$ruleSwitch, DryvRuleSettings settings = null)
    {
        this.Add$($methodPrefix)(rule,
            new[] { $properties},
            $ruleSwitchArgument,
            settings
            $gen3);
        return this;
    }
"@ | Add-Content $fn
}

'using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Validation;

namespace Dryv.Rules
{
    partial class DryvRules<TModel>
    {
' | Out-File $fn

for($o = 0; $o -lt 6; $o++)	{
    for($i = 1; $i -lt 6; $i++) {
        WriteMethod -propCount $i -optionCount $o -methodPrefix "Server";
    }
}

'
	}
}' | Add-Content $fn
