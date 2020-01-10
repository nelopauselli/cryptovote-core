using System.Text;
using System.Text.Json;
using Domain.Converters;

namespace Domain.Channels.Protocol
{
	public class Serializer
	{
		public static byte[] GetBytes(object obj)
		{
			var json = JsonSerializer.Serialize(obj, JsonDefaultSettings.Options);
			var raw = Encoding.UTF8.GetBytes(json);
			return raw;
		}

		public static T Parse<T>(byte[] raw)
		{
			var json = Encoding.UTF8.GetString(raw);
			var obj = JsonSerializer.Deserialize<T>(json, JsonDefaultSettings.Options);
			return obj;
		}
	}
}