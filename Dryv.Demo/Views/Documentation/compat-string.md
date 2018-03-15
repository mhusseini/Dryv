### String
| Compare |								 |
|--------|-------------------------------|
| `static Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase)									`{.language-csharp}	| :x: No							|
| `static Compare(String strA, int indexA, String strB, int indexB, int length, StringComparison comparisonType)					`{.language-csharp}	| :x: No							|
| `static Compare(String strA, String strB)																						`{.language-csharp}	| :heavy_check_mark: Yes 							|
| `static Compare(String strA, String strB, bool ignoreCase)																		`{.language-csharp}	| :heavy_check_mark: Yes 							|
| `static Compare(String strA, String strB, bool ignoreCase, CultureInfo culture)													`{.language-csharp}	| :warning: Yes, but culture is ignored
| `static Compare(String strA, String strB, CultureInfo culture, CompareOptions options)											`{.language-csharp}	| :heavy_check_mark: Yes 							|
| `static Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase, CultureInfo culture)				`{.language-csharp}	| :x: No 							|
| `static Compare(String strA, int indexA, String strB, int indexB, int length, CultureInfo culture, CompareOptions parameters)	`{.language-csharp}	| :x: No 							|
| `static Compare(String strA, String strB, StringComparison comparisonType)														`{.language-csharp}	| :heavy_check_mark: Yes 							|
| `static Compare(String strA, int indexA, String strB, int indexB, int length)													`{.language-csharp}	| :heavy_check_mark: Yes 							|

| CompareTo |								 |
|--------|-------------------------------|
| `CompareTo(String strB) `{.language-csharp}	| :heavy_check_mark: Yes |
| `CompareTo(object valuestrB) `{.language-csharp}	| :heavy_check_mark: Yes |	

| Contains |								 |
|--------|-------------------------------|
| `Contains(String value) `{.language-csharp}	| :heavy_check_mark: Yes |

| EndsWith |								 |
|--------|-------------------------------|
| `EndsWith(String value) `{.language-csharp}	| :heavy_check_mark: Yes |
| `EndsWith(String value, bool ignoreCase, CultureInfo culture) `{.language-csharp}	| :heavy_check_mark: Yes |
| `EndsWith(String value, StringComparison comparisonType) `{.language-csharp}	| :heavy_check_mark: Yes |
| `EndsWith(char value) `{.language-csharp}	| :heavy_check_mark: Yes |

| Equals |								 |
|--------|-------------------------------|
| `Equals(String a, String b, StringComparison comparisonType) `{.language-csharp}	| :heavy_check_mark: Yes |
| `Equals(String a, String b) `{.language-csharp}	| :heavy_check_mark: Yes |

| Format |								 |
|--------|-------------------------------|
| `Format(IFormatProvider provider, String format, object arg0) `{.language-csharp}	| :x: No |
| `Format(IFormatProvider provider, String format, object arg0, object arg1, object arg2) `{.language-csharp}	| :x: No |
| `Format(IFormatProvider provider, String format, params object[] args) `{.language-csharp}	| :x: No |
| `Format(String format, object arg0) `{.language-csharp}	| :heavy_check_mark: Yes |
| `Format(String format, params object[] args) `{.language-csharp}	| :x: No |
| `Format(String format, object arg0, object arg1, object arg2) `{.language-csharp}	| :heavy_check_mark: Yes |
| `Format(String format, object arg0, object arg1) `{.language-csharp}	| :heavy_check_mark: Yes |
| `Format(IFormatProvider provider, String format, object arg0, object arg1) `{.language-csharp}	| :x: No |

| IndexOf |								 |
|--------|-------------------------------|
| `IndexOf(char value)																	 `{.language-csharp}	| :heavy_check_mark: Yes |
| `IndexOf(char value, int startIndex)													 `{.language-csharp}	| :x: No |
| `IndexOf(char value, int startIndex, int count)										 `{.language-csharp}	| :x: No |
| `IndexOf(String value)																 `{.language-csharp}	| :heavy_check_mark: Yes |
| `IndexOf(String value, int startIndex, int count)									 `{.language-csharp}	| :x: No |
| `IndexOf(String value, int startIndex, int count, StringComparison comparisonType)	 `{.language-csharp}	| :x: No |
| `IndexOf(String value, int startIndex, StringComparison comparisonType)				 `{.language-csharp}	| :x: No |
| `IndexOf(String value, StringComparison comparisonType)								 `{.language-csharp}	| :warning: Yes, but only translates casing  |
| `IndexOf(String value, int startIndex)												 `{.language-csharp}	| :x: No |

| IsNullOrEmpty |								 |
|--------|-------------------------------|
| `IsNullOrEmpty(String value) `{.language-csharp}	| :heavy_check_mark: Yes |

| IsNullOrWhiteSpace |								 |
|--------|-------------------------------|
| `IsNullOrWhiteSpace(String value) `{.language-csharp}	| :heavy_check_mark: Yes |

| Normalize |								 |
|--------|-------------------------------|
| `Normalize() `{.language-csharp}	| :heavy_check_mark: Yes |
| `Normalize(NormalizationForm normalizationForm) `{.language-csharp}	| :warning: Yes, but ignores 2nd parameter |

| StartsWith |								 |
|--------|-------------------------------|
| `StartsWith(char value)`{.language-csharp}	| :heavy_check_mark: Yes |
| `StartsWith(String value) `{.language-csharp}	| :heavy_check_mark: Yes |
| `StartsWith(String value, bool ignoreCase, CultureInfo culture) `{.language-csharp}	| :warning: Yes, but ignores culture |
| `StartsWith(String value, StringComparison comparisonType) `{.language-csharp}	| :warning: Yes, but only translates casing |

| ToLower |								 |
|--------|-------------------------------|
| `ToLower() `{.language-csharp}	| :heavy_check_mark: Yes |
| `ToLower(CultureInfo culture) `{.language-csharp}	| :warning: Yes, but ignores culture |

| ToUpper |								 |
|--------|-------------------------------|
| `ToUpper() `{.language-csharp}	| :heavy_check_mark: Yes |
| `ToUpper(CultureInfo culture) `{.language-csharp}	| :warning: Yes, but ignores culture |

| Trim |								 |
|--------|-------------------------------|
| `Trim(char trimChar) `{.language-csharp}	| :x: No |
| `Trim(params char[] trimChars) `{.language-csharp}	| :x: No |
| `Trim() `{.language-csharp}	| :heavy_check_mark: Yes |

| TrimEnd |								 |
|--------|-------------------------------|
| `TrimEnd(char trimChar) `{.language-csharp}	| :x: No |
| `TrimEnd(params char[] trimChars) `{.language-csharp}	| :x: No |
| `TrimEnd() `{.language-csharp}	| :heavy_check_mark: Yes |

| TrimStart |								 |
|--------|-------------------------------|
| `TrimStart(char trimChar) `{.language-csharp}	| :x: No |
| `TrimStart(params char[] trimChars) `{.language-csharp}	| :x: No |
| `TrimStart() `{.language-csharp}	| :heavy_check_mark: Yes |
