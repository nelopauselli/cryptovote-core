using System.IO;
using System.Text;

namespace Domain.Channels.Protocol.Echo
{
	public class NotFoundCommand : ICommand
	{
		public string Name => "Not Found";
		public void Send(Stream stream)
		{
			var buffer = Encoding.UTF8.GetBytes("NOT FOUND");

			var header = new CommandHeader(CommandIds.NotFound, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer);
		}
	}

	public class EchoReplyCommand : ICommand
	{
		private readonly string message;

		public EchoReplyCommand(string message)
		{
			this.message = message;
		}

		public string Name => "REPLY: " + message;

		public void Send(Stream stream)
		{
			var buffer = Encoding.UTF8.GetBytes(message);

			var header = new CommandHeader(CommandIds.EchoReply, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}