using System.Text;
using Domain.Protocol;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class LastBlockQueryMessage : ProtocolMessage, IMessage<Block>
	{
		public const char CommandId = LastBlockQueryCommandId;

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes($"{CommandId}");
		}

		public Block Parse(ProtocolMessageChannel channel)
		{
			var body = channel.GetBody();

			if (!string.IsNullOrWhiteSpace(body))
			{
				var block = JsonConvert.DeserializeObject<Block>(body);
				return block;
			}

			return null;
		}
	}
}