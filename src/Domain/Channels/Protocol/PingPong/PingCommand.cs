using System.IO;

namespace Domain.Channels.Protocol.PingPong
{
	public class PingCommand : ICommand
	{
		public string Name => "PING";

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.Ping, 0);
			header.CopyTo(stream);
		}
	}
}