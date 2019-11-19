namespace Domain.Channels.Protocol
{
	public interface IMiddleware
	{
		bool Invoke(CommandHeader header, TcpPeer peer);
	}
}