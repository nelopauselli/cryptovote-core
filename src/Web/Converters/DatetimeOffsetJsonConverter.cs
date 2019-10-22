using System;
using Newtonsoft.Json;

namespace Web.Converters
{
	public class DatetimeOffsetJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is DateTimeOffset dateTimeOffset)
			{
				var unixtime = dateTimeOffset.ToUnixTimeMilliseconds();
				writer.WriteValue(unixtime);
			}
			else
				writer.WriteValue(value);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var value = reader.Value;
			if (value != null)
			{
				var unixtime = (long) value;
				return DateTimeOffset.FromUnixTimeMilliseconds(unixtime);
			}

			if (objectType == typeof(DateTimeOffset))
				return DateTimeOffset.FromUnixTimeMilliseconds(0);

			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
		}
	}
}