using System.IO;

namespace Domain.Channels.Protocol.Queries
{
	public class PeersRequestCommand : ICommand
	{
		public string Name => "REQUEST peers";

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.QueryRequestPeers, 0);
			header.CopyTo(stream);
		}
	}
}