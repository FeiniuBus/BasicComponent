using FeiniuBus.AspNetCore.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeiniuBus.AspNetCore.Json
{
    public static class FeiniuBusJsonConvert
    {
        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, FeiniuBusJsonSerializerSettings.Default);
        }

        public static string SerializeObject(object value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, FeiniuBusJsonSerializerSettings.Default);
        }

        public static T DeserializeObject<T>(string value, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(value, settings);
        }
    }
}