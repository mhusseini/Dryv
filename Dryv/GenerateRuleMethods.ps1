$fn = "DryvRules``.g.cs"

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

	if($methodPrefix -eq "Parameter"){
		$gen3 = "";
		for($i = 1; $i -le $optionCount; $i++)
		{
			$gen3 += ", typeof(TOptions$i)";
		}
	
		@"
		public DryvRules<TModel> Parameter<$($gen2)TResult>(string name, Func<$($gen2)TResult> parameter)
        {
			this.AddParameter(name, parameter$gen3);
			return this;
        }
"@ | Add-Content $fn

		return;
	}

	if($methodPrefix -eq "Server"){

		@"
		public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Func<TModel, $($gen2)DryvValidationResult> rule$ruleSwitch, string ruleName = null)
        {
			this.Add$($methodPrefix)(null, rule,
				new[] { $properties},
				$ruleSwitchArgument,
				ruleName$gen3);
			return this;
        }
		public DryvRules<TModel> $($methodPrefix)Rule$gen(string groupName,
$parameters			Func<TModel, $($gen2)DryvValidationResult> rule$ruleSwitch, string ruleName = null)
        {
			this.Add$($methodPrefix)(groupName, rule,
				new[] { $properties},
				$ruleSwitchArgument,
				ruleName$gen3);
			return this;
        }
		public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Func<TModel, $($gen2)Task<DryvValidationResult>> rule$ruleSwitch, string ruleName = null)
        {
			this.Add$($methodPrefix)(null, rule,
				new[] { $properties},
				$ruleSwitchArgument,
				ruleName$gen3);
			return this;
        }
		public DryvRules<TModel> $($methodPrefix)Rule$gen(string groupName,
$parameters			Func<TModel, $($gen2)Task<DryvValidationResult>> rule$ruleSwitch, string ruleName = null)
        {
			this.Add$($methodPrefix)(groupName, rule,
				new[] { $properties},
				$ruleSwitchArgument,
				ruleName$gen3);
			return this;
        }
"@ | Add-Content $fn

		return;
	}

	if(-Not($methodPrefix)){
		@"
		public DryvRules<TModel> DisableRules$gen(
$parameters			Expression<Func<TModel, $($gen2)bool>> rule$ruleSwitch)
        {
			this.Disable(rule,
				new[] { $properties},
				$ruleSwitchArgument$gen3);
			return this;
        }
		
		public DryvRules<TModel> DisableRules$gen(
$parameters			Expression<Func<TModel, $($gen2)Task<bool>>> rule$ruleSwitch)
        {
			this.Disable(rule,
				new[] { $properties},
				$ruleSwitchArgument$gen3);
			return this;
        }
"@ | Add-Content $fn
	}

	@"
		public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Expression<Func<TModel, $($gen2)DryvValidationResult>> rule$ruleSwitch, string ruleName = null)
        {
			this.Add$($methodPrefix)(null, rule,
				new[] { $properties},
				$ruleSwitchArgument,
				ruleName$gen3);
			return this;
        }
		public DryvRules<TModel> $($methodPrefix)Rule$gen(string groupName,
$parameters			Expression<Func<TModel, $($gen2)DryvValidationResult>> rule$ruleSwitch, string ruleName = null)
        {
			this.Add$($methodPrefix)(groupName, rule,
				new[] { $properties},
				$ruleSwitchArgument,
				ruleName$gen3);
			return this;
        }
		public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Expression<Func<TModel, $($gen2)Task<DryvValidationResult>>> rule$ruleSwitch, string ruleName = null)
        {
			this.Add$($methodPrefix)(null, rule,
				new[] { $properties},
				$ruleSwitchArgument,
				ruleName$gen3);
			return this;
        }
		public DryvRules<TModel> $($methodPrefix)Rule$gen(string groupName,
$parameters			Expression<Func<TModel, $($gen2)Task<DryvValidationResult>>> rule$ruleSwitch, string ruleName = null)
        {
			this.Add$($methodPrefix)(groupName, rule,
				new[] { $properties},
				$ruleSwitchArgument,
				ruleName$gen3);
			return this;
        }
"@ | Add-Content $fn;
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

$prefixes = @("","Server","Client")

for($o = 0; $o -lt 6; $o++)	{
	WriteMethod -propCount 0 -optionCount $o -methodPrefix "Parameter";

	foreach ($prefix in $prefixes) {
		for($i = 1; $i -lt 6; $i++) {
			WriteMethod -propCount $i -optionCount $o -methodPrefix $prefix;
		}
	}
}

'
	}
}' | Add-Content $fn
