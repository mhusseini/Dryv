$fn = "DryvRules``.parameters.g.cs"

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
        
		public DryvRules<TModel> Parameter<$($gen2)TResult>(string name, Func<$($gen2)Task<TResult>> parameter)
        {
			this.AddParameter(name, parameter$gen3);
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
	WriteMethod -propCount 0 -optionCount $o -methodPrefix "Parameter";
}

'
	}
}' | Add-Content $fn
