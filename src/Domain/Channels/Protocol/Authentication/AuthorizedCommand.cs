using System.IO;

namespace Domain.Channels.Protocol.Authentication
{
	public class AuthorizedCommand : ICommand
	{
		private readonly PeerInfo peerInfo;

		public AuthorizedCommand(PeerInfo peerInfo)
		{
			this.peerInfo = peerInfo;
		}

		public string Name => "AUTHORIZED";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(peerInfo);

			var header = new CommandHeader(CommandIds.Authorized, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}