using System.Text;
using Domain.Protocol;
using Domain.Utils;
using Newtonsoft.Json;

namespace Domain.Queries
{
	public class BlockQueryMessage : ProtocolMessage, IMessage<Block>
	{
		public const char CommandId = BlockQueryCommandId;

		private readonly byte[] hash;

		public BlockQueryMessage(byte[] hash)
		{
			this.hash = hash;
		}

		public override byte[] GetBytes()
		{
			var serialized = Base58.Encode(hash);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
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