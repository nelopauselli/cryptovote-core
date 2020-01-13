using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Utils;

namespace Domain.Converters
{
	public class ByteArrayJsonConverter : JsonConverter<byte[]>
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(byte[]);
		}

		public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			if (value != null)
				return Base58.Decode(value);

			return Array.Empty<byte>();
		}

		public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
		{
			if (value != null)
				writer.WriteStringValue(Base58.Encode(value));
			else
				writer.WriteNullValue();
		}
	}
}