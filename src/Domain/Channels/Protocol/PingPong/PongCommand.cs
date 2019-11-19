using System.IO;

namespace Domain.Channels.Protocol.PingPong
{
	public class PongCommand : ICommand
	{
		public string Name => "PONG";

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.Pong, 0);
			header.CopyTo(stream);
		}
	}
}