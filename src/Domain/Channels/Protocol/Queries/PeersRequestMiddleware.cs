using System.Linq;

namespace Domain.Channels.Protocol.Queries
{
	public class PeersRequestMiddleware : IMiddleware
	{
		private readonly IChannel channel;

		public PeersRequestMiddleware(IChannel channel)
		{
			this.channel = channel;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryRequestPeers) return false;

			peer.Send(new PeersResponseCommand(channel.Peers.ToArray()));

			return true;
		}
	}
}