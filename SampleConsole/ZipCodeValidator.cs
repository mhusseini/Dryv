﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dryv.SampleConsole
{
    public class ZipCodeValidator
    {
        private static readonly Dictionary<string, string> ZipCodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["00000"] = "nowhere",
            ["00001"] = "here",
        };

        public async Task<DryvValidationResult> ValidateZipCode(string zipCode, string city)
        {
            await Task.Delay(100);

            return !ZipCodes.TryGetValue(zipCode, out var location)
                ? "The ZIP code is unknown."
                : city?.Contains(location, StringComparison.OrdinalIgnoreCase) != true
                    ? "The ZIP code does not match the city."
                    : DryvValidationResult.Success;
        }
    }
}