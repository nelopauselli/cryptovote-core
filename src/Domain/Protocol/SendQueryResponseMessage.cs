using System.Text;
using System.Text.Json;

namespace Domain.Protocol
{
	public class SendQueryResponseMessage<T> : ProtocolMessage
	{
		private readonly byte[] bytes;

		public SendQueryResponseMessage(T entities)
		{
			var content = JsonSerializer.Serialize(entities);
			var body = $"R:{content.Length:D5}|{content}";
			bytes = Encoding.UTF8.GetBytes(body);
		}

		public override byte[] GetBytes()
		{
			return bytes;
		}
	}
}