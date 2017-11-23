using System;
using Microsoft.Net.Http.Headers;

namespace FeiniuBus.AspNetCore.Logging
{
    internal static class MultipartRequestHelper
    {
        public static bool HasMultipartFormContentType(string contentType)
        {
            if (MediaTypeHeaderValue.TryParse(contentType, out var value))
            {
                return contentType != null && value.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}