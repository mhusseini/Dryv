$fn = "Rules``.g.cs"

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
		$ruleSwitch = ",`r`n         			Expression<Func<$($gen2)bool>> ruleSwitch = null";
		$ruleSwitchArgument = "ruleSwitch";
	}
	else {
		$ruleSwitchArgument = "null";
	}

	@"
		public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Expression<Func<TModel, $($gen2)DryvResult>> rule$ruleSwitch)
        {
			this.Add$($methodPrefix)(rule,
				new[] { $properties},
				$ruleSwitchArgument);
			return this;
        }
"@ | Add-Content $fn;
}

'using System;
using System.Linq.Expressions;

namespace Dryv
{
    partial class Rules<TModel>
    {
' | Out-File $fn

$prefixes = @("","Server","Client")

for($i = 1; $i -lt 10; $i++) {
	for($o = 0; $o -lt 9; $o++)	{
		foreach ($prefix in $prefixes) {
			WriteMethod -propCount $i -optionCount $o -methodPrefix $prefix;
		}
	}
}

'
	}
}' | Add-Content $fn
