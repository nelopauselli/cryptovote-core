using System.Text;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendBlockMessage : ProtocolMessage
	{
		public const char CommandId = 'B';

		private readonly Block block;

		public SendBlockMessage(Block block)
		{
			this.block = block;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(block, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}