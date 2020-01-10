using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;

namespace Domain.Protocol
{
	public class SendDocumentCommand : ICommand
	{
		private readonly Document document;

		public SendDocumentCommand(Document document)
		{
			this.document = document;
		}

		public string Name => "Send Document";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(document);

			var header = new CommandHeader(CommandIds.SendDocument, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}