using System.Text;

namespace Domain.Channels.Protocol.Echo
{
	public class NotFoundMiddleware:IMiddleware{
		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			peer.Send(new NotFoundCommand());
			return true;
		}
	}

	public class EchoMiddleware : IMiddleware
	{
		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.Echo) return false;

			var buffer = new byte[64];
			var size = peer.Read(buffer, 0, 64);

			var message = Encoding.UTF8.GetString(buffer, 0, size);

			peer.Send(new EchoReplyCommand(message));

			return true;
		}
	}
}