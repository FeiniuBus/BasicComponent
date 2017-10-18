using FeiniuBus.AspNetCore.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeiniuBus.AspNetCore.Json
{
    public static class FeiniuBusJsonConvert
    {
        private static readonly JsonSerializerSettings Settings;

        static FeiniuBusJsonConvert()
        {
            Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss",
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            
            Settings.Converters.Add(new StringEnumConverter());
            Settings.Converters.Add(new IdConverter());
        }

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public static string SerializeObject(object value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static T DeserializeObject<T>(string value, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(value, settings);
        }
    }
}