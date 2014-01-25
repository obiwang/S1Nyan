using System;
using Newtonsoft.Json;

namespace S1Parser
{
    public class DateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String && 
                (objectType == typeof (DateTime) || objectType == typeof(DateTime?)))
            {
                try
                {
                    var the1970 = new DateTime(1970, 1, 1);
                    int interval = 0;
                    int.TryParse(reader.Value as string, out interval);
                    return the1970.AddSeconds(interval).ToLocalTime();
                }
                catch (Exception) { }
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteNull();
        }
    }
}