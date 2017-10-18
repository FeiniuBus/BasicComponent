using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace FeiniuBus.AspNetCore.Buffering
{
    public static class BufferingHelper
    {
        internal const int DefaultBufferThreshold = 1024 * 30;

        private static string _tempDirectory;

        private static readonly Func<string> GetTempDirectory = () => TempDirectory;

        public static string TempDirectory
        {
            get
            {
                if (_tempDirectory == null)
                {
                    var temp = Environment.GetEnvironmentVariable("ASPNETCORE_TEMP") ?? Path.GetTempPath();
                    if (!Directory.Exists(temp))
                        throw new DirectoryNotFoundException(temp);

                    _tempDirectory = temp;
                }

                return _tempDirectory;
            }
        }

        public static HttpRequest EnableRewind(this HttpRequest req, int bufferThreshold = DefaultBufferThreshold,
            long? bufferLimit = null)
        {
            if (req == null)
                throw new ArgumentNullException(nameof(req));

            var body = req.Body;
            if (!body.CanSeek)
            {
                var stream = new BufferingReadStream(body, bufferThreshold, bufferLimit, GetTempDirectory);
                req.Body = stream;
                req.HttpContext.Response.RegisterForDispose(stream);
            }

            return req;
        }
    }
}