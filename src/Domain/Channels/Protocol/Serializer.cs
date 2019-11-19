using System.Text;
using Newtonsoft.Json;

namespace Domain.Channels.Protocol
{
	public class Serializer
	{
		public static byte[] GetBytes(object obj)
		{
			var json = JsonConvert.SerializeObject(obj);
			var raw = Encoding.UTF8.GetBytes(json);
			return raw;
		}

		public static T Parse<T>(byte[] raw)
		{
			var json = Encoding.UTF8.GetString(raw);
			var obj = JsonConvert.DeserializeObject<T>(json);
			return obj;
		}
	}
}