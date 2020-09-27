using System.Security.Cryptography;
using System.Text;

namespace Dryv.AspNetCore.Internal
{
    internal static class Md5Helper
    {
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