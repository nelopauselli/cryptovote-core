using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Queries
{
	public class BlockQueryCommand : ICommand<Block>
	{
		private readonly byte[] hash;

		public BlockQueryCommand(byte[] hash)
		{
			this.hash = hash;
		}

		public string Name => "Block query";

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.QueryBlock, hash.Length);
			header.CopyTo(stream);

			stream.Write(hash, 0, hash.Length);
		}

		public Block Parse(Stream stream, int length)
		{
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			return Serializer.Parse<Block>(buffer);
		}
	}
}