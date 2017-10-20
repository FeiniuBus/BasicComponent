using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FeiniuBus.AspNetCore.Mvc.Formatters.Json;
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
    public class JsonFormattingTests
    {
        [Fact]
        public async Task JsonFormatter()
        {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);

            var resp = await server.CreateClient().GetAsync("http://localhost/values/list");
            resp.EnsureSuccessStatusCode();

            var body = await resp.Content.ReadAsStringAsync();
            Assert.True(body == "{\"id\":5,\"name\":\"xqlun\",\"create\":\"2017-10-20 12:06:30\"}");
        }

        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; set; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc().AddFeiniuBusJsonOptions();
            }

            public void Configure(IApplicationBuilder builder, IHostingEnvironment env)
            {
                builder.UseMvc();
            }
        }
    }

    [Route("values")]
    public class ValuesController : Controller
    {
        [HttpGet("list")]
        public IActionResult Values()
        {
            var create = new DateTime(2017, 10, 20, 12, 6, 30);
            var v = new Value
            {
                Id = 5,
                Name = "xqlun",
                Create = create
            };
            return Ok(v);
        }
    }

    public class Value
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("create")]
        public DateTime Create { get; set; }

        [JsonProperty("uid")]
        public long? UId { get; set; }
    }
}
