using System.Text;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendQueryResponseMessage<T> : ProtocolMessage
	{
		private readonly byte[] bytes;

		public SendQueryResponseMessage(T entities)
		{
			var content = JsonConvert.SerializeObject(entities);
			var body = $"R:{content.Length:D5}|{content}";
			bytes = Encoding.UTF8.GetBytes(body);
		}

		public override byte[] GetBytes()
		{
			return bytes;
		}
	}
}