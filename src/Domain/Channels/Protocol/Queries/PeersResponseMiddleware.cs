namespace Domain.Channels.Protocol.Queries
{
	public class PeersResponseMiddleware : IMiddleware
	{
		private readonly IChannel channel;

		public PeersResponseMiddleware(IChannel channel)
		{
			this.channel = channel;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.QueryResponsePeers) return false;

			var buffer = new byte[header.Length];
			peer.Read(buffer, 0, header.Length);

			var peers = Serializer.Parse<PeerInfo[]>(buffer);

			foreach (var peerInfo in peers)
				channel.Connect(peerInfo);

			return true;
		}
	}
}