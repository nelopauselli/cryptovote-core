using System.IO;
using Domain.Channels;
using Domain.Channels.Protocol;
using Domain.Elections;

namespace Domain.Protocol
{
	public class SendFiscalCommand : ICommand
	{
		private readonly Fiscal fiscal;

		public SendFiscalCommand(Fiscal fiscal)
		{
			this.fiscal = fiscal;
		}

		public string Name => "Send Fiscal";

		public void Send(Stream stream)
		{
			var buffer = Serializer.GetBytes(fiscal);

			var header = new CommandHeader(CommandIds.SendFiscal, buffer.Length);
			header.CopyTo(stream);

			stream.Write(buffer, 0, buffer.Length);
		}
	}
}