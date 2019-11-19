using System.IO;

namespace Domain.Channels.Protocol.Queries
{
	public class PeersResponseCommand : ICommand
	{
		private readonly TcpPeer[] peers;

		public PeersResponseCommand(TcpPeer[] peers)
		{
			this.peers = peers;
		}

		public string Name => "RESPONSE peers";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(peers);

			var header = new CommandHeader(CommandIds.QueryResponsePeers, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}