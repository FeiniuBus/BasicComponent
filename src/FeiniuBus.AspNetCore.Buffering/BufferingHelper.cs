using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace FeiniuBus.AspNetCore.Buffering
{
    public static class BufferingHelper
    {
        private static string _tempDirectory;

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
    }
}