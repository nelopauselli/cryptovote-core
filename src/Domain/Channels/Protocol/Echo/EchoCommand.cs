using System.IO;
using System.Text;

namespace Domain.Channels.Protocol.Echo
{
	public class EchoCommand : ICommand
	{
		private readonly string message;

		public EchoCommand(string message)
		{
			this.message = message;
		}

		public string Name => "ECHO: " + message;

		public void Send(Stream stream)
		{
			var buffer = Encoding.UTF8.GetBytes(message);

			var header = new CommandHeader(CommandIds.Echo, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}