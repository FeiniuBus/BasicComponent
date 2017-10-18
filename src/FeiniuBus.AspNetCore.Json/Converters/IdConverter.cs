using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FeiniuBus.AspNetCore.Json.Converters
{
    public class IdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jt = JToken.ReadFrom(reader);
            if (objectType == typeof(long))
            {
                return jt.Value<long>();
            }
            if (objectType == typeof(ulong))
            {
                return jt.Value<ulong>();
            }
            if (objectType == typeof(long?))
            {
                return jt.Value<long?>();
            }

            return jt.Value<ulong?>();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(long) == objectType || typeof(long?) == objectType || typeof(ulong) == objectType ||
                   typeof(ulong?) == objectType;
        }
    }
}