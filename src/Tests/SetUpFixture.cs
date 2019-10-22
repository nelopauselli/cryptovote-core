using Domain.Converters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
	[SetUpFixture]
	public class SetUpFixture
	{
		[OneTimeSetUp]
		public void Json()
		{
			JsonConvert.DefaultSettings = () =>
			{
				var settings = new JsonSerializerSettings();
				settings.Converters.Add(new GuidJsonConverter());
				settings.Converters.Add(new DatetimeOffsetJsonConverter());
				settings.Converters.Add(new ByteArrayJsonConverter());
				return settings;
			};
		}
	}
}