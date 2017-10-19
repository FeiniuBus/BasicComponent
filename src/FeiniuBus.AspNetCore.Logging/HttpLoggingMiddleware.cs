using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace FeiniuBus.AspNetCore.Logging
{
    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(context.Request.ContentType))
            {
                if (_logger != null && _logger.IsEnabled(LogLevel.Information))
                {
                    var builder = new StringBuilder();
                    var uri = context.Request.GetDisplayUrl();
                    builder.AppendFormat("RequestUri: {0}{1}", uri, Environment.NewLine);
                    builder.AppendFormat("HttpMethod: {0}", context.Request.Method);

                    if (context.Request.QueryString.HasValue)
                    {
                        builder.AppendFormat("{1}QueryString: {0}", context.Request.QueryString.ToUriComponent(),
                            Environment.NewLine);
                    }

                    if (MayContainRequestBody(context.Request))
                    {
                        if (context.Request.Body.CanSeek)
                        {
                            var reader = new StreamReader(context.Request.Body);
                            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
                            builder.AppendFormat("{1}RequestBody: {0}", body, Environment.NewLine);
                            context.Request.Body.Seek(0, SeekOrigin.Begin);
                        }
                    }
                    
                    _logger.LogInformation(builder.ToString());
                }
            }

            await _next(context);
        }
        
        private static bool MayContainRequestBody(HttpRequest request)
        {
            return request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH";
        }
    }
}