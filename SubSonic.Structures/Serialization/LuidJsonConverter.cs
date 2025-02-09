using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSonic.Serialization
{
    public class LuidJsonConverter
        : JsonConverter<Luid>
    {
        public override Luid ReadJson(JsonReader reader, Type objectType, Luid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (CanConvert(objectType))
            {
                return serializer.Deserialize<string>(reader);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Luid value, JsonSerializer serializer)
        {
            if (CanConvert(value.GetType()))
            {
                serializer.Serialize(writer, (string)value);
            }
        }
    }
}
