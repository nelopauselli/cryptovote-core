using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class GetBlockCommand : ICommand
	{
		public string Name => "Get block";

		private readonly byte[] hash;

		public GetBlockCommand(byte[] hash)
		{
			this.hash = hash;
		}

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.GetBlock, hash.Length);
			header.CopyTo(stream);

			stream.Write(hash, 0, hash.Length);
		}
	}
}