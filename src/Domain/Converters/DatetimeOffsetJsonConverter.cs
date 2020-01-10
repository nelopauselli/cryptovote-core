using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Converters
{
	public class DatetimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
		}

		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TryGetInt64(out var value))
			{
				return DateTimeOffset.FromUnixTimeMilliseconds(value);
			}

			return DateTimeOffset.FromUnixTimeMilliseconds(0);
		}

		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
		{
				var unixtime = value. ToUnixTimeMilliseconds();
				writer.WriteNumberValue(unixtime);
		}
	}
}