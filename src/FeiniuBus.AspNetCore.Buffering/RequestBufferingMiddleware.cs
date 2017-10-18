using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FeiniuBus.AspNetCore.Buffering
{
    public class RequestBufferingMiddleware
    {
        private const int DefaultMemoryBufferThreshold = 1024 * 64;
        private const int DefaultBufferBodyLengthLimit = 1024 * 1024 * 128;
        private readonly RequestDelegate _next;

        public RequestBufferingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalRequestBody = context.Request.Body;

            if (originalRequestBody.CanSeek)
            {
                await _next(context);
                return;
            }

            context.Request.EnableRewind(DefaultMemoryBufferThreshold, DefaultBufferBodyLengthLimit);
            await _next(context);
        }
    }
}