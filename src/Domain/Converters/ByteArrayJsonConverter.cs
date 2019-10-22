using System;
using Domain.Utils;
using Newtonsoft.Json;

namespace Domain.Converters
{
	public class ByteArrayJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is byte[] array)
				writer.WriteValue(Base58.Encode(array));
			else
				writer.WriteValue(value);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var value = reader.Value;
			if (value != null)
			{
				return Base58.Decode(value.ToString());
			}

			if (objectType == typeof(byte[]))
				return Array.Empty<byte>();

			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(byte[]);
		}
	}
}