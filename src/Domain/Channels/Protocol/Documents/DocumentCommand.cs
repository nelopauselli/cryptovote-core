using System.IO;

namespace Domain.Channels.Protocol.Documents
{
	public class DocumentCommand : ICommand
	{
		private readonly Stream source;
		private readonly int size;

		public DocumentCommand(Stream source, int size)
		{
			this.source = source;
			this.size = size;
		}

		public string Name => "Send Document";

		public void Send(Stream stream)
		{
			var header = new CommandHeader(CommandIds.Document, size);
			header.CopyTo(stream);

			source.CopyTo(stream);
		}
	}
}