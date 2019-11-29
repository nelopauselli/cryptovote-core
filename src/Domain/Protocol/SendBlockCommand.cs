using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class SendBlockCommand : ICommand
	{
		private readonly Block block;

		public SendBlockCommand(Block block)
		{
			this.block = block;
		}

		public string Name => $"Send Block #{block.BlockNumber}";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(block);

			var header = new CommandHeader(CommandIds.SendBlock, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}