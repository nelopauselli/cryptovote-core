namespace Domain.Channels.Protocol.PingPong
{
	public class PingMiddleware : IMiddleware
	{
		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.Ping) return false;

			var response = new PongCommand();
			peer.Send(response);

			return true;
		}
	}
}