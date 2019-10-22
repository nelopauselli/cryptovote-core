using System;
using Newtonsoft.Json;

namespace Web.Converters
{
	public class GuidJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is Guid guid)
				writer.WriteValue(guid.ToString("N"));
			else
				writer.WriteValue(value);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var value = reader.Value;
			if (value != null)
			{
				if(Guid.TryParse((string) value.ToString(), out var guid))
					return guid;
			}

			if (objectType == typeof(Guid))
				return Guid.Empty;

			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Guid) || objectType == typeof(Guid?);
		}
	}
}