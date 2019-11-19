namespace Domain.Channels.Protocol.Authentication
{
	public class LoginMiddleware : IMiddleware
	{
		private readonly IChannel channel;

		public LoginMiddleware(IChannel channel)
		{
			this.channel = channel;
		}

		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.Login) return false;

			var buffer = new byte[header.Length];
			peer.Read(buffer, 0, header.Length);

			var peerInfo = Serializer.Parse<PeerInfo>(buffer);
			peer.Authenticate(peerInfo);

			peer.Send(new AuthorizedCommand(new PeerInfo{Id = channel.ID, Host = channel.ListenHost, Port = channel.ListenPort}));

			return true;
		}
	}
}