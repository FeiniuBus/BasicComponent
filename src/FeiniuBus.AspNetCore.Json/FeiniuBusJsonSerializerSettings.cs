using FeiniuBus.AspNetCore.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeiniuBus.AspNetCore.Json
{
    public static class FeiniuBusJsonSerializerSettings
    {
        static FeiniuBusJsonSerializerSettings()
        {
            Default = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss",
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            
            Default.Converters.Add(new StringEnumConverter());
            Default.Converters.Add(new IdConverter());
        }
        
        public static JsonSerializerSettings Default { get; }
    }
}