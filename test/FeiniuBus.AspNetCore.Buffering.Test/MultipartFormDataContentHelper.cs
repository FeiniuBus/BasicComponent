using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FeiniuBus.AspNetCore.Buffering.Test
{
    internal static class MultipartFormDataContentHelper
    {
        public static MultipartFormDataContent MultipartFormDataContent(string file)
        {
            var multiPartContent = new MultipartFormDataContent("boundary=----FormBoundary7MA4YWxkTrZu0gW");

            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                multiPartContent.Add(streamContent, "file",
                    file.Substring(file.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1));
            }

            return multiPartContent;
        }
    }
}