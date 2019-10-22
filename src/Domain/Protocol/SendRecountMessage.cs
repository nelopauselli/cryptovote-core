using System.Text;
using Domain.Scrutiny;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendRecountMessage : ProtocolMessage
	{
		public const char CommandId = 'R';

		private readonly Recount recount;

		public SendRecountMessage(Recount recount)
		{
			this.recount = recount;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(recount, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}