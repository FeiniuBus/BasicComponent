using FeiniuBus.AspNetCore.NLog;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using MicrosoftNLog = NLog;

namespace FeiniuBus.AspNetCore.Logging.Test
{
    public class NLogTests
    {
        [Fact]
        public void NLogTest()
        {
            var logger = NLogSharedBuilder.ConfigureNLog("FeiniuBusNLogTest").GetCurrentClassLogger();
            Assert.IsType<MicrosoftNLog.Logger>(logger);

            WebHost.CreateDefaultBuilder().UseFeiniuBusNLog().Configure(app =>
            {
                var l = app.ApplicationServices.GetRequiredService<ILogger<NLogTests>>();
                Assert.IsType<MicrosoftNLog.Logger>(l);
            });
        }
    }
}