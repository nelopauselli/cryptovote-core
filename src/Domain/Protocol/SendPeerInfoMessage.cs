using System.Text;
using Newtonsoft.Json;

namespace Domain.Protocol
{
	public class SendPeerInfoMessage : ProtocolMessage
	{
		public const char CommandId =SendPeerCommandId;

		private readonly PeerInfo peer;

		public SendPeerInfoMessage(PeerInfo peer)
		{
			this.peer = peer;
		}

		public override byte[] GetBytes()
		{
			var serialized = JsonConvert.SerializeObject(peer, Formatting.None);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}