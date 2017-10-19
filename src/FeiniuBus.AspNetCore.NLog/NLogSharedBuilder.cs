using System;
using System.IO;
using NLog;
using NLog.Web;

namespace FeiniuBus.AspNetCore.NLog
{
    public static class NLogSharedBuilder
    {
        private const string DefaultNLogConfigFile = "nlog.config";
        private static readonly string[] PotentialEnvironmentPaths = {"HOME", "USERPROFILE"};
        private const string DefaultSharedFileLocation = ".feiniubus";

        public static LogFactory ConfigureNLog(string configFile = DefaultNLogConfigFile)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                throw new ArgumentNullException(nameof(configFile));
            }

            var filePath = ResolveSharedFileLocation(configFile);
            if (string.IsNullOrEmpty(filePath))
            {
                throw new FileNotFoundException("File not exists.", filePath);
            }

            return NLogBuilder.ConfigureNLog(filePath);
        }

        private static string ResolveSharedFileLocation(string filename)
        {
            foreach (var env in PotentialEnvironmentPaths)
            {
                var envPath = Environment.GetEnvironmentVariable(env);
                if (!string.IsNullOrEmpty(envPath))
                {
                    var filePath = TestSharedFileExists(Path.Combine(envPath, DefaultSharedFileLocation, filename));
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        return filePath;
                    }
                }
            }

            return null;
        }

        private static string TestSharedFileExists(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }
    }
}