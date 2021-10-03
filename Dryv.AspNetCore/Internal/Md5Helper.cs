using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dryv.AspNetCore.Internal
{
    internal static class Md5Helper
    {
        private static readonly Regex RegexUrlCars = new Regex(@"[^a-z0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static string GetShortMd5(string input, int? length = null)
        {
            using var md5 = MD5.Create();
            
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            
            var hash = RegexUrlCars
                .Replace(Convert.ToBase64String(hashBytes), string.Empty)
                .ToLowerInvariant();
            
            return length.HasValue && length.Value <= hash.Length 
                ? hash.Substring(0, length.Value) 
                : hash;
        }
        
        public static string CreateMd5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString());
            }
            
            return sb.ToString();
        }
    }
}