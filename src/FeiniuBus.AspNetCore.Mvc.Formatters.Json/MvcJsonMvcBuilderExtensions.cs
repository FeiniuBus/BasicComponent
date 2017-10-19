using FeiniuBus.AspNetCore.Json.Converters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeiniuBus.AspNetCore.Mvc.Formatters.Json
{
    public static class MvcJsonMvcBuilderExtensions
    {
        public static IMvcBuilder AddFeiniuBusJsonOptions(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(o =>
            {
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                o.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

                o.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                o.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
                o.SerializerSettings.Converters.Add(new IdConverter());
            });

            return builder;
        }

        public static IMvcBuilder AddFeiniuBusFrontJsonOptions(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(o =>
            {
                o.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                o.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

                o.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                o.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
                o.SerializerSettings.Converters.Add(new IdConverter());
            });

            return builder;
        }
    }
}