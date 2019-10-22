using System.Text;
using Domain.Elections;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendUrnMessage : ProtocolMessage
	{
		public const char CommandId = SendUrnCommandId;

		private readonly Urn urn;

		public SendUrnMessage(Urn urn)
		{
			this.urn = urn;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(urn, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}