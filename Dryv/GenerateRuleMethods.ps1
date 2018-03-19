$fn = "Rules``.g.cs"

function WriteMethod($propCount, $optionCount)
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

	@"
		public Rules<TModel> Rule$gen(
$parameters			Expression<Func<TModel, $($gen2)DryvResult>> rule)
        {
			this.Add(rule, 
				new[] { $properties});
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

for($i = 1; $i -lt 10; $i++)
{
	for($o = 0; $o -lt 9; $o++)
	{
		WriteMethod -propCount $i -optionCoun $o;
	}
}
'
	}
}' | Add-Content $fn
