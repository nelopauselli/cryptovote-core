using System.Text;

namespace Domain.Channels.Protocol.Echo
{
	public class EchoReplyMiddleware : IMiddleware
	{
		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.EchoReply) return false;

			var buffer = new byte[64];
			var size = peer.Read(buffer, 0, 64);

			var reply = Encoding.UTF8.GetString(buffer, 0, size);
			peer.History.Add(reply);
			
			return true;
		}
	}
}