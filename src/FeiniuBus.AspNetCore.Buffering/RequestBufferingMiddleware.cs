using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

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

            var originalBufferingFeature = context.Features.Get<IHttpBufferingFeature>();
            try
            {
                var stream = new BufferingReadStream(originalRequestBody, DefaultMemoryBufferThreshold,
                    DefaultBufferBodyLengthLimit, BufferingHelper.TempDirectory);
                context.Request.Body = stream;
                context.Response.RegisterForDispose(stream);
                
                if (originalBufferingFeature != null)
                {
                    context.Features.Set<IHttpBufferingFeature>(new HttpBufferingFeature(stream, originalBufferingFeature));
                }
                await _next(context);
            }
            finally
            {
                context.Features.Set(originalBufferingFeature);
                context.Request.Body = originalRequestBody;
            }
        }
    }
}