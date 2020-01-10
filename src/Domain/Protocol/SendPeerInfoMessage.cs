using System.Text;
using System.Text.Json;
using System.Xml;
using Domain.Converters;

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
			var serialized = JsonSerializer.Serialize(peer, JsonDefaultSettings.Options);
			var message = $"{CommandId}:{serialized.Length:D5}|{serialized}";
			return Encoding.UTF8.GetBytes(message);
		}
	}
}