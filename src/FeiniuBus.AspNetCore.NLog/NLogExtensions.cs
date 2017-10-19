using System;
using Microsoft.AspNetCore.Hosting;
using NLog.LayoutRenderers;
using NLog.Web;
using NLog.Web.LayoutRenderers;

namespace FeiniuBus.AspNetCore.NLog
{
    public static class NLogExtensions
    {
        public static IWebHostBuilder UseFeiniuBusNLog(this IWebHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            LayoutRenderer.Register<AspNetMvcActionRenderer>("aspnet-mvc-action");
            LayoutRenderer.Register<AspNetMvcControllerRenderer>("aspnet-mvc-controller");
            LayoutRenderer.Register<AspNetRequestHostLayoutRenderer>("aspnet-request-host");
            LayoutRenderer.Register<AspNetRequestHttpMethodRenderer>("aspnet-request-method");
            LayoutRenderer.Register<AspNetQueryStringLayoutRenderer>("aspnet-request-querystring");
            LayoutRenderer.Register<AspNetRequestUserAgent>("aspnet-request-useragent");
            LayoutRenderer.Register<AspNetRequestUrlRenderer>("aspnet-request-url");
            LayoutRenderer.Register<AspNetUserIdentityLayoutRenderer>("aspnet-user-identity");
            LayoutRenderer.Register<AspNetRequestIpLayoutRenderer>("aspnet-request-ip");
            
            return builder.UseNLog();
        }
    }
}