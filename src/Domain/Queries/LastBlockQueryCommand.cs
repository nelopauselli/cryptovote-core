using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Queries
{
	public class LastBlockQueryCommand : ICommand<Block>
	{
		public string Name => "Last Block query";

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.QueryLastBlock, 0);
			header.CopyTo(stream);
		}

		public Block Parse(Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return Serializer.Parse<Block>(buffer);
		}
	}
}