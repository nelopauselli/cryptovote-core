using System.IO;

namespace Domain.Channels.Protocol.Authentication
{
	public class LoginCommand : ICommand
	{
		private readonly PeerInfo peerInfo;

		public LoginCommand(PeerInfo peerInfo)
		{
			this.peerInfo = peerInfo;
		}

		public string Name => "LOGIN";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(peerInfo);
			
			var header = new CommandHeader(CommandIds.Login, buffer.Length);
			header.CopyTo(stream);
			
			stream.Write(buffer, 0, buffer.Length);
		}
	}
}