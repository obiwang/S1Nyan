using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using S1Parser.User;
using System;

namespace S1Parser.DZParser
{
    public static class JsonParser
    {
        public static T Parse<T>(this string json)
        {
            return DoParse(json).ToObject<T>();
        }

        public static object Parse(this string json, Type type)
        {
            return DoParse(json).ToObject(type);
        }

        private static JObject DoParse(string json)
        {
            DZHeader response;
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<JObject>(json);
                var error = jsonObject.ToObject<DZError>().Error;
                if (!string.IsNullOrEmpty(error))
                    throw new S1UserException(error);

                response = jsonObject.ToObject<DZHeader>();
            }
            catch (S1UserException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new S1UserException(UserErrorTypes.InvalidData);
            }

            if (response.Message != null)
            {
                throw new S1UserException(response.Message.Messagestr);
            }
            if (response.Variables == null)
            {
                throw new S1UserException(UserErrorTypes.InvalidData);
            }

            return response.Variables;
        }
    }
}
