### Regex
| IsMatch |								 |
|--------|-------------------------------|
| `static IsMatch(string input, string pattern) `{.language-csharp}	| :heavy_check_mark: Yes |
| `static IsMatch(string input, string pattern, RegexOptions options) `{.language-csharp}	| :heavy_check_mark: Yes |
| `static IsMatch(string input, string pattern, RegexOptions options, TimeSpan matchTimeout) `{.language-csharp}	| :warning: Yes, but 3rd parameter is ignored.  |
| `IsMatch(string input) `{.language-csharp}	| :heavy_check_mark: Yes |
| `IsMatch(string input, int startat) `{.language-csharp}	| :warning: Yes, but 2nd parameter is ignored. |

| Match |								 |
|--------|-------------------------------|
| `static Match(string input, string pattern) `{.language-csharp}	| :warning: Yes, but see notes below. |
| `static Match(string input, string pattern, RegexOptions options) `{.language-csharp}	| :warning: Yes, but see notes below. |
| `static Match(string input, string pattern, RegexOptions options, TimeSpan matchTimeout) `{.language-csharp}	| :warning: Yes, but 3rd parameter is ignored, also see notes below.  |
| `Match(string input) `{.language-csharp}	| :warning: Yes, but see notes below. |
| `Match(string input, int startat) `{.language-csharp}	| :warning: Yes, but 2nd parameter is ignored, also see notes below. |
| `Match(string input, int startat, int length) `{.language-csharp}	| :warning: Yes, but 2nd and 3rd parameter is ignored, also see notes below. |

#### Notes
The `Match` method is not translated as is. Instead, it is only translated in conjuction with the `Success`
property of the result object. For example, the following C# code...

```csharp
m => new Regex(@"\d+").Match(m.Text).Success
```

... will be translated into the following JavaScript code:

```javascript
function (m)
{
	return /\d+/.test(m.text);
}
```

Any other uses of the `Match` method are currently not supported.