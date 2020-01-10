using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Converters
{
	public class GuidJsonConverter : JsonConverter<Guid>
	{
		public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			if (value != null)
			{
				if (Guid.TryParse(value, out var guid))
					return guid;
			}

			return Guid.Empty;
		}

		public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString("N"));
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Guid) || objectType == typeof(Guid?);
		}

	}
}