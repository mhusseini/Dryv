using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dryv.AspNetCore.Middlewares
{
    public class DryvClientCodeMiddleware
    {
        private readonly RequestDelegate next;

        public DryvClientCodeMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
        }
    }
}