using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SoWhenExactly.Utils
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, object> ToDictionary(this object o)
        {
            var result = new Dictionary<string, object>();
            foreach (var property in o.GetType().GetProperties())
            {
                var value = property.GetValue(o);
                result[property.Name] = value;
            }
            return result;
        }

        public static string ToJsonEncoded(this Dictionary<string, object> dictionary)
        {
            var result = JObject.FromObject(dictionary).ToString(Formatting.None);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(result));
        }

        public static Dictionary<string, object> FromJsonEncoded(this string s)
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(s));
            return JObject.Parse(json).ToObject<Dictionary<string, object>>();
        }
    }
}