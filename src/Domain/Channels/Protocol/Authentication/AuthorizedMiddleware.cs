namespace Domain.Channels.Protocol.Authentication
{
	public class AuthorizedMiddleware : IMiddleware
	{
		public bool Invoke(CommandHeader header, TcpPeer peer)
		{
			if (header.CommandId != CommandIds.Authorized) return false;

			var buffer = new byte[header.Length];
			peer.Read(buffer, 0, header.Length);

			var peerInfo = Serializer.Parse<PeerInfo>(buffer);
			peer.Authenticate(peerInfo); 

			peer.IsAutenticated = true;

			return true;
		}
	}
}