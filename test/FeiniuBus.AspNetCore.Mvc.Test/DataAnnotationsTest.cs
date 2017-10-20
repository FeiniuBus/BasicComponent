using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FeiniuBus.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace FeiniuBus.AspNetCore.Mvc.Test
{
    public class DataAnnotationsTest
    {
        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; set; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc();
            }

            public void Configure(IApplicationBuilder builder, IHostingEnvironment env)
            {
                builder.UseMvc();
            }
        }

        [Fact]
        public async Task EnumDataAnnotation()
        {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);

            var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost/demo/a1");
            req.Content = new StringContent("{\"type\": \"abc\"}");
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var resp = await server.CreateClient().SendAsync(req).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        [Fact]
        public async Task EnumDataAnnotation1()
        {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);

            var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost/demo/a1");
            req.Content = new StringContent("{\"type\": \"name\"}");
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var resp = await server.CreateClient().SendAsync(req).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task EnumDataAnnotation2()
        {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);

            var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost/demo/a1");
            req.Content = new StringContent("{\"type\": \"Gender\"}");
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var resp = await server.CreateClient().SendAsync(req).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task EnumDataAnnotation3()
        {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);

            var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost/demo/a1");
            req.Content = new StringContent("{\"type\": \"1\"}");
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var resp = await server.CreateClient().SendAsync(req).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task EnumDataAnnotation4()
        {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);

            var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost/demo/a1");
            req.Content = new StringContent("{\"type\": 1}");
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var resp = await server.CreateClient().SendAsync(req).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task EnumDataAnnotation5()
        {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);

            var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost/demo/a1");
            req.Content = new StringContent("{\"type\": 9}");
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var resp = await server.CreateClient().SendAsync(req).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
    }

    public enum DemoEnum
    {
        Name = 1,
        Age = 2,
        Gender = 3
    }

    public class DemoModel
    {
        [JsonProperty("type")]
        [IgnoreCaseEnumDataType(typeof(DemoEnum))]
        public DemoEnum Type { get; set; }
    }

    [Route("demo")]
    public class DemoController : Controller
    {
        [HttpPost("a1")]
        public IActionResult Post([FromBody] DemoModel model)
        {
            if (ModelState.IsValid)
                return Ok();

            return BadRequest();
        }
    }
}