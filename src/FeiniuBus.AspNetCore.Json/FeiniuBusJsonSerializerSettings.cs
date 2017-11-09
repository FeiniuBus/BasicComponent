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
            
            FrontEnd = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss",
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            
            FrontEnd.Converters.Add(new StringEnumConverter());
            FrontEnd.Converters.Add(new IdConverter());
            
            GoInteraction = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyyMMddTHHmmssZ",
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            
            GoInteraction.Converters.Add(new StringEnumConverter());
            GoInteraction.Converters.Add(new IdConverter());
        }
        
        public static JsonSerializerSettings Default { get; }
        
        /// <summary>
        /// 返回给前端用这个设置
        /// </summary>
        public static JsonSerializerSettings FrontEnd { get; }
        
        public static JsonSerializerSettings GoInteraction { get; }
    }
}