namespace Domain.Channels.Protocol.PingPong
{
	public class PongMiddleware : IMiddleware
	{
		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.Pong) return false;

			peer.History.Add("Recibiendo PONG");

			peer.KeepAlive();

			return true;
		}
	}
}