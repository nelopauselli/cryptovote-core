using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendUrnCommand : ICommand
	{
		private readonly Urn urn;

		public SendUrnCommand(Urn urn)
		{
			this.urn = urn;
		}

		public string Name => "Send Urn";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(urn);

			var header = new CommandHeader(CommandIds.SendUrn, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}