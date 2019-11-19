using System.IO;

namespace Domain.Channels.Protocol.Documents
{
	public class DocumentHashCommand : ICommand
	{
		private readonly byte[] hash;

		public DocumentHashCommand(byte[] hash)
		{
			this.hash = hash;
		}

		public string Name => "Document Hash";
		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.DocumentHash, hash.Length);
			header.CopyTo(stream);

			stream.Write(hash, 0, hash.Length);
		}
	}
}