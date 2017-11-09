using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Web;

namespace FeiniuBus.AspNetCore.NLog
{
    public static class NLogSharedBuilder
    {
        private const string DefaultNLogConfigFile = "nlog.config";
        private static readonly string[] PotentialEnvironmentPaths = {"HOME", "USERPROFILE"};
        private const string DefaultSharedFileLocation = ".feiniubus";

        public static LogFactory ConfigureNLog(string application, string configFile = DefaultNLogConfigFile)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                throw new ArgumentNullException(nameof(configFile));
            }

            var file = configFile;
            if (!File.Exists(file))
            {
                file = ResolveSharedFileLocation(configFile);
                if (string.IsNullOrEmpty(file))
                {
                    throw new FileNotFoundException("File not exists.", file);
                }
            }

            var configuration = new XmlLoggingConfiguration(file);
            configuration.Variables["FeiniuBus-Application"] = application;

            return NLogBuilder.ConfigureNLog(configuration);
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