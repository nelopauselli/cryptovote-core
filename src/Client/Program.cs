using Domain.Converters;
using Newtonsoft.Json;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			JsonConvert.DefaultSettings = () =>
			{
				var settings = new JsonSerializerSettings();
				settings.Converters.Add(new GuidJsonConverter());
				settings.Converters.Add(new DatetimeOffsetJsonConverter());
				settings.Converters.Add(new ByteArrayJsonConverter());
				return settings;
			};

			var config = new AppConfig(args);

			using (var worker = new Worker(config))
			{
				worker.Work();
			}
		}
	}
}
