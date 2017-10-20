using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace FeiniuBus.AspNetCore.Buffering.Test
{
    public class RequestBufferingMiddlewareTests
    {
        [Fact]
        public async Task BufferRequest_ReadBody()
        {
            var body = "hello";

            var builder = new WebHostBuilder().Configure(app =>
            {
                app.UseRequestBuffering();
                app.Run(async context =>
                {
                    Assert.True(context.Request.Body.CanRead);
                    Assert.True(context.Request.Body.CanSeek);
                    var reader = new StreamReader(context.Request.Body);
                    var text = await reader.ReadToEndAsync().ConfigureAwait(false);
                    Assert.True(text == body);

                    context.Request.Body.Seek(0, SeekOrigin.Begin);

                    var r = new StreamReader(context.Request.Body);
                    var b = await r.ReadToEndAsync().ConfigureAwait(false);
                    await context.Response.WriteAsync(b);
                });
            });

            var server = new TestServer(builder);
            var resp = await server.CreateClient().PostAsync("", new StringContent(body));
            resp.EnsureSuccessStatusCode();

            var respBody = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            Assert.True(respBody == body);
        }
    }
}