using System.Text.Json;

namespace Domain.Converters
{
	public class JsonDefaultSettings
	{
		public static JsonSerializerOptions Options
		{
			get
			{
				var settings = new JsonSerializerOptions();
				settings.Converters.Add(new GuidJsonConverter());
				settings.Converters.Add(new DatetimeOffsetJsonConverter());
				settings.Converters.Add(new ByteArrayJsonConverter());
				return settings;
			}
		}
	}
}