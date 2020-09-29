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
		$ruleSwitch = ",`r`n         			Func<$($gen2)bool> ruleSwitch";
		$ruleSwitchAsync = ",`r`n         			Func<$($gen2)Task<bool>> ruleSwitch";
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

	if(-Not($methodPrefix)){
		@"
		public DryvRules<TModel> DisableRules$gen(
$parameters			Expression<Func<TModel, $($gen2)bool>> rule$ruleSwitch)
        {
			this.Disable(rule,
				new[] { $properties},
				$ruleSwitchArgument
				$gen3);
			return this;
        }
"@ | Add-Content $fn
        if($ruleSwitch) {
            @"
            public DryvRules<TModel> DisableRules$gen(
    $parameters			Expression<Func<TModel, $($gen2)bool>> rule)
            {
                this.Disable(rule,
                    new[] { $properties},
                    null
                    $gen3);
                return this;
            }
            
            public DryvRules<TModel> DisableRules$gen(
    $parameters			Expression<Func<TModel, $($gen2)bool>> rule$ruleSwitchAsync)
            {
                this.Disable(rule,
                    new[] { $properties},
                    $ruleSwitchArgument
                    $gen3);
                return this;
            }
"@ | Add-Content $fn
        }
	}

	@"
		public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Expression<Func<TModel, $($gen2)DryvValidationResult>> rule$ruleSwitch, DryvRuleSettings settings = null)
        {
			this.Add$($methodPrefix)(rule,
				new[] { $properties},
				$ruleSwitchArgument,
				settings
				$gen3);
			return this;
        }
        
		public DryvRules<TModel> $($methodPrefix)Rule$gen(
$parameters			Expression<Func<TModel, $($gen2)Task<DryvValidationResult>>> rule$ruleSwitch, DryvRuleSettings settings = null)
        {
			this.Add$($methodPrefix)(rule,
				new[] { $properties},
				$ruleSwitchArgument,
				settings
				$gen3);
			return this;
        }
"@ | Add-Content $fn;
    if($ruleSwitch){
        @"
        
            public DryvRules<TModel> $($methodPrefix)Rule$gen(
    $parameters			Expression<Func<TModel, $($gen2)Task<DryvValidationResult>>> rule, DryvRuleSettings settings = null)
            {
                this.Add$($methodPrefix)(rule,
                    new[] { $properties},
                    null,
                    settings
                    $gen3);
                return this;
            }
            
            public DryvRules<TModel> $($methodPrefix)Rule$gen(
    $parameters			Expression<Func<TModel, $($gen2)DryvValidationResult>> rule, DryvRuleSettings settings = null)
            {
                this.Add$($methodPrefix)(rule,
                    new[] { $properties},
                    null,
                    settings
                    $gen3);
                return this;
            }
            
            public DryvRules<TModel> $($methodPrefix)Rule$gen(
    $parameters			Expression<Func<TModel, $($gen2)DryvValidationResult>> rule$ruleSwitchAsync, DryvRuleSettings settings = null)
            {
                this.Add$($methodPrefix)(rule,
                    new[] { $properties},
                    $ruleSwitchArgument,
                    settings
                    $gen3);
                return this;
            }
            public DryvRules<TModel> $($methodPrefix)Rule$gen(
    $parameters			Expression<Func<TModel, $($gen2)Task<DryvValidationResult>>> rule$ruleSwitchAsync, DryvRuleSettings settings = null)
            {
                this.Add$($methodPrefix)(rule,
                    new[] { $properties},
                    $ruleSwitchArgument,
                    settings
                    $gen3);
                return this;
            }
"@ | Add-Content $fn;
    }
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

$prefixes = @("","Client")

foreach ($prefix in $prefixes) {
    for($o = 0; $o -lt 6; $o++)	{
		for($i = 1; $i -lt 6; $i++) {
			WriteMethod -propCount $i -optionCount $o -methodPrefix $prefix;
		}
	}
}

'
	}
}' | Add-Content $fn
