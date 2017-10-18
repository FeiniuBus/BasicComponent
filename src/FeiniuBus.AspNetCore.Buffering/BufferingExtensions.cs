using Microsoft.AspNetCore.Builder;

namespace FeiniuBus.AspNetCore.Buffering
{
    public static class BufferingExtensions
    {
        public static IApplicationBuilder UseRequestBuffering(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestBufferingMiddleware>();
        }
    }
}